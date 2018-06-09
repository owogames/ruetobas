#include "Parser.h"

pair<string, string> parse(string str) {
	string command = "", text = "";
	int part = 0;
	
	for(auto c : str) {
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
