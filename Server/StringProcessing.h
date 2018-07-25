#pragma once

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
inline std::string format(const char* fmt) {
	return fmt;
}

template<class T, class... Ts>
inline std::string format(const char* fmt, T x, Ts... xs) {
	std::stringstream ss;
	
	for(; *fmt && *fmt != '%'; ++fmt) 
		ss << *fmt;
	 
	if(*fmt == '%') 
		ss << x << format(fmt+1, xs...);
	
	return ss.str();
}

