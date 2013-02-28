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
        protected float m_fMomentOfInertia;

        public SquareCollider(GameObject parent, float mass, Vector2 relativeOrigin, Vector2 size) 
            : base(parent, mass)
        {
            m_vRelativeOrigin = relativeOrigin;
            m_vSize = size;
            m_fMomentOfInertia = m_fMass * (m_vSize.Y * m_vSize.Y + m_vSize.X + m_vSize.X) / 12.0f;
        }

        public Vector2 GetWorldOrigin()
        {
            return (GetParent().GetPosition() + m_vRelativeOrigin);
        }

        public override float GetMomentOfInertia()
        {
            return m_fMomentOfInertia;
        }

        public override Collision CollidesWith(SquareCollider other)
        {
            Vector2 myOrigin = GetWorldOrigin();
            Vector2 otherOrigin = other.GetWorldOrigin();
            if (myOrigin.X > otherOrigin.X + other.m_vSize.X ||
                myOrigin.X + m_vSize.X < otherOrigin.X ||
                myOrigin.Y > otherOrigin.Y + other.m_vSize.Y ||
                myOrigin.Y + m_vSize.Y < otherOrigin.Y)
            {
                return null;
            }
            return null;
        }

        public override Collision CollidesWith(LineCollider other)
        {
            return null;
        }

        public override Collision CollidesWith(CircleCollider other)
        {
            return null;
        }
    }
}
