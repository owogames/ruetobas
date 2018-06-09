#include <iostream>
#include <algorithm>
#include <tuple>
#include <string>
#include <map>
#include <set>

#include "ServerTCP.h"
#include "Parser.h"

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
                if(nicks.find(text) != nicks.end())
                   server.write(fd, "ERROR Login taken");
                else {
                    nicks.insert(text);
                    users[fd] = text;
                    for(auto p : users)
                        server.write(p.first, "JOIN " + text + "\n");
                }
            }

            else if(command == "CHAT") {
                for(auto p : users)
                    server.write(p.first, "CHAT " + text + "\n");
            }

            else if(command == "QUIT") {
                for(auto p : users)
                    server.write(p.first, "BYE " + users[fd] + "\n");
                server.remove(fd);
                nicks.erase(users[fd]);
                users.erase(fd);
            }
		}
	}

}
