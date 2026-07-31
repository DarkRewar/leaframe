using UnityEngine;
using UnityEngine.UIElements;

namespace Leaframe.Extensions
{
    public static class Painter2DExtensions
    {
        public static void LineRect(this Painter2D painter, Rect rect) => painter.LineRect(rect.min, rect.max);

        public static void LineRect(this Painter2D painter, Vector2 min, Vector2 max)
        {
            painter.MoveTo(min);
            painter.LineTo(new Vector2(min.x, max.y));
            painter.LineTo(max);
            painter.LineTo(new Vector2(max.x, min.y));
            painter.LineTo(min);
        }
    }
}