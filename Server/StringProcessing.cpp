#include <sstream>
#include <string>
#include <vector>

#include "StringProcessing.h"

std::pair<std::string, std::string> split(const std::string& str) {
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

bool invalidLogin(const std::string& str) {
	if(str.empty() || str.size() > 32) return true;
	for(auto c : str)
		if(c < 33 || c > 126) return true;
	return false;
}

bool invalidChatMsg(const std::string& str) {
	if(str.empty() || str.size() > 128) return true;
	for(auto c : str)
		if(c < 32 || c > 126) return true;
	return false;
}

bool intList(const std::string& str, std::vector<int>& v) {
	//sprawdzanie czy string jest listą liczb całkowitych
	int state = 0;
	for(auto c : str) {
		if(state == 0) {
			if(isspace(c)) state = 0;
			else if(c == '-') state = 1;
			else if(isdigit(c)) state = 2;
			else return false;
		}
		else if(state == 1) {
			if(isdigit(c)) state = 2;
			else return false;
		}
		else if(state == 2) {
			if(isspace(c)) state = 0;
			else if(isdigit(c)) state = 2;
			else return false;
		}
	}
	if(state == 1) return false;
	
	std::stringstream ss(str);
	int x;
	ss >> x;
	while(ss) {
		v.push_back(x);
		ss >> x;
	}
	return true;
}

std::string toStr(int x) {
	std::stringstream ss;
	ss << x;
	return ss.str();
}
