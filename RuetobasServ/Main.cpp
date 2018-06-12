#include <iostream>
#include <algorithm>
#include <tuple>
#include <string>
#include <map>
#include <set>

#include "ServerTCP.h"
#include "Parser.h"
#include "Tunnel.h"

bool invalid_login(std::string s) {
	if(s.empty() || s.size() > 32) return true;
	for(auto c : s)
		if(c < 33 || c > 126) return true;
	return false;
}

bool invalid_chat_msg(std::string s) {
	if(s.empty() || s.size() > 128) return true;
	for(auto c : s)
		if(c < 32 || c > 126) return true;
	return false;
}

int main() {
	ServerTCP server(2137);
	std::set<std::string> nicks;
	std::map<int, std::string> users;
	
	auto tunnels = parseTunnels("../karty_normalne.txt");
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = server.read();
		std::tie(command, text) = parse(msg);

		if(fd != -1) {

            if(command == "LOGIN") {
				if(users.find(fd) != users.end())
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
            
            else
				server.write(fd, "ERROR Unknown command");
		}
	}
}
