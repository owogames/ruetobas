using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public enum PlayerClass { Unknown, Reggid, Ruetobas, Boss, Profiteer, Pig }

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

        public static int CompareByName(Player p1, Player p2)
        {
            if (p1 == null && p2 == null)
                return 0;
            if (p1 == null)
                return 1;
            if (p2 == null)
                return -1;
            return p1.username.CompareTo(p2.username);
        }
    }
}
