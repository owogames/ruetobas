#pragma once

#include <string>
#include <vector>

#define REGGID true
#define RUETOBAS false

struct Player {
	std::string name;
	std::vector<int> cards;
	bool ready;
	bool team;
	int score;
	
	Player();
	Player(std::string name);
	
	bool hasCard(int c);
	void addCard(int c);
	void removeCard(int c);
};
