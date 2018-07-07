#include <string>
#include <cstring>

///tworzy serwer na danym porcie
void wakeMeUp(int port);

///zwraca nową wiadomość jako {numer socketa, wiadomość}
std::pair<int, std::string> read();

///wysyła wiadomość do socketa
void write(int fd, std::string msg);

///wysyła sformatowaną wiadomość do socketa
template<class... Args>
void write(int fd, const char* fmt, Args... args) {
	char buffer[1024];
	sprintf_s(buffer, 1024, fmt, args...);
	write(fd, std::string(buffer));
}

///wywala socketa z serwera
void remove(int fd);

///wyjebuje serwer
void endMyLife();

