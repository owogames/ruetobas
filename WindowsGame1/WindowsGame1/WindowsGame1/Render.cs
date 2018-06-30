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
        public static Texture2D RenderTunnel(Game game, SpriteBatch spriteBatch, int ID)
        {
            Texture2D target = new Texture2D(game.GraphicsDevice, 210, 300);
            Color[] maskData = new Color[210 * 300];
            Color[] dirtData = new Color[420 * 600];
            Color[] output = new Color[210 * 300];
            Logic.cardTexture[ID].GetData(maskData);
            Logic.tileDirt.GetData(dirtData);
            for (int x = 0; x < 210; x++)
            {
                for (int y = 0; y < 300; y++)
                {
                    if (maskData[x + 210 * y].B < 20)
                    {
                        output[x + 210 * y] = dirtData[2 * (x + 420 * y)];
                        double amount = 0;
                        for (int x1 = Math.Max(0, x - 12); x1 < Math.Min(210, x + 13); x1++)
                            for (int y1 = Math.Max(0, y - 12); y1 < Math.Min(300, y + 13); y1++)
                                if (maskData[x1 + 210 * y1].B > 20)
                                    amount += 0.25f;
                        output[x + 210 * y].R -= Math.Min((byte)amount, output[x + 210 * y].R);
                        output[x + 210 * y].G -= Math.Min((byte)amount, output[x + 210 * y].G);
                        output[x + 210 * y].B -= Math.Min((byte)amount, output[x + 210 * y].B);
                    }
                    else
                        output[x + 210 * y] = Color.Transparent;
                }
            }

            target.SetData(output);
            return target;
        }
    }
}
