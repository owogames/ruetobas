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
            RenderTarget2D target = new RenderTarget2D(game.GraphicsDevice, 420, 600);
            game.GraphicsDevice.SetRenderTarget(target);
            game.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            Logic.maskEffect.Parameters["sprite"].SetValue(Logic.tileDirt);
            Logic.maskEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(Logic.cardTexture[ID], new Rectangle(0, 0, 420, 600), Color.White);
            spriteBatch.End();
            game.GraphicsDevice.SetRenderTarget(null);
            return target;
        }
    }
}
