#include <string>
#include <algorithm>
#include <map>

#include "Logic.h"
#include "TCP.h"
#include "Player.h"
#include "StringProcessing.h"
#include "Board.h"
#include "Cards.h"


//gracze, nicki, karty, itd.
static std::map<std::string, int> usernames;
static std::map<int, Player> players;
static std::vector<int> cards;

static std::vector<int> player_order;
static int curr_player;
static bool running = false;



//////////////////////////funkcje pomocnicze///////////////////////////

///wysyła wiadomość do wszystkich zalogowanych graczy
static void writeAll(std::string msg) {
	for(auto p : players)
		write(p.first, msg);
}
///wysyła wiadomość do wszystkich zalogowanych graczy oprócz twojej starej
static void writeAllBut(int fd, std::string msg) {
	for(auto p : players)
		if(p.first != fd)
			write(p.first, msg);
}




///sprawdza, czy gra powinna się zacząć
static bool gameShouldStart() {
	if(players.empty()) return false;

	for(auto p : players)
		if(!p.second.ready) return false;
	return true;
}

///robi nową grę
static void newGame() {
	int player_cnt = players.size();
	
	if(player_cnt > 10) {
		writeAll("ERROR Too many players (limit is 10)");
		return;
	}
	
	//inicjalizuje planszę
	initBoard();
	
	//kolejność kart
	cards.clear();
	for(int i = 2; i <= 41; i++) //potem zrobie to lepiej xdd
		cards.push_back(i);
	for(int i = 46; i <= 72; i++)
		cards.push_back(i);	
		
	std::random_shuffle(cards.begin(), cards.end());
	
	//kolejność graczy (alfabetycznie)
	player_order.clear();
	for(auto p : players)
		player_order.push_back(p.first);
		
	std::sort(player_order.begin(), player_order.end(), [](int a, int b) {
		return players[a].name < players[b].name;
	});
	
	//rozdanie frakcji
	bool tab[12];
	int s = 1, k = 3, siz = players.size();

	if (siz > 3)
		k = 4;
	if (siz > 4)
		s = 2;
	if (siz > 5)
		k = 5;
	if (siz > 6)
		s = 3;
	if (siz > 7)
		k = 6;
	if (siz > 8)
		k = 7;
	if (siz > 9)
		s = 4;
	int _s = s, _k = k;

	for (int i = 0; i < s + k; i++)
	{
		if (_s > 0)
		{
			tab[i] = 1;
			_s--;
		}
		else if (_k > 0)
		{
			tab[i] = 0;
			_k--;
		}
	}

	std::random_shuffle(tab, tab + k + s);

	int nr = 0;
	for(auto& p : players)
		p.second.team = tab[nr++];

	//ustawienie pierwszego gracza
	curr_player = rand() % player_cnt;
	
	//clearowanie kart
	for(auto& p : players)
		p.second.cards.clear();
		
	//clearowanie buffów
	for(auto& p : players)
		p.second.buff_mask = 0;
	
	//rozdanie kart
	int cards_per_player = player_cnt <= 5 ? 6 :
						   player_cnt <= 7 ? 5 :
						   4;
	
	for(auto& p : players) {
		std::string card_list = "";
		for(int i = 0; i < cards_per_player; i++) {
			p.second.addCard(cards.back());
			card_list += toStr(cards.back()) + ' ';
			cards.pop_back();
		}

		write(p.first, format("START % %", card_list, p.second.team));
	}
	
	writeAll(format("TURN %", players[player_order[curr_player]].name));
}


///zabiera graczowi starą kartę i daje mu nową
void giveNewCard(int fd, int old_card) {
	players[fd].removeCard(old_card);
		
	write(fd, format("TAEK %", old_card)); 
					
	if(cards.empty())	
		write(fd, "GIB 0");
	
	else {
		write(fd, format("GIB %", cards.back()));
		players[fd].addCard(cards.back());
		cards.pop_back();
	}
}


