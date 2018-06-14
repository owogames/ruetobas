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
//#include "Board.h"

//5, 7

//const int NUM_CARDS = 

int main() {
	ServerTCP server(2137);
	
	std::set<std::string> nicks;
	std::map<int, std::string> users;
	std::set<int> ready;
	
	bool running = false;
	
	std::vector<int> cards(41);
	std::iota(cards.begin(), cards.end(), 1);
	std::random_shuffle(cards.begin(), cards.end());
	
	auto tunnels = parseTunnels("../karty_normalne.txt");
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = server.read();
		std::tie(command, text) = parse(msg);

		

		if(fd == -1) continue;

		if(command == "LOGIN") {
			if(running) 
				server.write(fd, "ERROR The game is already running");
			else if(users.find(fd) != users.end())
				server.write(fd, "ERROR Already logged in");
			else if(nicks.find(text) != nicks.end())
				server.write(fd, "ERROR Login taken");
			else if(invalid_login(text))
				server.write(fd, "ERROR Invalid login");
				
			else {
				nicks.insert(text);
				users[fd] = text;
				
				std::string usernames;
				for(auto u : users)
					if(u.first != fd) {
						server.write(u.first, "JOIN " + text);
						usernames += ' ' + u.second;
					}
				
				server.write(fd, "OK" + usernames);
			}
		}

		else if(command == "CHAT") {
			if(users.find(fd) == users.end())
				server.write(fd, "ERROR Not logged in");
			else if(invalid_chat_msg(text))
				server.write(fd, "ERROR Invalid chat message");
			else {
				for(auto u : users)
					server.write(u.first, "CHAT " + text);
			}
		}

		else if(command == "QUIT") {
			for(auto u : users)
				server.write(u.first, "BYE " + users[fd]);
			server.remove(fd);
			nicks.erase(users[fd]);
			users.erase(fd);
		}
		
		else if(command == "READY") {
			if(running) 
				server.write(fd, "ERROR The game is already running");
			else if(ready.find(fd) != ready.end())
				server.write(fd, "ERROR Already ready");
			else {
				ready.insert(fd);
				
				server.write(fd, "OK");
				for(auto u : users)
					if(u.first != fd)
						server.write(u.first, "READY " + users[fd]);
				
				if(ready.size() == users.size()) {
					running = true;
					
					for(auto u : users) {
						std::stringstream ss;
						ss << "START";
						for(int i = 0; i < 6; i++) {
							ss << ' ' << cards.back();
							cards.pop_back();
						}
						server.write(u.first, ss.str());
					}
						
				}
			}
		}
		
		else if(command == "NOP") {}
		
		else
			server.write(fd, "ERROR Unknown command");
	}

}
