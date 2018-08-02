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
            Console.WriteLine("Rendering texture ID: {0}", ID);
            Texture2D target = new Texture2D(game.GraphicsDevice, 420, 600);
            Color[] maskData = new Color[420 * 600];
            Color[] dirtData = new Color[420 * 600];
            Color[] output = new Color[420 * 600];
            float[,] prefixSums = new float[421, 601];
            if (ID > 0)
                Logic.cardTexture[ID].GetData(maskData);
            else
            {
                for (int i = 0; i < 420 * 600; i++)
                    maskData[i] = Color.White;
            }
            Logic.tileDirt.GetData(dirtData);
            for (int x = 0; x < 420; x++)
                for (int y = 0; y < 600; y++)
                {
                    prefixSums[x + 1, y + 1] = prefixSums[x, y + 1] + prefixSums[x + 1, y] - prefixSums[x, y];
                    if (maskData[x + 420 * y].B > 20)
                        prefixSums[x + 1, y + 1] += 0.25f;
                }

            for (int x = 0; x < 420; x++)
            {
                for (int y = 0; y < 600; y++)
                {
                    if (maskData[x + 420 * y].B < 20)
                    {
                        output[x + 420 * y] = dirtData[x + 420 * y];
                        double amount = prefixSums[Math.Min(420, x + 13), Math.Min(600, y + 13)] - prefixSums[Math.Max(0, x - 12), Math.Min(600, y + 13)] - prefixSums[Math.Min(420, x + 13), Math.Max(0, y - 12)] + prefixSums[Math.Max(0, x - 12), Math.Max(0, y - 12)];
                        output[x + 420 * y].R -= Math.Min((byte)amount, output[x + 420 * y].R);
                        output[x + 420 * y].G -= Math.Min((byte)amount, output[x + 420 * y].G);
                        output[x + 420 * y].B -= Math.Min((byte)amount, output[x + 420 * y].B);
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
