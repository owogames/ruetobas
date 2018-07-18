#pragma once

///inicjalizuje planszę
void initBoard();

///kładzie kartę na pozycji (x, y)
bool placeCard(int id, int x, int y, bool flip);

///sprawdza czy istnieje nieodryky skarb, który powinien być odkryry.
///jeśli tak, odkrywa go, ustawia id, x, y, flip, zwraca true
///jeśli nie, zwraca false
///jeśli jest więcej niż jeden, odkrywa DOKŁADNIE JEDEN z nich
bool revealedCard(int& id, int& x, int& y, bool& flip);

///wyburz
bool useCrush(int x, int y);

///używa mapy
int useMap(int x, int y);
