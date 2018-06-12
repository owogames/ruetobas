#include "Tunnel.h"

std::vector<Tunnel> parseTunnels(const char* file) {
	std::ifstream ifs(file);
	if(!ifs.is_open()) {
		std::cerr << "Couldn't open file " << file << std::endl;
		return {};
	}
	
	std::vector<Tunnel> out;
	bool ok = true;
	
	do {
		Tunnel tunnel;
		char c;
		
		ifs >> c;
		
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
			out.push_back(tunnel);
		else
			ok = false;
	} while(ok);
	
	return out;
}
