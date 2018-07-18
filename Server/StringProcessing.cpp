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

std::string censored(std::string str) {
	const std::vector<std::pair<std::string, std::string>> c = {
		{"cheat", "break the law"},
		{"ppv2", "pp"},
		{"gay", "coconut milk"},
		{"fgt", "lieutenant"},
		{"nigger", "fine sir"},
		{"loli", "fine lady"},
		{"fuck", "duck"},
		{"ddos", "tomato sauce bottle"},
		{"dildo", "poporing"},
		{"filter", "percolate"},
		{"illuminati", "baseball"},
		{"kappa", "umbrellas"},
		{"faggot", "lamborghini"},
		{"^", "I agree!"},
		{"cunt", "ant-hill"},
		{"pico", "rocket"},
		{"fag", "_"},
		{"farm", "play"},
		{"farming", "fruit picking"},
		{"silenced", "tuned out"},
		{"porn", "milk"},
		{"cancer", "rainbows"},
		{"suicide", "happyfuntime"},
		{"crai", "memes"},
		{"bancho", "mary poppins"},
		{"weed", "salmon"},
		{"bisexual", "onions"},
		{"penis", "pencil"},
		{"strawpoll.me", "osu.ppy.sh"},
		{"shigetora.pw", "monopoly.com"},
	};

	for(auto p : c) {
		size_t index = str.find(p.first);
		while(index != std::string::npos) {
			str.replace(index, p.first.size(), p.second);
			index = str.find(p.first);
		}
	}

	return str;
}
