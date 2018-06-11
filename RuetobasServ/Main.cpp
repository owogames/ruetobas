#include <iostream>
#include <algorithm>
#include <tuple>
#include <string>
#include <map>
#include <set>

#include "ServerTCP.h"
#include "Parser.h"

bool invalid_login(std::string s) {
	if(s.empty()) return true;
	for(auto c : s) 
		if(c < 32 || c > 126) return true;
	return false;
}

int main() {
	ServerTCP server(2137);
	std::set<std::string> nicks;
	std::map<int, std::string> users;
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = server.read();
		std::tie(command, text) = parse(msg);

		if(fd != -1) {

            if(command == "LOGIN") {
				if(users.find(fd) != users.end())
					server.write(fd, "ERROR Already Logged In");
				
                else if(nicks.find(text) != nicks.end())
                   server.write(fd, "ERROR Login taken");
                   
				else if(invalid_login(text))
					server.write(fd, "ERROR Invalid Login");
					
                else {
                    nicks.insert(text);
                    users[fd] = text;
                    for(auto u : users)
                        server.write(u.first, "JOIN " + text);
                }
            }

            else if(command == "CHAT") {
				if(users.find(fd) == users.end())
					server.write(fd, "ERROR Not Logged In");
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
		}
	}
}
