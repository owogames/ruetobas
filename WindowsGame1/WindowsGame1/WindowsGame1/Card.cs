using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ruetobas
{
    public enum CardType { Tunnel }
    public enum TunnelObject { None, Ladder, Rock, Gem, Gold}

    public class Card
    {
        public Texture2D texture;
        public int ID;
        public CardType cardType;

        public static Card ParseString(string line, int ID) //linijka z pliku oraz numer linijki (czyli numer karty)
        {
            string[] words = line.Split();
            int size = words.Length;
            if(words[0] == "T")
            {
                TunnelObject objekt = (TunnelObject)int.Parse(words[size - 1]);
                Tunnel output = new Tunnel(Logic.skurwielTexture, ID, objekt);
                for(int i = 0; i < 4; i++)
                {
                    output.entrance[i] = bool.Parse(words[i + 1]);
                }
                for(int i = 0; i < 16; i++)
                {
                    output.graph[i / 4, i % 4] = bool.Parse(words[i + 5]);
                }
                return output;
            }
            //Elo Robert zakodź
            //Uwaga, przyjmowana tekstura to zoltyskurwiel
            //UWAGA Nie testowane ;)
            //Prawdopodobnie parsowanie bool.Parse może nie wyjść po konwertuję "0" na false a nie "false" na false
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
