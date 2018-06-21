#pragma once

#include <string>
#include <vector>

#include "Tunnel.h"

enum {
	CARD_TUNNEL,
	CARD_BUFF,
	CARD_DEBUFF,
	CARD_MAP,
	CARD_CRUSH,
};

enum {
	BUFF_NONE,
	BUFF_PICKAXE,
	BUFF_LANTERN,
	BUFF_CART,
};

void loadCards(std::string path);

std::vector<Tunnel> getTunnels();

int buff(int id);
int debuff(int id, int flip);
