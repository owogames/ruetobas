using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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
