using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Physics
{
    class BoundingBox2D
    {
        protected Vector2 m_vTopLeft;
        protected Vector2 m_vBottomRight;

        public BoundingBox2D(Vector2 topLeft, Vector2 bottomRight)
        {
            m_vTopLeft = topLeft;
            m_vBottomRight = bottomRight;
        }

        public BoundingBox2D Combine(BoundingBox2D other)
        {
            return new BoundingBox2D(Vector2.Min(m_vTopLeft, other.m_vTopLeft), Vector2.Max(m_vBottomRight, other.m_vBottomRight));
        }

        public Boolean Overlap(BoundingBox2D other)
        {
            if (m_vTopLeft.X > other.m_vBottomRight.X ||
                m_vBottomRight.X < other.m_vTopLeft.X ||
                m_vTopLeft.Y > other.m_vBottomRight.Y ||
                m_vBottomRight.Y < other.m_vTopLeft.Y)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "BoundingBox2D [ " + m_vTopLeft + " , " + m_vBottomRight + " ]";
        }

    }
}
