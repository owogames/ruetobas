using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ruetobas
{
    public enum PlayerClass { Unknown, Reggid, Ruetobas, Boss, Profiteer, Pig }
    public enum Buff { None, Pickaxe, Lantern, Cart }

    public class Player
    {
        public int ID;
        public string username;
        public int score;
        public PlayerClass playerClass;
        public List<Buff> buffs;

        public Player(int ID, string username)
        {
            this.ID = ID;
            this.username = username;
            score = 0;
            playerClass = PlayerClass.Unknown;
            buffs = new List<Buff>();
        }

        public bool AddBuff(Buff buff)
        {
            if (buffs.Contains(buff))
                return false;
            buffs.Add(buff);
            return true;
        }

        public bool RemoveBuff(Buff buff)
        {
            if (!buffs.Contains(buff))
                return false;
            buffs.Remove(buff);
            return true;
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

        public static Player FindByName(string name)
        {
            for (int i = 0; i < Logic.players.Count; i++)
            {
                if (Logic.players[i].username == name)
                    return Logic.players[i];
            }
            return null;
        }
    }
}
