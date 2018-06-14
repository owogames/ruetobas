#include <string>
//#include <utility>

///@brief rozdziel wiadomość na komendę i treść komendy
std::pair<std::string, std::string> parse(const std::string& str);

///@brief sprawdza, czy login jest legitny
bool invalid_login(const std::string& str);

///@brief sprawdza, czy wiadomość chatu jest legitna
bool invalid_chat_msg(const std::string& str);
