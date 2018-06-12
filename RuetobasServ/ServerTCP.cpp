#include "ServerTCP.h"

#include <iostream>
#include <string>
#include <queue>

#include <cstdlib>
#include <cstring>

#ifdef WIN32

#undef UNICODE
#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>

#else

#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>

#endif

//#define MANUAL_INPUT

void fatal_err(const char* msg) {
	fprintf(stderr, "%s", msg);
    fflush(stderr);
    exit(1);
}

void err(const char* msg) {
    fprintf(stderr, "%s", msg);
    fflush(stderr);
}

const int BUFF_SIZE = 1024;



void ServerTCP::remove(int fd) {
    FD_CLR(fd, &fds);
    #ifdef WIN32
    closesocket(fd);
    #else
    close(fd);
    #endif
}



std::string ServerTCP::getMsg(int fd) {

	char buff[BUFF_SIZE];
	memset(buff, 0, BUFF_SIZE);

	int len = recv(fd, buff, BUFF_SIZE, 0);

	if(len <= 0) {
		if(len < 0) 
			printf("read failed @ socket %i\n", fd);
		else
			printf("socket %i hung up\n", fd);
			
        remove(fd);
        msg_queue.emplace(fd, "QUIT");
	}
	else
		printf("[%i]: %.*s\n", fd, len, buff);

	return buff;
}



int ServerTCP::accept() {

    socklen_t addrlen = sizeof(sockaddr_in);
	sockaddr_in client_addr;

    #ifdef WIN32
	int newsockfd = ::accept(sockfd, nullptr, nullptr);
    #else
	int newsockfd = ::accept(sockfd, (sockaddr*) &client_addr, &addrlen);
    #endif

	if(newsockfd == -1)
		err("accept");
	else {
        printf("socket %i connected\n", newsockfd);
		FD_SET(newsockfd, &fds);
		if(newsockfd > maxfd)
			maxfd = newsockfd;
	}

	return newsockfd;
}



ServerTCP::ServerTCP(int port) {

    #ifdef WIN32
	WSADATA wsaData;
	if(WSAStartup(MAKEWORD(2,2), &wsaData) != 0)
        fatal_err("WSAStartup");

	addrinfo *result = NULL;
    addrinfo hints;

	ZeroMemory(&hints, sizeof(hints));
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_STREAM;
    hints.ai_protocol = IPPROTO_TCP;
    hints.ai_flags = AI_PASSIVE;

	char port_cstr[10];
	memset(port_cstr, 0, 10);
	sprintf_s(port_cstr, 10, "%i", port);

    if(getaddrinfo(NULL, port_cstr, &hints, &result) != 0)
        fatal_err("getaddrinfo");

    sockfd = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
    if(sockfd == INVALID_SOCKET)
        fatal_err("socket");

    if(bind(sockfd, result->ai_addr, (int)result->ai_addrlen) == SOCKET_ERROR)
        fatal_err("bind");

    freeaddrinfo(result);

    if(listen(sockfd, SOMAXCONN) == SOCKET_ERROR)
        fatal_err("listen");


    #else
    sockfd = socket(AF_INET, SOCK_STREAM, 0);
	if(sockfd == -1)
		fatal_err("socket");

	sockaddr_in my_addr;
	my_addr.sin_family = AF_INET;
	my_addr.sin_port = htons(port);
	my_addr.sin_addr.s_addr = INADDR_ANY;
	memset(my_addr.sin_zero, 0, sizeof(my_addr.sin_zero));

	int yes = 1;
	if(setsockopt(sockfd, SOL_SOCKET, SO_REUSEADDR, &yes, sizeof(int)) == -1)
		fatal_err("setsockopt");

	if(bind(sockfd, (sockaddr*) &my_addr, sizeof(sockaddr)) == -1)
		fatal_err("bind");

	if(listen(sockfd, 10) == -1)
		fatal_err("listen");
    #endif



	FD_ZERO(&fds);
	FD_SET(sockfd, &fds);

	maxfd = sockfd;
}



std::pair<int, std::string> ServerTCP::read() {
	#ifdef MANUAL_INPUT
	int fd;
	std::string str;
	std::cin >> fd;
	std::getline(std::cin, str);
	return {fd, str};
	
	#else
	if(msg_queue.empty()) {

		fd_set read_fds = fds;
		timeval timeout{0, 0};

		if(select(maxfd + 1, &read_fds, NULL, NULL, &timeout) == -1)
			err("select");

		for(int fd = 0; fd <= maxfd; fd++) {
			if(FD_ISSET(fd, &read_fds)) {
				if(fd == sockfd) {
					int newfd = accept();
					if(newfd != -1)
						msg_queue.emplace(newfd, "NOP");
				}
				else
					msg_queue.emplace(fd, getMsg(fd));
			}
		}
	}

	if(msg_queue.empty())
		return {-1, ""};
	else {
		auto ret = msg_queue.front();
		msg_queue.pop();
		return ret;
	}
	
	#endif
}



void ServerTCP::write(int fd, std::string msg) {
	#ifdef MANUAL_INPUT
	std::cout << fd << ": " << msg << std::endl;
	
	#else
	if(send(fd, msg.data(), msg.size(), 0) == -1)
		err("send");
		
	#endif
}



ServerTCP::~ServerTCP() {
    #ifdef WIN32
    closesocket(sockfd);
    WSACleanup();
    #else
    //TODO
    #endif
}
