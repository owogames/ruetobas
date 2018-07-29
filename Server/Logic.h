#pragma once

#include <string>
#include <vector>

void login  (int fd, std::string name);
void chat   (int fd, std::string text);
void ready  (int fd);
void quit   (int fd);
void tunnel (int fd, int id, int x, int y, int flip);
void crush  (int fd, int id, int x, int y);
void map    (int fd, int id, int x, int y);
void buff   (int fd, int id, std::string player);
void debuff (int fd, int id, std::string player, int flip);
void discard(int fd, std::vector<int>& ids);

void admin_login(int fd, std::string passwd);
void admin_kick(int fd, std::string player);
void admin_forcestart(int fd);
void admin_forceskip(int fd);
void admin_say(int fd, std::string text);
