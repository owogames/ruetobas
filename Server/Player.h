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
	int buff_mask;
	
	Player();
	Player(std::string name);
	
	bool hasCard(int c);
	void addCard(int c);
	void removeCard(int c);
	
	bool hasBuff(int buff_id);
	void addBuff(int buff_id);
	void removeBuff(int buff_id);
};
