#pragma once

#include <string>
#include <vector>

#define TEAM_REGGID 1
#define TEAM_RUETOBAS 2

struct Player {
	std::string name;
	std::vector<int> cards;
	bool ready;
	int team;
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
