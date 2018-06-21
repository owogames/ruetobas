#include <string>
#include <vector>
#include <fstream>
#include <iostream>

#include "Cards.h"
#include "Tunnel.h"

static std::vector<Tunnel> tunnels;

bool loadCards(std::string path) {
	
	std::ifstream ifs(path);
	if(!ifs.is_open()) {
		std::cerr << "Couldn't open file " << path << std::endl;
		return false;
	}
	
	bool ok = true;
	
	do {
		char c;
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
				
				
				if(ifs.good())
					tunnels.push_back(tunnel);
				else
					ok = false;
				break;
				
			case 'B':
				//TODO
				break;
				
			//TODO...
		}
		
		
	} while(ok);
	
	return true;
}

std::vector<Tunnel> getTunnels() {
	return tunnels;
}


int cardType(int card_id) {
	
}

int buffId(int card_id) {
	//TODO
	return 0;
}

int debuffId(int card_id, int flip) {
	//TODO
	return 0;
}


