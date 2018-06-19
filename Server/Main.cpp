#include <iostream>
#include <algorithm>
#include <numeric>
#include <tuple>
#include <string>
#include <map>
#include <set>
#include <chrono>

#include "TCP.h"
#include "StringProcessing.h"
#include "Player.h"
#include "Board.h"

static std::set<std::string> usernames;
static std::map<int, Player> players;
static std::vector<int> cards;

static std::vector<int> player_order;
static int curr_player;
static bool running = false;



void writeAll(std::string msg) {
	for(auto p : players)
		write(p.first, msg);
}


void writeAllBut(int fd, std::string msg) {
	for(auto p : players)
		if(p.first != fd)
			write(p.first, msg);
}




void newPlayer(int fd, std::string name) {
	writeAll("JOIN " + name);
	
	std::string username_list;
	for(auto u : usernames)
		username_list += ' ' + u;
		
	write(fd, "OK" + username_list);
	
	usernames.insert(name);
	players[fd] = Player(name);
}


void readyPlayer(int fd) {
	players[fd].ready = true;
				
	write(fd, "OK READY");
	writeAllBut(fd, "READY " + players[fd].name);
}


bool isEveryoneReady() {
	if(players.empty()) return false;

	for(auto p : players)
		if(!p.second.ready) return false;
	return true;
}


void newGame() {
	int player_cnt = players.size();
	
	if(player_cnt > 10) {
		writeAll("ERROR Too many players (limit is 10)");
		return;
	}
	
	//inicjalizuje planszę
	initBoard();
	
	//kolejność kart
	cards.resize(40); //const
	std::iota(cards.begin(), cards.end(), 2);
	std::random_shuffle(cards.begin(), cards.end());
	
	//kolejność graczy (alfabetycznie)
	player_order.clear();
	for(auto p : players)
		player_order.push_back(p.first);
		
	std::sort(player_order.begin(), player_order.end(), [](int a, int b) {
		return players[a].name < players[b].name;
	});
	
	//rozdanie frakcji
	const int saboteur_cnt[11] = {0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 4};
	bool team[10];
	std::fill(team, team+player_cnt, REGGID);
	std::fill(team, team+saboteur_cnt[player_cnt], RUETOBAS);
	std::random_shuffle(team, team+player_cnt);

	int nr = 0;
	for(auto& p : players)
		p.second.team = team[nr++];

	//rozdanie kart
	for(auto& p : players) {
		p.second.cards.clear();

		std::string card_list = "";
		for(int i = 0; i < 6; i++) {
			card_list += toStr(cards.back()) + ' ';
			p.second.addCard(cards.back());
			cards.pop_back();
		}
		write(p.first, "START " + card_list + (p.second.team == REGGID ? '1' : '2'));
	}
	
	//ustawienie pierwszego gracza
	curr_player = rand() % player_cnt;
	writeAll("TURN " + players[player_order[curr_player]].name);
}


void newCard(int fd, int old_card) {
	players[fd].removeCard(old_card);
					
	if(cards.empty())	
		write(fd, "GIB 0");
	
	else {
		write(fd, "GIB " + toStr(cards.back()));
		players[fd].addCard(cards.back());
		cards.pop_back();
	}
}



bool revealCards() {
	bool ret = false;
	
	int id, x, y;
	bool flip;
	while(revealedCard(id, x, y, flip)) {
		if(id == 43) //const
			ret = 1;
		
		writeAll("PLACE " + toStr(id) + " " + toStr(x) + " " + toStr(y) + " " + toStr(flip));
	}
	
	return ret;
}


bool nextPlayer() {
	for(int i = 0; i < player_order.size(); i++) {
	
		curr_player++;
		if(curr_player >= player_order.size())
			curr_player = 0;
		
		if(!players[player_order[curr_player]].cards.empty()) {
			writeAll("TURN " + players[player_order[curr_player]].name);
			return true;
		}
	}
	return false;
}


void endGame(bool who_won) {
	std::string msg = (who_won == REGGID ? "END 1 " : "END 2 ");
	
	for(auto& p : players) {
		if(p.second.team == who_won)
			p.second.score++; 
			
		p.second.ready = false;
			
		msg += p.second.name + " " + toStr(p.second.score) + " " + (p.second.team == REGGID ? "1 " : "2 ");
	}

	writeAll(msg);
	
	running = false;
}


void removePlayer(int fd) {
	writeAll("BYE " + players[fd].name);
	
	//sprawdź czy nie jest jego tura
	if(running) {
		if(fd == player_order[curr_player])
			nextPlayer();
			
		if(fd == player_order[curr_player])
			endGame(RUETOBAS);
	}
	
	//zaktualizuj kolejność graczy i wskaźnik na gracza, którego jest tura
	int p = player_order[curr_player];
	player_order.erase(std::find(player_order.begin(), player_order.end(), fd));
	if(p != player_order[curr_player])
		curr_player--;
	
	remove(fd);
	usernames.erase(players[fd].name);
	players.erase(fd);
}



////////////////////////////////////////////////////////////////////////////////////////////////



int main() {	
	srand(std::chrono::system_clock::now().time_since_epoch().count()); //top lel
	
	wakeMeUp(2137);
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = read();
		std::tie(command, text) = split(msg);

		if(!running && isEveryoneReady()) {		
			running = true;
			newGame();
		}

		if(fd == -1) continue;

		if(command == "LOGIN") {
			if(running) 
				write(fd, "ERROR The game is already running");
				
			else if(players.find(fd) != players.end())
				write(fd, "ERROR Already logged in");
				
			else if(usernames.find(text) != usernames.end())
				write(fd, "ERROR Login taken");
				
			else if(invalidLogin(text))
				write(fd, "ERROR Invalid login");
				
			else  
				newPlayer(fd, text);
		}

		else if(command == "CHAT") {
			if(players.find(fd) == players.end())
				write(fd, "ERROR Not logged in");
				
			else if(invalidChatMsg(text))
				write(fd, "ERROR Invalid chat message");
				
			else 
				writeAll("CHAT " + text);
		}

		else if(command == "QUIT") 
			removePlayer(fd);
		
		else if(command == "READY") {
			if(running) 
				write(fd, "ERROR The game is already running");
				
			else if(players[fd].ready)
				write(fd, "ERROR Already ready");
				
			else 
				readyPlayer(fd);
		}
		
		else if(command == "PLACE") {
			std::vector<int> v;
			
			if(!running)
				write(fd, "ERROR The game is not yet running");
			
			else if(!intList(text, v) || v.size() != 4) 
				write(fd, "ERROR Incorrect command syntax");
				
			else if(fd != player_order[curr_player])
				write(fd, "ERROR Not your turn");
				
			else if(!players[fd].hasCard(v[0]))
				write(fd, "ERROR You don't have that card");
				
			else if(!placeCard(v[0], v[1], v[2], v[3])) 
				write(fd, "ERROR Invalid move");
			
			else {
				writeAll(msg);
				newCard(fd, v[0]);
				if(revealCards()) 
					endGame(REGGID);
				
				else {
					if(!nextPlayer()) 
						endGame(RUETOBAS);
				}
			}
		}
		
		else if(command == "") {}
		
		else
			write(fd, "ERROR Unknown command");
	} 
	
	
	endMyLife();
}
