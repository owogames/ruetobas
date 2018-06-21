#pragma once

#include <string>
#include <vector>

#include "Tunnel.h"

const int NCARDS = 72;

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

bool loadCards(std::string path);

std::vector<Tunnel> getTunnels();

int cardType(int card_id);

int buffId(int card_id);
int debuffId(int card_id, int flip);
