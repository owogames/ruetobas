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
static std::map<int, Player>::iterator curr_player_itr;


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


void removePlayer(int fd) {
	writeAll("BYE " + players[fd].name);
	
	remove(fd);
	usernames.erase(players[fd].name);
	players.erase(fd);
}


void readyPlayer(int fd) {
	players[fd].ready = true;
				
	write(fd, "OK READY");
	writeAllBut(fd, "READY " + players[fd].name);
}


bool everyoneReady() {
	if(players.empty()) return false;
	std::cout << rand() << std::endl;
	for(auto p : players)
		if(!p.second.ready) return false;
	return true;
}


void newGame() {
	initBoard();
		
	cards.resize(40); //const
	std::iota(cards.begin(), cards.end(), 2);
	std::random_shuffle(cards.begin(), cards.end());
			
	for(auto& p : players) {
		std::string card_list = "";
		p.second.cards.clear();
		
		p.second.team = rand() & 1;
		
		for(int i = 0; i < 6; i++) {
			card_list += toStr(cards.back()) + ' ';
			p.second.addCard(cards.back());
			cards.pop_back();
		}
		write(p.first, "START " + card_list + (p.second.team == REGGID ? '1' : '2'));
	}
	
	curr_player_itr = players.begin();
	
	writeAll("TURN " + curr_player_itr->second.name);
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
	++curr_player_itr;
	if(curr_player_itr == players.end())
		curr_player_itr = players.begin();
	
	for(int i = 0; i < players.size(); i++) {
	
		if(!curr_player_itr->second.cards.empty()) {
			writeAll("TURN " + curr_player_itr->second.name);
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
}


int main() {
	srand(std::chrono::system_clock::now().time_since_epoch().count()); //top lel
	
	wakeMeUp(2137);

	bool running = false;
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = read();
		std::tie(command, text) = split(msg);

		if(!running && everyoneReady()) {		
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
				
			else if(fd != curr_player_itr->first)
				write(fd, "ERROR Not your turn");
				
			else if(!players[fd].hasCard(v[0]))
				write(fd, "ERROR You don't have that card");
				
			else if(!placeCard(v[0], v[1], v[2], v[3])) 
				write(fd, "ERROR Invalid move");
			
			else {
				writeAll(msg);
				newCard(fd, v[0]);
				if(revealCards()) {
					endGame(REGGID);
				}
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
