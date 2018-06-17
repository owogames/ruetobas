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
#include "Tunnel.h"
#include "Player.h"
#include "Board.h"

int main() {
	
	srand(std::chrono::system_clock::now().time_since_epoch().count());
	
	wakeMeUp(2137);
	
	std::set<std::string> usernames;
	std::map<int, Player> players;
	
	int ready_cnt = 0;
	bool running = false;
	
	std::vector<int> cards(41);
	std::iota(cards.begin(), cards.end(), 1);
	std::random_shuffle(cards.begin(), cards.end());
	
	auto curr_player_itr = players.begin();
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = read();
		std::tie(command, text) = split(msg);

		
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
				
			else {
				usernames.insert(text);
				players[fd] = Player(text);
				
				std::string username_list;
				for(auto u : players)
					if(u.first != fd) {
						write(u.first, "JOIN " + text);
						username_list += ' ' + u.second.name;
					}
				
				write(fd, "OK" + username_list);
			}
		}

		else if(command == "CHAT") {
			if(players.find(fd) == players.end())
				write(fd, "ERROR Not logged in");
				
			else if(invalidChatMsg(text))
				write(fd, "ERROR Invalid chat message");
				
			else {
				for(auto u : players)
					write(u.first, "CHAT " + text);
			}
		}

		else if(command == "QUIT") {
			for(auto p : players)
				write(p.first, "BYE " + players[fd].name);
			remove(fd);
			usernames.erase(players[fd].name);
			players.erase(fd);
		}
		
		else if(command == "READY") {
			
			if(running) 
				write(fd, "ERROR The game is already running");
				
			else if(players[fd].ready)
				write(fd, "ERROR Already ready");
				
			else {
				players[fd].ready = true;
				ready_cnt++;
				
				write(fd, "OK READY");
				for(auto p : players)
					if(p.first != fd)
						write(p.first, "READY " + players[fd].name);
				
				if(ready_cnt == players.size()) {
					
					running = true;
					initBoard();
					
					for(auto p : players) {
						std::string card_list = "START";

						for(int i = 0; i < 6; i++) {
							card_list += ' ' + toStr(cards.back());
							players[p.first].addCard(cards.back());
							cards.pop_back();
						}
						write(p.first, card_list);
					}
					
					curr_player_itr = players.begin();
					
					for(auto p : players)
						write(p.first, "TURN " + curr_player_itr->second.name);
				}
			}
		}
		
		else if(command == "PLACE") {
			std::vector<int> v;
			
			if(!running)
				write(fd, "ERROR The game is not yet running");
			
			else if(!intList(text, v) || v.size() != 4) 
				write(fd, "ERROR Incorrect command syntax");
				
			else if(fd != curr_player_itr->first)
				write(fd, "ERROR Not your turn");
				
			else {
				if(!players[fd].hasCard(v[0]))
					write(fd, "ERROR You don't have that card");
				
				else if(!placeCard(v[0], v[1], v[2], v[3])) 
					write(fd, "ERROR Invalid move");
				
				else {
					for(auto p : players)
						write(p.first, msg);
					
					players[fd].removeCard(v[0]);
						
					if(cards.empty()) 	
						write(fd, "GIB 0");
					
					else {
						write(fd, "GIB " + toStr(cards.back()));
						players[fd].addCard(cards.back());
						cards.pop_back();
					}
					
					int id, x, y;
					bool flip;
					while(revealedCard(id, x, y, flip)) {	
						for(auto p : players)
							write(p.first, "PLACE " + toStr(id) + " " + toStr(x) + " " + toStr(y) + " " + toStr(flip));
					}
					
					++curr_player_itr;
					if(curr_player_itr == players.end())
						curr_player_itr = players.begin();
					
					for(auto p : players)
						write(p.first, "TURN " + curr_player_itr->second.name);
				}
				
				
				
			}
		}
		
		else if(command == "") {}
		
		else
			write(fd, "ERROR Unknown command");
	}
	
	
	endMyLife();

}
