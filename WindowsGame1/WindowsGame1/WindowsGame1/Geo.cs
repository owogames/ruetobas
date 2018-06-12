using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ruetobas
{
    public static class Geo
    {
        public static bool RectContains(Rectangle rect, Vector2 point)
        {
            return point.X >= rect.X && point.Y >= rect.Y && point.X <= rect.X + rect.Width && point.Y <= rect.Y + rect.Height;
        }

        public static Rectangle Shrink(Rectangle rect, int amount)
        {
            return new Rectangle(rect.X + amount, rect.Y + amount, rect.Width - 2 * amount, rect.Height - 2 * amount);
        }
    }
}
