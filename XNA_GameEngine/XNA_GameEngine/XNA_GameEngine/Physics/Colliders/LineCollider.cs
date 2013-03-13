using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics.Colliders
{
    class LineCollider : ICollider
    {
        private Vector2 m_vRelativePointA;
        private Vector2 m_vRelativePointB;

        public LineCollider(Vector2 relativePointA, Vector2 relativePointB)
        {
            m_vRelativePointA = relativePointA;
            m_vRelativePointB = relativePointB;
        }

        public Vector2 GetLocalA()
        {
            return m_vRelativePointA;
        }

        public Vector2 GetLocalB()
        {
            return m_vRelativePointB;
        }

        public override float GetMomentOfInertia(float mass)
        {
            return mass * ((m_vRelativePointB - m_vRelativePointA).LengthSquared() + 1) / 12.0f;
        }

        public override Collision CollidesWith(LineCollider other)
        {
            BoundingBox2D myBox = GetBoundingBox();
            BoundingBox2D otherBox = other.GetBoundingBox();
            // Trivial bounding box test
            if (!myBox.Overlap(otherBox))
            {
                return null;
            }

            Vector2 myPointA = base.TransformToWorld(m_vRelativePointA);
            Vector2 myPointB = base.TransformToWorld(m_vRelativePointB);
            Vector2 otherPointA = other.TransformToWorld(other.m_vRelativePointA);
            Vector2 otherPointB = other.TransformToWorld(other.m_vRelativePointB);

            float denominator = (otherPointB.Y - otherPointA.Y) * (myPointB.X - myPointA.X) - (otherPointB.X - otherPointA.X) * (myPointB.Y - myPointA.Y);
            if (denominator == 0)
            {
                return null;
            }
            float distAlongThis = ((otherPointB.X - otherPointA.X) * (myPointA.Y - otherPointA.Y) - (otherPointB.Y - otherPointA.Y) * (myPointA.X - otherPointA.X)) / denominator;
            if (distAlongThis < 0.0f || distAlongThis > 1.0f)
            {
                return null;
            }
            float distAlongOther = ((myPointB.X - myPointA.X) * (myPointA.Y - otherPointA.Y) - (myPointB.Y - myPointA.Y) * (myPointA.X - otherPointA.X)) / denominator;
            if (distAlongOther < 0.0f || distAlongOther > 1.0f)
            {
                return null;
            }

            Vector2 pointOfCollision = myPointA + (distAlongThis * (myPointB - myPointA));
            Collision collision = new Collision(new CollisionPoint(pointOfCollision, Vector2.Zero, this, other));
            return collision;
        }

        public override Collision CollidesWith(SquareCollider other)
        {
            Collision collision = other.CollidesWith(this);
            if (collision != null)
            {
                collision.ReverseCollision();
            }
            return collision;
        }

        public override Collision CollidesWith(CircleCollider other)
        {
            Collision collision = other.CollidesWith(this);
            if (collision != null)
            {
                collision.ReverseCollision();
            }
            return collision;
        }

        public override BoundingBox2D GetBoundingBox()
        {
            Vector2 vertexA = base.TransformToWorld(m_vRelativePointA);
            Vector2 vertexB = base.TransformToWorld(m_vRelativePointB);
            Vector2 topLeft = Vector2.Min(vertexA, vertexB);
            Vector2 bottomRight = Vector2.Max(vertexA, vertexB);

            return new BoundingBox2D(topLeft, bottomRight);
        }
    }
}
