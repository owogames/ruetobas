#include <iostream>
#include <tuple>
#include <string>
#include <chrono>

#include "TCP.h"
#include "Cards.h"
#include "StringProcessing.h"
#include "Player.h"
#include "Board.h"
#include "Logic.h"

int main() {	
	srand(std::chrono::system_clock::now().time_since_epoch().count()); //top lel
	
	wakeMeUp(2137);
	loadCards("../karty_normalne.txt");
	
	while(true) {
		int fd;
		std::string msg, command, text;
		std::tie(fd, msg) = read();
		std::tie(command, text) = split(msg);

		std::stringstream ss(text);

		#define IF_SYNTAX_OK if(ss.fail()) write(fd, "ERROR Incorrect syntax"); else

		if(command == "LOGIN") 
			login(fd, text);
	
		else if(command == "CHAT") 
			chat(fd, text);
		
		else if(command == "QUIT") 
			quit(fd);
		
		else if(command == "READY") 
			ready(fd);
		
		else if(command == "PLACE") {
			int id, x, y, flip;
			ss >> id >> x >> y >> flip;
			
			IF_SYNTAX_OK tunnel(fd, id, x, y, flip);
		}
		
		else if(command == "BUFF") {
			int id;
			std::string player;
			ss >> id >> player;
			
			IF_SYNTAX_OK buff(fd, id, player);
		}
		
		else if(command == "DEBUFF") {
			int id, flip;
			std::string player;
			ss >> id >> player >> flip;
			
			IF_SYNTAX_OK debuff(fd, id, player, flip);
		}
		
		else if(command == "CRUSH") {
			int id, x, y;
			ss >> id >> x >> y;
			
			IF_SYNTAX_OK crush(fd, id, x, y);
		}
		
		else if(command == "MAP") {
			int id, x, y;
			ss >> id >> x >> y;
			
			IF_SYNTAX_OK map(fd, id, x, y);
		}
		
		else if(command == "DISCARD") {
			std::vector<int> ids;
			if(!intList(text, ids))
				write(fd, "ERROR Incorrect syntax");
			else
				discard(fd, ids);
		}

		else if(command == "ADMIN")
			admin_login(fd, text);
		
		else if(command == "KICK")
			admin_kick(fd, text);
		
		else if(command == "FORCESTART")
			admin_forcestart(fd);
		
		else if(command == "FORCESKIP")
			admin_forceskip(fd);

		else if(command == "SAY")
			admin_say(fd, text);

		else if(command == "") {}
		
		else
			write(fd, "ERROR Unknown command");
	}
	
	endMyLife();
}
