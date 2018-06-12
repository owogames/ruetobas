using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public enum PlayerClass { Unknown, DiggerA, DiggerB, Ruetobas, Boss, Profiteer, Pig }

    public class Player
    {
        public int ID;
        public string username;
        public int score;
        public PlayerClass playerClass;

        public Player(int ID, string username)
        {
            this.ID = ID;
            this.username = username;
            score = 0;
            playerClass = PlayerClass.Unknown;
        }
    }
}
