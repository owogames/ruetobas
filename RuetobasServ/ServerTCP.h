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

	///przyjmuje nowe po��czenie
	int accept();

	///czyta wiadomo�� od socketa, wy��czaj�c go, je�li si� roz��czy�
	std::string getMsg(int fd);

public:
    ///tworzy serwer na danym porcie
	ServerTCP(int port);

	///zwraca now� wiadomo�� jako {numer socketa, wiadomo��}, lub {-1, ""}, kiedy nie ma �adnych wiadomo�ci
	std::pair<int, std::string> read();

	///wysy�a wiadomo�� do socketa
	void write(int fd, std::string msg);

	///wywala socketa z serwera
	void remove(int fd);

	///wyjebuje serwer
	~ServerTCP();
};
