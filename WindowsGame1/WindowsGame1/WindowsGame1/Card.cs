using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ruetobas
{
    public enum CardType { Empty, Tunnel, Buff, Debuff, Remove, Map }
    public enum TunnelObject { None, Ladder, Rock, Gem, Gold}

    public class Card
    {
        public Texture2D texture;
        public int ID;
        public CardType cardType;

        public static Card EmptyCard()
        {
            Card card = new Card();
            card.texture = Logic.cardTexture[0];
            card.ID = 0;
            card.cardType = CardType.Empty;
            return card;
        }

        public static Card ParseString(string line, int ID) //linijka z pliku oraz numer linijki (czyli numer karty)
        {
            string[] words = line.Split(' ');
            int size = words.Length;
            if (words[0] == "T")
            {
                TunnelObject objekt = (TunnelObject)int.Parse(words[size - 1]);
                Tunnel output = new Tunnel(Logic.cardTexture[ID], ID, objekt);
                for (int i = 0; i < 4; i++)
                {
                    output.entrance[i] = Convert.ToBoolean(int.Parse(words[i + 1]));
                }
                for (int i = 0; i < 16; i++)
                {
                    output.graph[i / 4, i % 4] = Convert.ToBoolean(int.Parse(words[i + 5]));
                }
                return output;
            }
            else if (words[0] == "B")
            {
                BuffCard output = new BuffCard(Logic.cardTexture[ID], ID, (Buff)int.Parse(words[1]));
                return output;
            }
            else if (words[0] == "D")
            {
                DebuffCard output = new DebuffCard(Logic.cardTexture[ID], ID, (Buff)int.Parse(words[1]), (Buff)int.Parse(words[2]));
                return output;
            }
            else if (words[0] == "M")
            {
                MapCard output = new MapCard(Logic.cardTexture[ID], ID);
                return output;
            }
            else if (words[0] == "C")
            {
                RemoveCard output = new RemoveCard(Logic.cardTexture[ID], ID);
                return output;
            }
            return null;
        }

        public static bool[,,] reach = new bool[19, 15, 4];
        
        public static void RecalculateReach()
        {
            Queue<Tuple<int, int, int>> q = new Queue<Tuple<int, int, int>>();
            q.Enqueue(new Tuple<int, int, int>(5, 7, 0));
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 15; j++)
                    for (int ij = 0; ij < 4; ij++)
                        reach[i, j, ij] = false;
            reach[5, 7, 0] = true;

            while (q.Count > 0)
            {
                Tuple<int, int, int> T = q.Dequeue();
                int x = T.Item1;
                int y = T.Item2;
                int e = T.Item3;
                if (x <= 0) continue;
                if (x >= 18) continue;
                if (y <= 0) continue;
                if (y >= 14) continue;
                Tunnel t1 = (Tunnel)Logic.cards[Logic.map[x, y].ID];
                bool r1 = (Logic.map[x, y].rotation == 1);
                for (int i = 0; i < 4; i++)
                    if (t1.IsConnected(i, e, r1) && !reach[x, y, i])
                    {
                        reach[x, y, i] = true;
                        q.Enqueue(new Tuple<int, int, int>(x, y, i));
                    }

                int xn = e == 0 ? x : (e == 1 ? x + 1 : (e == 2 ? x : x - 1));
                int yn = e == 0 ? y - 1 : (e == 2 ? y : (e == 3 ? y + 1 : y));
                PlacedCard neighbor = Logic.map[xn, yn];
                Tunnel t2 = (Tunnel)Logic.cards[neighbor.ID];
                bool r2 = (neighbor.rotation == 1);
                if (t1.GetEntrance(e, r1) && t2.GetEntrance(e + 2, r2) && !reach[xn, yn, (e + 2) % 2])
                {
                    reach[xn, yn, (e + 2) % 2] = true;
                    q.Enqueue(new Tuple<int, int, int>(xn, yn, (e + 2) % 2));
                }
            }
        }

        public static int CheckPlacement(int x, int y, int ID, int rot)
        {
            if (Logic.cards[ID].cardType != CardType.Tunnel)
                return 4;
            if (x < 1 || x > 17 || y < 1 || y > 13)
                return -1;
            if (Logic.cards[Logic.map[x, y].ID].cardType != CardType.Empty)
                return 3;

            Point[] placements = { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
            bool any_valid_card = false;
            Tunnel center = (Tunnel)Logic.cards[ID];

            for (int i = 0; i < 4; i++)
            {
                PlacedCard current = Logic.map[x + placements[i].X, y + placements[i].Y];
                if (current.ID == 45 || current.ID == 0)
                    continue;
                else
                {

                    Tunnel currentTL = (Tunnel)Logic.cards[current.ID];
                    if (center.GetEntrance(i + rot * 2) != currentTL.GetEntrance(i + (current.rotation - 1) * 2))
                    {
                        return 2;
                    }
                    else if (center.GetEntrance(i + rot * 2) && reach[x + placements[i].X, y + placements[i].Y, (i + 2) % 2])
                    {
                        any_valid_card = true;
                    }
                }
            }
            //    0       0    
            //  3 C 1 - 3 T 1  //no rotation
            //    2       2    

            if (any_valid_card == false)
                return 1;
            return 0;
            //0 - OK
            //1 - karta musi przylegać do innej karty (pamiętać, żeby nie brać karty 45 pod uwagę)
            //2 - tunele wychodzące z karty muszą pasować do sąsiednich kart
            //3 - karta musi być położona na pustym polu
            //4 - karta musi tunelem
        }
    }

    public class Tunnel : Card
    {
        public bool[] entrance; //Tablica booli czy istnieje wyjście na danym brzegu karty
        public bool[,] graph; //Graf
        public TunnelObject tunnelObject;
        public Tunnel(Texture2D texture, int ID, TunnelObject tunnelObject)
        {
            this.texture = texture;
            this.ID = ID;
            this.tunnelObject = tunnelObject;
            cardType = CardType.Tunnel;
            entrance = new bool[4];
            graph = new bool[4, 4];
        }

        public bool GetEntrance(int entrance_number)
        {
            entrance_number = (entrance_number % 4) + 4; //dotatnie modulo
            return entrance[entrance_number % 4];
        }

        public bool GetEntrance(int enterance_number, bool rotation)
        {
            return GetEntrance(enterance_number + (rotation ? 2 : 0));
        }

        public bool IsConnected(int x, int y, bool rot)
        {
            if (rot)
            {
                x += 2;
                y += 2;
            }
            return graph[x % 4, y % 4];
        }
    }

    public class BuffCard : Card
    {
        public Buff buffType;
        public BuffCard(Texture2D texture, int ID, Buff buffType)
        {
            this.texture = texture;
            this.ID = ID;
            this.buffType = buffType;
            cardType = CardType.Buff;
        }
    }

    public class DebuffCard : Card
    {
        public Buff buffType;
        public Buff buffType2;

        public DebuffCard(Texture2D texture, int ID, Buff buffType, Buff buffType2)
        {
            this.texture = texture;
            this.ID = ID;
            this.buffType = buffType;
            this.buffType2 = buffType2;
            cardType = CardType.Debuff;
        }
    }

    public class RemoveCard : Card
    {
        public RemoveCard(Texture2D texture, int ID)
        {
            this.texture = texture;
            this.ID = ID;
            cardType = CardType.Remove;
        }
    }

    public class MapCard : Card
    {
        public MapCard(Texture2D texture, int ID)
        {
            this.texture = texture;
            this.ID = ID;
            cardType = CardType.Map;
        }
    }


    public class PlacedCard
    {
        public int ID;
        public int rotation;
        public PlacedCard(int ID, int rotation)
        {
            this.ID = ID;
            this.rotation = rotation;
        }
    }
}