///odkrywa skarby, zwraca true jeśli odkryto złoto
bool revealCards() {
	bool ret = false;
	
	int id, x, y;
	bool flip;
	//TODO: trzeba to zaimplementować lepiej no plox xdd
	while(revealedCard(id, x, y, flip)) {
		if(id == 43)
			ret = true;
		
		writeAll(format("PLACE % % % %", id, x, y, flip));
	}
	
	return ret;
}


///kończy turę gracza i przesuwa wskaźnik na następnego gracza, który ma jakieś karty
///zwraca fd następnego gracza, lub -1 jeśli karty się kończą
int nextPlayer() {
	for(int i = 0; i < (int)player_order.size(); i++) {
		curr_player++;
		if(curr_player >= (int)player_order.size())
			curr_player = 0;
		
		if(!players[player_order[curr_player]].cards.empty()) {
			return player_order[curr_player];
		}
	}
	return -1;
}

///kończy grę - dodaje punkty wygranym, wysyła wiadomość, resetuje wszystko
void endGame(int who_won) {
	std::string msg = format("END %", who_won);
	
	for(auto& p : players) {
		if(p.second.team == who_won)
			p.second.score++;
			
		p.second.ready = false;
		msg += format(" % % %", p.second.name, p.second.score, p.second.team);
	}

	writeAll(msg);
	
	running = false;
}



//////////////////funkcje z nagłówka///////////////////////////



//bardzo fajne makro ;--D
#define ASS(x, err) if(!(x)) {write(fd, std::string("ERROR ") + err); return;}


void login(int fd, std::string name) {
	ASS(!running, "The game is already running");
	ASS(players.find(fd) == players.end(), "Already logged in");
	ASS(usernames.find(name) == usernames.end(), "Login taken");
	ASS(name.size() <= 32, "Login too long");
	ASS(!name.empty(), "Empty login");
		
	for(auto c : name)
		ASS(c > 32 && c < 127, "Only non-whitespace printable characters allowed");
		
	writeAll("JOIN " + name);

	std::string user_list;
	for(auto p : players)
		user_list += ' ' + p.second.name + ' ' + toStr(p.second.score);
		
	write(fd, "OK" + user_list);
	
	usernames.emplace(name, fd);
	players[fd] = Player(name);	
}


void chat(int fd, std::string text) {
	ASS(players.find(fd) != players.end(), "Not logged in");
	ASS(text.size() <= 128, "Message too long");
		
	for(auto c : text)
		ASS(c >= 32 && c < 127, "Only printable characters allowed");
	
	writeAll("CHAT " + censored(text));	
}


void ready(int fd) {
	ASS(!running, "The game is already running");
	ASS(!players[fd].ready, "You're already ready");
	
	players[fd].ready = true;
	write(fd, "OK READY");
	writeAllBut(fd, "READY " + players[fd].name);
	
	if(gameShouldStart()) 
		newGame();
}


void quit(int fd) {
	//jesli gracz nie jest zalogowany to nic nie rób
	if(players.find(fd) == players.end())
		return;
	
	writeAll("BYE " + players[fd].name);
	
	//jeśli gra trwa zaktualizuj jej stan
	if(running) {
		//przesuń wskaźnik jeśli jest jego tura
		if(fd == player_order[curr_player]) {
			int fd2 = nextPlayer();
			//jeśli nikt poza nim nie ma kart (czyli wróciło do niego) to zakończ grę
			if(fd == fd2) 
				endGame(TEAM_RUETOBAS);
		}
		
		//usuń gracza z kolejności graczy i zaktualizuj wskaźnik
		int p = player_order[curr_player];
		player_order.erase(std::find(player_order.begin(), player_order.end(), fd));
		if(p != player_order[curr_player])
			curr_player--;
		
	}
	
	remove(fd);
	usernames.erase(players[fd].name);
	players.erase(fd);
	
	if(gameShouldStart()) 
		newGame();
}


