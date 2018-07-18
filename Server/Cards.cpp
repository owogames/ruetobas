#include <string>
#include <vector>
#include <fstream>
#include <iostream>

#include "Cards.h"
#include "Tunnel.h"

static int card_type[1000];
static std::vector<Tunnel> tunnels;
static int buff_type[2][1000];


bool loadCards(std::string path) {
	
	#define PANIC(str) {std::cerr << str << path << std::endl; return false;}
	
	std::ifstream ifs(path);
	if(!ifs.is_open())
		PANIC("Couldn't open file " + path);

	
	bool ok = true;
	int card_id = 0;
	
	do {
		char c = 0;
		ifs >> c;
		
		switch(c) {
			case 'T':
				Tunnel tunnel;
				
				for(int i = 0; i < 4; i++) {
					ifs >> c;
					tunnel.open[i] = (c == '1');
				}
				
				for(int i = 0; i < 4; i++)
					for(int j = 0; j < 4; j++) {
						ifs >> c;
						tunnel.G[i][j] = (c == '1');
					}
					
				ifs >> c;
				
					 if(c == '0') tunnel.type = Tunnel::NORMAL;
				else if(c == '1') tunnel.type = Tunnel::LADDER;
				else if(c == '2') tunnel.type = Tunnel::NOGOLD;
				else if(c == '3') tunnel.type = Tunnel::CRYSTAL;
				else if(c == '4') tunnel.type = Tunnel::GOLD;
			
				tunnels.push_back(tunnel);
					
				card_type[card_id] = CARD_TUNNEL;
				
				break;
				
			case 'B':
				int buff;
				ifs >> buff;
				
				buff_type[0][card_id] = buff;
				card_type[card_id] = CARD_BUFF;
				break;
				
			case 'D':
				int buff0, buff1;
				ifs >> buff0 >> buff1;
				
				buff_type[0][card_id] = buff0;
				buff_type[1][card_id] = buff1;
				card_type[card_id] = CARD_DEBUFF;
				break;
				
			case 'M':
				card_type[card_id] = CARD_MAP;
				break;
				
			case 'C':
				card_type[card_id] = CARD_CRUSH;
				break;
				
			default:
				ok = false;
				
			
		}
		card_id++;
		
	} while(ok);
	
	return true;
}

std::vector<Tunnel> getTunnels() {
	return tunnels;
}

int cardType(int card_id) {
	return card_type[card_id];
}

int buffType(int card_id, int flip) {
	return buff_type[flip][card_id];
}


