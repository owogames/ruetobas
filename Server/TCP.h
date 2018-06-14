#include <string>

///tworzy serwer na danym porcie
void wakeMeUp(int port);

///zwraca nową wiadomość jako {numer socketa, wiadomość}, lub {-1, ""}, kiedy nie ma żadnych wiadomości
std::pair<int, std::string> read();

///wysyła wiadomość do socketa
void write(int fd, std::string msg);

///wywala socketa z serwera
void remove(int fd);

///wyjebuje serwer
void endMyLife();

