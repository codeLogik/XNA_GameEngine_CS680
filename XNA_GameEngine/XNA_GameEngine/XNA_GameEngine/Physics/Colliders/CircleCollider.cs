using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics.Colliders
{
    class CircleCollider : ICollider
    {
        private float m_fRadius;
        private Vector2 m_vRelativeCenter;

        public CircleCollider(GameObject gameObject, Vector2 relativeCenter, float radius) : base(gameObject)
        {
            m_vRelativeCenter = relativeCenter;
            m_fRadius = radius;
        }

        public Vector2 GetWorldOrigin()
        {
            return (GetParent().GetPosition() + m_vRelativeCenter);
        }

        public override bool CollidesWith(SquareCollider other)
        {
            return other.CollidesWith(this);
        }

        public override bool CollidesWith(CircleCollider other)
        {
            Vector2 myOrigin = GetWorldOrigin();
            Vector2 otherOrigin = other.GetWorldOrigin();

            float distance = (otherOrigin - myOrigin).Length();

            if (distance < m_fRadius + other.m_fRadius)
            {
                return true;
            }
            return false;
        }
    }
}
