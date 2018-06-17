#include "Board.h"
#include "Tunnel.h"
#include <queue>
#include <tuple>
#include <algorithm>

using namespace std;

static pair <int, bool> board[19][15];
static int dis[19][15][4];
static int t[3];
static vector <Tunnel> tunnels;

void initBoard() {

	tunnels = parseTunnels("../karty_normalne.txt");
	board[5][7] = make_pair(1, 0);
	board[13][7] = make_pair(45, 0);
	board[13][5] = make_pair(45, 0);
	board[13][9] = make_pair(45, 0);

	t[0] = 42;
	t[1] = 43;
	t[2] = 44;

	random_shuffle(t, t + 3);
}

bool placeCard(int id, int x, int y, bool flip) {
	
	if (x < 1 || x > 17 || y < 1 || y > 13)
		return false;
	if (board[x][y] != make_pair(0, false))
		return false;

	board[x][y] = make_pair(id, flip);

	return true;
}

bool revealedCard(int& id, int& _x, int& _y, bool& _flip) {
	
	queue < tuple <int, int, int> > q;
	
	q.push(make_tuple(5, 7, 0));
	q.push(make_tuple(5, 7, 1));
	q.push(make_tuple(5, 7, 2));
	q.push(make_tuple(5, 7, 3));

	for (int i = 0; i < 19; i++)
		for (int j = 0; j < 15; j++)
			for (int k = 0; k < 4; k++)
				dis[i][j][k] = 123456789;

	while (!q.empty())
	{
		int x, y, z;
		tie(x, y, z) = q.front();
		q.pop();

		if (board[x][y].first == 45)
		{
			if (y == 5)
			{
				board[x][y] = make_pair(t[0], 0);
				id = t[0];
				_x = x;
				_y = y;
				_flip = 0;
			}
			else if (y == 7)
			{
				board[x][y] = make_pair(t[1], 0);
				id = t[1];
				_x = x;
				_y = y;
				_flip = 0;
			}
			else
			{
				board[x][y] = make_pair(t[2], 0);
				id = t[2];
				_x = x;
				_y = y;
				_flip = 0;
			}

			return true;
		}

		int pos = z;
		if (board[x][y].second)
			pos = (pos + 2) % 4;

		if (pos == 0)
		{
			if (board[x][y - 1].first != 0)
			{
				int pos2 = 2;
				if (board[x][y - 1].second)
					pos2 = (pos2 + 2) % 4;

				if (dis[x][y - 1][pos2] > dis[x][y][z] + 1 && tunnels[board[x][y - 1].first].open[pos2])
				{
					dis[x][y - 1][pos2] = dis[x][y][z] + 1;
					q.push(make_tuple(x, y - 1, pos2));
				}
			}
		}

		if (pos == 1)
		{
			if (board[x + 1][y].first != 0)
			{
				int pos2 = 3;
				if (board[x + 1][y].second)
					pos2 = (pos2 + 2) % 4;		
				
				if (dis[x + 1][y][pos2] > dis[x][y][z] + 1 && tunnels[board[x + 1][y].first].open[pos2])
				{
					dis[x + 1][y][pos2] = dis[x][y][z] + 1;
					q.push(make_tuple(x + 1, y, pos2));
				}
			}
		}

		if (pos == 2)
		{
			if (board[x][y + 1].first != 0)
			{
				int pos2 = 0;
				if (board[x][y + 1].second)
					pos2 = (pos2 + 2) % 4;		

				if (dis[x][y + 1][pos2] > dis[x][y][z] + 1 && tunnels[board[x][y + 1].first].open[pos2])
				{
					dis[x][y + 1][pos2] = dis[x][y][z] + 1;
					q.push(make_tuple(x, y + 1, pos2));
				}
			}
		}

		if (pos == 3)
		{
			if (board[x - 1][y].first != 0)
			{
				int pos2 = 1;
				if (board[x - 1][y].second)
					pos2 = (pos2 + 2) % 4;

				if (dis[x - 1][y][pos2] > dis[x][y][z] + 1 && tunnels[board[x - 1][y].first].open[pos2])
				{
					dis[x - 1][y][pos2] = dis[x][y][z] + 1;
					q.push(make_tuple(x - 1, y, pos2));
				}
			}
		}

		for (int i = 0; i < 4; i++)
		{
			if (i != z)
				if (tunnels[board[x][y].first].G[z][i])
					if (dis[x][y][i] > dis[x][y][z] + 1)
					{
						dis[x][y][i] = dis[x][y][z] + 1;
						q.push(make_tuple(x, y, i));
					}
		}
	}

	return false;
}
