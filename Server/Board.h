#pragma once

///@brief inicjalizuje planszę
void initBoard();


///@brief próbuje połozyć kartę na pozycji (x, y).
///jeśli się uda, kładzie ją i zwraca true,
///jeśli nie, nie kładzie jej i zwraca false.
bool placeCard(int id, int x, int y, bool flip);

///@brief sprawdza czy istnieje nieodryky skarb, który powinien być odkryry.
///jeśli tak, odkrywa go, ustawia id, x, y, flip, zwraca true
///jeśli nie, zwraca false
///jeśli jest więcej niż jeden, odkrywa DOKŁADNIE JEDEN z nich
bool revealedCard(int& id, int& x, int& y, bool& flip);
