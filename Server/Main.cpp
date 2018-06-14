#include <iostream>
#include <sstream>
#include <algorithm>
#include <numeric>
#include <tuple>
#include <string>
#include <map>
#include <set>

#include "ServerTCP.h"
#include "StringProcessing.h"
#include "Tunnel.h"
#include "Player.h"
#include "Board.h"

int main() {	
	
	ServerTCP server(2137);
	
	std::set<std::string> usernames;
	std::map<int, Player> players;
	
	int ready_cnt = 0;
	bool running = false;
	
	std::vector<int> cards(41);
	std::iota(cards.begin(), cards.end(), 1);
	std::random_shuffle(cards.begin(), cards.end());
	
	auto tunnels = parseTunnels("../karty_normalne.txt");
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = server.read();
		std::tie(command, text) = split(msg);

		
		if(fd == -1) continue;

		if(command == "LOGIN") {
			
			if(running) 
				server.write(fd, "ERROR The game is already running");
				
			else if(players.find(fd) != players.end())
				server.write(fd, "ERROR Already logged in");
				
			else if(usernames.find(text) != usernames.end())
				server.write(fd, "ERROR Login taken");
				
			else if(invalidLogin(text))
				server.write(fd, "ERROR Invalid login");
				
			else {
				usernames.insert(text);
				players[fd] = Player(text);
				
				std::string username_list;
				for(auto u : players)
					if(u.first != fd) {
						server.write(u.first, "JOIN " + text);
						username_list += ' ' + u.second.name;
					}
				
				server.write(fd, "OK" + username_list);
			}
		}

		else if(command == "CHAT") {
			if(players.find(fd) == players.end())
				server.write(fd, "ERROR Not logged in");
				
			else if(invalidChatMsg(text))
				server.write(fd, "ERROR Invalid chat message");
				
			else {
				for(auto u : players)
					server.write(u.first, "CHAT " + text);
			}
		}

		else if(command == "QUIT") {
			for(auto p : players)
				server.write(p.first, "BYE " + players[fd].name);
			server.remove(fd);
			usernames.erase(players[fd].name);
			players.erase(fd);
		}
		
		else if(command == "READY") {
			
			if(running) 
				server.write(fd, "ERROR The game is already running");
				
			else if(players[fd].ready)
				server.write(fd, "ERROR Already ready");
				
			else {
				players[fd].ready = true;
				ready_cnt++;
				
				server.write(fd, "OK READY");
				for(auto p : players)
					if(p.first != fd)
						server.write(p.first, "READY " + players[fd].name);
				
				if(ready_cnt == players.size()) {
					
					running = true;
					initBoard();
					
					for(auto p : players) {
						std::string card_list = "START";

						for(int i = 0; i < 6; i++) {
							card_list += ' ' + toStr(cards.back());
							players[fd].addCard(cards.back());
							cards.pop_back();
						}
						server.write(p.first, card_list);
					}
				}
			}
		}
		
		else if(command == "PLACE") {
			std::vector<int> v;
			
			if(!running)
				server.write(fd, "ERROR The game is not yet running");
			
			else if(!intList(text, v)) 
				server.write(fd, "ERROR Incorrect command syntax");
				
			else {
				if(!players[fd].hasCard(v[0]))
					server.write(fd, "ERROR You don't have that card");
				
				else if(!placeCard(v[0], v[1], v[2], v[3])) 
					server.write(fd, "ERROR Invalid move");
				
				else {
					for(auto p : players)
						server.write(p.first, msg);
					
					players[fd].removeCard(v[0]);
						
					if(cards.empty()) 	
						server.write(fd, "GIB 0");
					
					else {
						server.write(fd, "GIB " + toStr(cards.back()));
						players[fd].addCard(cards.back());
						cards.pop_back();
					}
					
					int id, x, y;
					bool flip;
					while(revealedCard(id, x, y, flip)) {	
						for(auto p : players)
							server.write(p.first, "PLACE " + toStr(id) + " " + toStr(x) + " " + toStr(y) + " " + toStr(flip));
					}
				}
			}
		}
		
		else if(command == "") {}
		
		else
			server.write(fd, "ERROR Unknown command");
	}

}
