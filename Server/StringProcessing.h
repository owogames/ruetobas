#include <string>
#include <vector>

///@brief rozdziel wiadomość na komendę i treść komendy
std::pair<std::string, std::string> split(const std::string& str);

///@brief sprawdza, czy login jest legitny
bool invalidLogin(const std::string& str);

///@brief sprawdza, czy wiadomość chatu jest legitna
bool invalidChatMsg(const std::string& str);

std::string censored(std::string str);

///@brief rozbija stringa na listę intów
bool intList(const std::string& str, std::vector<int>& v);

///@brief int -> string (c++ taki super język xddddd)
std::string toStr(int x);
