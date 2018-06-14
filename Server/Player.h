#pragma once

#include <string>
#include <vector>

struct Player {
	std::string name;
	std::vector<int> cards;
	bool ready;
	
	Player();
	Player(std::string name);
	
	bool hasCard(int c);
	void addCard(int c);
	void removeCard(int c);
};
