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

/*
#define BUFF_NONE    0
#define BUFF_PICKAXE 1
#define BUFF_LANTERN 2
#define BUFF_CART    4
*/
//wtedy można by robić np BUFF_PICKAXE | BUFF_CART ale trzeba konwertować z formatu klucza i to wkurza :c

enum {
	BUFF_NONE,
	BUFF_PICKAXE,
	BUFF_LANTERN,
	BUFF_CART,
};

bool loadCards(std::string path);

std::vector<Tunnel> getTunnels();

int cardType(int card_id);

int buffType(int card_id, int flip = 0);

