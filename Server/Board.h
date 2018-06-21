#pragma once

///@brief inicjalizuje planszę
void initBoard();

///@brief sprawdza czy można położyć kartę na pozycji (x, y)
bool canPlaceCard(int id, int x, int y, bool flip);
///@brief kładzie kartę na pozycji (x, y)
void placeCard(int id, int x, int y, bool flip);

///@brief sprawdza czy istnieje nieodryky skarb, który powinien być odkryry.
///jeśli tak, odkrywa go, ustawia id, x, y, flip, zwraca true
///jeśli nie, zwraca false
///jeśli jest więcej niż jeden, odkrywa DOKŁADNIE JEDEN z nich
bool revealedCard(int& id, int& x, int& y, bool& flip);

///@brief sprawdza czy można użyć wyburzenia
bool canUseCrush(int x, int y);
///@brief wyburz
void useCrush(int x, int y);

///@brief sprawdza czy można użyć mapy
bool canUseMap(int x, int y);
///@brief używa mapy
int useMap(int x, int y);

