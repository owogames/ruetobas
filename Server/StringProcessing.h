#include <string>
#include <sstream>
#include <vector>

///rozdziela wiadomość na komendę i treść komendy
std::pair<std::string, std::string> split(const std::string& str);

///cenzuruje wiadomość (hehe)
std::string censored(std::string str);

///rozbija stringa na listę intów
bool intList(const std::string& str, std::vector<int>& v);


///konwertuje na stringa
template<class T>
std::string toStr(T x) {
	std::stringstream ss;
	ss << x;
	return ss.str();
}


///formatuje do stringa
template<class... Ts>
std::string format(const char* fmt, Ts... ts) {
	char buffer[1000];
	sprintf(buffer, fmt, ts...);
	return std::string(buffer);
}

