set(CMAKE_CXX_STANDARD 11)
project(Main)
add_executable(Main Main.cpp 
                    TCP.cpp 
                    StringProcessing.cpp 
                    Tunnel.cpp
                    Player.cpp
                    Board.cpp
                    Cards.cpp
                    Logic.cpp)
if(WIN32)
  target_link_libraries(Main wsock32 ws2_32)
endif()
