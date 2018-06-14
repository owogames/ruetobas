#include "StringProcessing.h"

std::pair<std::string, std::string> parse(const std::string& str) {
	std::string command = "", text = "";
	int part = -1;
	
	for(auto c : str) {
		if(part == -1) {
			if(!isspace(c)) part++;
		}
		if(part == 0) {
			if(isspace(c)) part++;
			else command += c;
		}
		if(part == 1) {
			if(!isspace(c)) part++;
		}
		if(part == 2) {
			text += c;
		}
	}
	
	while(!text.empty() && isspace(text.back()))
		text.pop_back();
	
	return {command, text};
}

bool invalid_login(const std::string& str) {
	if(str.empty() || str.size() > 32) return true;
	for(auto c : str)
		if(c < 33 || c > 126) return true;
	return false;
}

bool invalid_chat_msg(const std::string& str) {
	if(str.empty() || str.size() > 128) return true;
	for(auto c : str)
		if(c < 32 || c > 126) return true;
	return false;
}
