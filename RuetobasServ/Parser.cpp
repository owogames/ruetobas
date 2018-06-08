#include "Parser.h"

pair<string, string> pars(string command) {
	string first = "", second = "";
	bool first_part = true;
	for (int i = 0; i < command.length(); i++) {
		if (command[i] == ' ' & first_part) {
			first_part = false;
		}
		else if (first_part) {
			first += command[i];
		}
		else {
			second += command[i];
		}
	}
	return make_pair(first, second);
}