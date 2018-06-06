#include <iostream>
#include <string>
#include <queue>

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>


class ServerTCP {
private:
	int sockfd;
	fd_set fds;
	int maxfd;
	
	std::queue<std::pair<int, std::string>> msg_queue;
	
	int accept();
	std::string getMsg(int fd);
	
public:
	ServerTCP(int port);
	std::pair<int, std::string> read();
	void write(int fd, std::string msg);
};
