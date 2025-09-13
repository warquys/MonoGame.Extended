using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MonoGame.Extended.Triangulation
{
    /// <summary>
    /// MIT Licensed: https://github.com/nickgravelyn/Triangulator
    /// </summary>
    struct VertexLineSegment
    {
        public Vertex Start;
        public Vertex End;

        public VertexLineSegment(Vertex start, Vertex end)
        {
            Start = start;
            End = end;
        }

        public float? IntersectsWithRay(Vector2 origin, Vector2 direction)
        {
            float largestDistance = MathHelper.Max(Start.Position.X - origin.X, End.Position.X - origin.X) * 2f;
            VertexLineSegment raySegment = new VertexLineSegment(new Vertex(origin, 0), new Vertex(origin + (direction * largestDistance), 0));

            Vector2? intersection = FindIntersection(this, raySegment);
            float? value = null;

            if (intersection != null)
                value = Vector2.Distance(origin, intersection.Value);

            return value;
        }

        public static Vector2? FindIntersection(VertexLineSegment a, VertexLineSegment b)
        {
            float x1 = a.Start.Position.X;
            float y1 = a.Start.Position.Y;
            float x2 = a.End.Position.X;
            float y2 = a.End.Position.Y;
            float x3 = b.Start.Position.X;
            float y3 = b.Start.Position.Y;
            float x4 = b.End.Position.X;
            float y4 = b.End.Position.Y;

            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

            if (denom == 0) return null;

            float uaNum = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
            float ubNum = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);

            float ua = uaNum / denom;
            float ub = ubNum / denom;

            if (MathHelper.Clamp(ua, 0f, 1f) != ua || MathHelper.Clamp(ub, 0f, 1f) != ub)
                return null;

            return a.Start.Position + (a.End.Position - a.Start.Position) * ua;
        }

        public static implicit operator Segment2(VertexLineSegment v) => new Segment2(v.Start.Position, v.End.Position);
    }
}
