#include <iostream>
#include <string>
#include <queue>

#include <cstdlib>
#include <cstring>

#ifdef WIN32

#undef UNICODE
#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>

#else

#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>

#endif

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
	~ServerTCP();
};
