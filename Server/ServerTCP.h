#include <string>
#include <queue>

#ifdef WIN32
#include <ws2tcpip.h>
#else
//TODO: wywal niepotrzeble includy z linuxa







#endif

class ServerTCP {
private:
	int sockfd;
	fd_set fds;
	int maxfd;

	std::queue<std::pair<int, std::string>> msg_queue;

	///przyjmuje nowe połączenie
	int accept();

	///czyta wiadomość od socketa, wyłączając go, jeśli się rozłączył
	std::string getMsg(int fd);

public:
    ///tworzy serwer na danym porcie
	ServerTCP(int port);

	///zwraca nową wiadomość jako {numer socketa, wiadomość}, lub {-1, ""}, kiedy nie ma żadnych wiadomości
	std::pair<int, std::string> read();

	///wysyła wiadomość do socketa
	void write(int fd, std::string msg);

	///wywala socketa z serwera
	void remove(int fd);

	///wyjebuje serwer
	~ServerTCP();
};
