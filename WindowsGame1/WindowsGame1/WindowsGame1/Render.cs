using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ruetobas
{
    public static class Render
    {
        public const bool DEBUGMODE = false;

        public static Texture2D RenderTunnel(Game game, SpriteBatch spriteBatch, int ID)
        {
            Console.WriteLine("Rendering texture ID: {0}", ID);
            Texture2D target = new Texture2D(game.GraphicsDevice, 420, 600);
            Color[] maskData = new Color[420 * 600];
            Color[] dirtData = new Color[420 * 600];
            Color[] output = new Color[420 * 600];
            Logic.cardTexture[ID].GetData(maskData);
            Logic.tileDirt.GetData(dirtData);
            for (int x = 0; x < 420; x++)
            {
                for (int y = 0; y < 600; y++)
                {
                    if (maskData[x + 420 * y].B < 20)
                    {
                        output[x + 420 * y] = dirtData[x + 420 * y];
                        if (!DEBUGMODE)
                        {
                            double amount = 0;
                            for (int x1 = Math.Max(0, x - 12); x1 < Math.Min(420, x + 13); x1++)
                                for (int y1 = Math.Max(0, y - 12); y1 < Math.Min(600, y + 13); y1++)
                                    if (maskData[x1 + 420 * y1].B > 20)
                                        amount += 0.25f;
                            output[x + 420 * y].R -= Math.Min((byte)amount, output[x + 420 * y].R);
                            output[x + 420 * y].G -= Math.Min((byte)amount, output[x + 420 * y].G);
                            output[x + 420 * y].B -= Math.Min((byte)amount, output[x + 420 * y].B);
                        }
                    }
                    else
                        output[x + 420 * y] = Color.Transparent;
                }
            }

            target.SetData(output);
            return target;
        }
    }
}
