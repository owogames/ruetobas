#include <string>
#include <vector>
#include <algorithm>
#include "Player.h"

Player::Player() {}
Player::Player(std::string name): name(name), ready(false), score(0), buff_mask(0) {}

bool Player::hasCard(int c) {
	return std::find(cards.begin(), cards.end(), c) != cards.end();
}

void Player::addCard(int c) {
	cards.push_back(c);
}

void Player::removeCard(int c) {
	cards.erase(std::find(cards.begin(), cards.end(), c));
}

bool Player::hasBuff(int buff_id) {
	return buff_mask & (1<<buff_id);
}

void Player::addBuff(int buff_id) {
	buff_mask |= (1<<buff_id);
}

void Player::removeBuff(int buff_id) {
	buff_mask &= ~(1<<buff_id);
}
