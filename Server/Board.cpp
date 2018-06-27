#include "Board.h"
#include "Cards.h"
#include <algorithm>
#include <queue>
#include <tuple>
#include <algorithm>



const static int inf = 0x7fffffff;

using namespace std;

static pair <int, bool> board[19][15];
static int dis[19][15][4];
static int treasure_id[3];
static int treasure_x[3] = {13, 13, 13};
static int treasure_y[3] = {5, 7, 9};
static vector <Tunnel> tunnels;

////////////////////funkcje pomocnicze//////////////////////////////

///sprawdza czy pozycja jest git
static bool validPos(int x, int y) {
	return x >= 1 && x <= 17 && y >= 1 && y <= 13;
}

///liczy odległość od drabiny wszystkiego
static void bfs() {
	
	queue < tuple <int, int, int> > q;
	
	q.emplace(5, 7, 0);
	q.emplace(5, 7, 1);
	q.emplace(5, 7, 2);
	q.emplace(5, 7, 3);

	for (int i = 0; i < 19; i++)
		for (int j = 0; j < 15; j++)
			for (int k = 0; k < 4; k++)
				dis[i][j][k] = inf;
	
	dis[5][7][0] = 0;
	dis[5][7][1] = 0;
	dis[5][7][2] = 0;
	dis[5][7][3] = 0;

	while (!q.empty())
	{
		int x, y, z;
		tie(x, y, z) = q.front();
		q.pop();

		if (z == 0)
		{
			if (board[x][y - 1].first != 0)
			{
				int pos2 = 2;
				if (board[x][y - 1].second)
					pos2 = (pos2 + 2) % 4;

				if (dis[x][y - 1][2] > dis[x][y][z] + 1 && tunnels[board[x][y - 1].first].open[pos2])
				{
					dis[x][y - 1][2] = dis[x][y][z] + 1;
					q.emplace(x, y - 1, 2);
				}
			}
		}

		if (z == 1)
		{
			if (board[x + 1][y].first != 0)
			{
				int pos2 = 3;
				if (board[x + 1][y].second)
					pos2 = (pos2 + 2) % 4;		
				
				if (dis[x + 1][y][3] > dis[x][y][z] + 1 && tunnels[board[x + 1][y].first].open[pos2])
				{
					dis[x + 1][y][3] = dis[x][y][z] + 1;
					q.emplace(x + 1, y, 3);
				}
			}
		}

		if (z == 2)
		{
			if (board[x][y + 1].first != 0)
			{
				int pos2 = 0;
				if (board[x][y + 1].second)
					pos2 = (pos2 + 2) % 4;		

				if (dis[x][y + 1][0] > dis[x][y][z] + 1 && tunnels[board[x][y + 1].first].open[pos2])
				{
					dis[x][y + 1][0] = dis[x][y][z] + 1;
					q.emplace(x, y + 1, 0);
				}
			}
		}

		if (z == 3)
		{
			if (board[x - 1][y].first != 0)
			{
				int pos2 = 1;
				if (board[x - 1][y].second)
					pos2 = (pos2 + 2) % 4;

				if (dis[x - 1][y][1] > dis[x][y][z] + 1 && tunnels[board[x - 1][y].first].open[pos2])
				{
					dis[x - 1][y][1] = dis[x][y][z] + 1;
					q.emplace(x - 1, y, 1);
				}
			}
		}

		int pos = z;
		if (board[x][y].second)
			pos = (pos + 2) % 4;

		for (int i = 0; i < 4; i++)
		{
			int a = i;
			if (board[x][y].second)
				a = (a + 2) % 4;

			if (a != pos)
				if (tunnels[board[x][y].first].G[pos][a])
					if (dis[x][y][i] > dis[x][y][z] + 1)
					{
						dis[x][y][i] = dis[x][y][z] + 1;
						q.emplace(x, y, i);
					}
		}
	}
}

static void revealTreasure(int t) {
	int id = treasure_id[t];
	int x = treasure_x[t];
	int y = treasure_y[t];
	
	for(int i = 0; i < 4; i++) {
		if(dis[x][y][i] < inf) {
			board[x][y].first = id;
			if(!tunnels[id].open[i])
				board[x][y].second = true;
			break;
		}
	}
}

///////////////////funkcje z board.h///////////////////////

void initBoard() {

	for(int i = 0; i < 19; i++)
		for(int j = 0; j < 15; j++)
			board[i][j] = {0, false};

	tunnels = getTunnels();
	board[5][7] = make_pair(1, 0);
	
	for(int i = 0; i < 3; i++)
		board[treasure_x[i]][treasure_y[i]] = make_pair(45, 0);
	
	treasure_id[0] = 42;
	treasure_id[1] = 43;
	treasure_id[2] = 44;

	random_shuffle(treasure_id, treasure_id+3);
}

bool canPlaceCard(int id, int x, int y, bool flip) {
	if(!validPos(x, y))
		return false;
	if (board[x][y].first != 0)
		return false;
		
	const int dx[4] = {0, 1, 0, -1};
	const int dy[4] = {-1, 0, 1, 0};
		
	bfs();
	
	bool visited = false;
		
	for(int i = 0; i < 4; i++) {
		int id2 = board[x+dx[i]][y+dy[i]].first;
		if(id2 == 0) continue;
		
		int d1 = flip ? (i+2)%4 : i;
		int d2 = board[x+dx[i]][y+dy[i]].second ? i : (i+2)%4;
		
		if(tunnels[id].open[d1] != tunnels[id2].open[d2])
			return false;
		if(dis[x+dx[i]][y+dy[i]][(i+2)%4] < inf)
			visited = true;
	}
		
	return visited;
}

void placeCard(int id, int x, int y, bool flip) {
	board[x][y] = {id, flip};
}




bool revealedCard(int& id, int& x, int& y, bool& flip) {
	bfs();
	
	for(int i = 0; i < 3; i++) {
		int _x = treasure_x[i];
		int _y = treasure_y[i];
		
		if(board[_x][_y].first == 45 && (dis[_x][_y][0] < inf || 
									     dis[_x][_y][1] < inf || 
									     dis[_x][_y][2] < inf || 
									     dis[_x][_y][3] < inf)) {
			revealTreasure(i);
			id = board[_x][_y].first;
			x = _x;
			y = _y;
			flip = board[_x][_y].second;
			return true;
		}
	}
	return false;
}

bool canUseCrush(int x, int y) {
	return tunnels[board[x][y].first].type == Tunnel::NORMAL || 
		   tunnels[board[x][y].first].type == Tunnel::CRYSTAL; 
}

void useCrush(int x, int y) {
	board[x][y] = {0, false};
}


bool canUseMap(int x, int y) {
	return board[x][y].first == 45;
		   
}

int useMap(int x, int y) {
	return y == 5 ? treasure_id[0] : 
		   y == 7 ? treasure_id[1] :
					treasure_id[2];
}

