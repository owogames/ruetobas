using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ruetobas
{
    public enum CardType { Tunnel }
    public enum TunnelObject { None, Ladder, Rock, Gold, Gem}

    public class Card
    {
        public Texture2D texture;
        public int ID;
        public CardType cardType;

        public static Card ParseString(string line)
        {
            Tunnel output = new Tunnel();

            //Elo Robert zakodź

            return output;
        }
    }

    public class Tunnel : Card
    {
        public bool[] enterance; //Tablica booli czy istnieje wyjście na danym brzegu karty
        public bool[,] graph; //Graf
        public TunnelObject tunnelObject;
        public Tunnel(Texture2D texture, int ID, TunnelObject tunnelObject)
        {
            this.texture = texture;
            this.ID = ID;
            this.tunnelObject = tunnelObject;
            cardType = CardType.Tunnel;
            enterance = new bool[4];
            graph = new bool[4, 4];
        }
    }


    public class PlacedCard
    {
        int ID;
        int rotation;
        public PlacedCard(int ID, int rotation)
        {
            this.ID = ID;
            this.rotation = rotation;
        }
    }
}