void tunnel(int fd, int id, int x, int y, int flip) {
	ASS(running, "The game is not yet running");
	ASS(fd == player_order[curr_player], "Not your turn");
	ASS(players[fd].hasCard(id), "You don't have that card");
	ASS(cardType(id) == CARD_TUNNEL, "That's not a tunnel card");
	ASS(players[fd].buff_mask == 0, "You can't place tunnels, for you are blocked");
	
	ASS(placeCard(id, x, y, flip), "Invalid move");
	writeAll(format("PLACE % % % %", id, x, y, flip));

	giveNewCard(fd, id);
	int fd_next = nextPlayer();
	writeAll(format("TURN % TUNNEL % % % %", players[fd_next].name, id, x, y, flip));
	
	if(revealCards())
		endGame(TEAM_REGGID);
	
	else if(fd_next == -1)
		endGame(TEAM_RUETOBAS);
}


void crush(int fd, int id, int x, int y) {
	ASS(running, "The game is not yet running");
	ASS(fd == player_order[curr_player], "Not your turn");
	ASS(players[fd].hasCard(id), "You don't have that card");
	ASS(cardType(id) == CARD_CRUSH, "That's not a destroy card");
	
	ASS(useCrush(x, y), "You can't destroy that");
	writeAll(format("PLACE 0 % % 0", x, y));
	
	giveNewCard(fd, id);
	int fd_next = nextPlayer();
	writeAll(format("TURN % CRUSH % % %", players[fd_next].name, id, x, y));
	if(fd_next == -1)
		endGame(TEAM_RUETOBAS);
}


void map(int fd, int id, int x, int y) {
	ASS(running, "The game is not yet running");
	ASS(fd == player_order[curr_player], "Not your turn");
	ASS(players[fd].hasCard(id), "You don't have that card");
	ASS(cardType(id) == CARD_CRUSH, "That's not a map card");
	
	int ret = useMap(x, y);
	ASS(ret != -1, "Can't look there");
	write(fd, format("MAP % % %", ret, x, y));
	
	giveNewCard(fd, id);
	int fd_next = nextPlayer();
	writeAll(format("TURN % MAP % % %", players[fd_next].name, id, x, y));
	if(fd_next == -1)
		endGame(TEAM_RUETOBAS);
}


void buff(int fd, int id, std::string name) {
	ASS(running, "The game is not yet running");
	ASS(fd == player_order[curr_player], "Not your turn");
	ASS(players[fd].hasCard(id), "You don't have that card");
	ASS(cardType(id) == CARD_BUFF, "That's not a buff card");
	ASS(usernames.find(name) != usernames.end(), "Player doensn't exist");
	
	int fd2 = usernames[name];
	int buff = buffType(id);
	ASS(players[fd2].hasBuff(buff), "This player already has that buff");
	
	players[fd2].addBuff(buff);
	writeAll(format("BUFF % % %", players[fd].name, name, buff));
	
	giveNewCard(fd, id);
	int fd_next = nextPlayer();
	writeAll(format("TURN % BUFF % %", players[fd_next].name, id, name));
	if(fd_next == -1)
		endGame(TEAM_RUETOBAS);
}


void debuff(int fd, int id, std::string name, int flip) {
	ASS(running, "The game is not yet running");
	ASS(fd == player_order[curr_player], "Not your turn");
	ASS(players[fd].hasCard(id), "You don't have that card");
	ASS(cardType(id) == CARD_DEBUFF, "That's not a debuff card");
	ASS(usernames.find(name) != usernames.end(), "Player doensn't exist");
	
	int fd2 = usernames[name];
	int buff = buffType(id, flip);
	ASS(players[fd2].hasBuff(buff), "This player doesn't have that buff");
	
	players[fd2].removeBuff(buff);
	writeAll(format("DEBUFF % % %", players[fd].name, name, buff));
	
	giveNewCard(fd, id);
	int fd_next = nextPlayer();
	writeAll(format("TURN % DEBUFF % % %", players[fd_next].name, id, name, flip));
	if(fd_next == -1)
		endGame(TEAM_RUETOBAS);
}


void discard(int fd, int id) {
	ASS(running, "The game is not yet running");
	ASS(fd == player_order[curr_player], "Not your turn");
	ASS(players[fd].hasCard(id), "You don't have that card");
	
	giveNewCard(fd, id);
	int fd_next = nextPlayer();
	writeAll(format("TURN % DISCARD", players[fd_next].name));
	if(fd_next == -1)
		endGame(TEAM_RUETOBAS);
}
