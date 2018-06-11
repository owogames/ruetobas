#include <iostream>
#include <fstream>
#include <vector>

///karta, która jest tunelem
struct Tunnel {
	///czy z danej strony jest wejście
	bool open[4];
	///graf
	bool G[4][4];
	///czy ma kryształ
	bool crystal;
	///rodzaj tunelu
	enum {NORMAL, LADDER, NOGOLD, CRYSTAL, GOLD} type;
};

///
std::vector<Tunnel> parseTunnels(const char* file); 
