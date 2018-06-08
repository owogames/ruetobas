#include <iostream>
#include <algorithm>
#include <tuple>
#include <set>

#include "ServerTCP.h"
#include "Parser.h"

int main() {
	/*
	while (true) {
		std::string s, first, second;
		std::getline(cin, s);
		std::tie(first, second) = pars(s);
		cout << first << "||" << second << "|END" << endl;
	}
	*/
	ServerTCP server(2137);
	std::set<int> fds;

	while(true) {
		int fd;
		std::string msg;
		std::tie(fd, msg) = server.read();

		if(fd != -1) {

			if(!fds.insert(fd).second) {
				for(auto fd2 : fds)
					if(fd2 != fd)
						server.write(fd2, msg);
			}
		}
	}
	
}
