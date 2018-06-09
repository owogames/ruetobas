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

	///przyjmuje nowe po³¹czenie
	int accept();

	///czyta wiadomoœæ od socketa, wy³¹czaj¹c go, jeœli siê roz³¹czy³
	std::string getMsg(int fd);

public:
    ///tworzy serwer na danym porcie
	ServerTCP(int port);

	///zwraca now¹ wiadomoœæ jako {numer socketa, wiadomoœæ}, lub {-1, ""}, kiedy nie ma ¿adnych wiadomoœci
	std::pair<int, std::string> read();

	///wysy³a wiadomoœæ do socketa
	void write(int fd, std::string msg);

	///wywala socketa z serwera
	void remove(int fd);

	///wyjebuje serwer
	~ServerTCP();
};
