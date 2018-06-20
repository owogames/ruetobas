#pragma once

///@brief inicjalizuje planszę
void initBoard();

///@brief sprawdza czy można położyć kartę na pozycji (x, y)
void canPlaceCard(int id, int x, int y, bool flip);
	
///@brief kładzie kartę na pozycji (x, y)
void placeCard(int id, int x, int y, bool flip);

///@brief sprawdza czy istnieje nieodryky skarb, który powinien być odkryry.
///jeśli tak, odkrywa go, ustawia id, x, y, flip, zwraca true
///jeśli nie, zwraca false
///jeśli jest więcej niż jeden, odkrywa DOKŁADNIE JEDEN z nich
bool revealedCard(int& id, int& x, int& y, bool& flip);
