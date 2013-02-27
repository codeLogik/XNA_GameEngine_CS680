using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics.Colliders
{
    class SquareCollider : ICollider
    {
        protected Vector2 m_vRelativeOrigin;
        protected Vector2 m_vSize;

        public SquareCollider(GameObject parent, Vector2 relativeOrigin, Vector2 size) : base(parent)
        {
            m_vRelativeOrigin = relativeOrigin;
            m_vSize = size;
        }

        public Vector2 GetWorldOrigin()
        {
            return (GetParent().GetPosition() + m_vRelativeOrigin);
        }

        public override bool CollidesWith(SquareCollider other)
        {
            Vector2 myOrigin = GetWorldOrigin();
            Vector2 otherOrigin = other.GetWorldOrigin();
            if (myOrigin.X > otherOrigin.X + other.m_vSize.X ||
                myOrigin.X + m_vSize.X < otherOrigin.X ||
                myOrigin.Y > otherOrigin.Y + other.m_vSize.Y ||
                myOrigin.Y + m_vSize.Y < otherOrigin.Y)
            {
                return false;
            }
            return true;
        }

        public override bool CollidesWith(CircleCollider other)
        {
            return false;
        }
    }
}
