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
        protected float m_fMomentOfInertia;

        public CircleCollider(GameObject gameObject, float mass, float radius)
            : base(gameObject, mass)
        {
            m_fRadius = radius;
            m_fMomentOfInertia = m_fMass * m_fRadius * m_fRadius / 2.0f;
        }

        public Vector2 GetOrigin()
        {
            return GetParent().GetPosition();
        }

        public float GetRadius()
        {
            return m_fRadius;
        }

        public override float GetMomentOfInertia()
        {
            return m_fMomentOfInertia;
        }

        public override Collision CollidesWith(SquareCollider other)
        {
            return other.CollidesWith(this);
        }

        public override Collision CollidesWith(LineCollider other)
        {
            Vector2 myWorldOrigin = GetOrigin();
            Vector2 otherPointA = other.GetWorldPointA() - myWorldOrigin;
            Vector2 otherPointB = other.GetWorldPointB() - myWorldOrigin;

            // Trivial bounding box test
            Vector2 otherTopLeft = new Vector2();
            Vector2 otherBottomRight = new Vector2();
            if (other.GetWorldPointA().X > other.GetWorldPointA().X)
            {
                otherTopLeft.X = other.GetWorldPointA().X;
                otherBottomRight.X = other.GetWorldPointA().X;
            }
            if (other.GetWorldPointA().Y > other.GetWorldPointA().Y)
            {
                otherTopLeft.Y = other.GetWorldPointA().Y;
                otherBottomRight.Y = other.GetWorldPointA().Y;
            }
            Vector2 myTopLeft = new Vector2(myWorldOrigin.X - m_fRadius, myWorldOrigin.Y - m_fRadius);
            Vector2 myBottomRight = new Vector2(myWorldOrigin.X + m_fRadius, myWorldOrigin.Y + m_fRadius);

            if (!base.BoundingBoxesOverlap(myTopLeft, myBottomRight, otherTopLeft, otherBottomRight))
            {
                return null;
            }

            float a = (otherPointB.X - otherPointA.X) * (otherPointB.X - otherPointA.X) + (otherPointB.Y - otherPointA.Y) * (otherPointB.Y - otherPointA.Y);
            float b = 2 * (otherPointA.X * (otherPointB.X - otherPointA.X) + otherPointA.Y * (otherPointB.Y - otherPointA.Y));
            float c = otherPointA.X * otherPointA.X + otherPointA.Y * otherPointA.Y - m_fRadius * m_fRadius;

            float delta = (b * b - 4 * a * c);
            if (delta < 0)
            {
                return null;
            }
            // Basically zero
            else if (delta < 0.5f)
            {
                float distAlongLine = (-b) / (2 * a);
                if (distAlongLine < 0 || distAlongLine > 1)
                {
                    return null;
                }
                Vector2 pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
                Vector2 axisOfCollision = pointOfCollision - myWorldOrigin;
                Collision collision = new Collision(new CollisionPoint(pointOfCollision, this, other), axisOfCollision);
                return collision;
            }
            else
            {
                float sqrt = (float)Math.Sqrt(delta);
                float distAlongLine = (sqrt - b) / (2 * a);

                // First point is not on line
                if (distAlongLine < 0 || distAlongLine > 1)
                {
                    distAlongLine = (-b - sqrt) / (2 * a);
                    // Second point is not on line
                    if (distAlongLine < 0 || distAlongLine > 1)
                    {
                        return null;
                    }
                    // Second point is on line
                    else
                    {
                        Vector2 pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
                        Vector2 axisOfCollision = pointOfCollision - myWorldOrigin;
                        Collision collision = new Collision(new CollisionPoint(pointOfCollision, this, other), axisOfCollision);
                        return collision;
                    }
                }
                // First point is on line
                else
                {
                    // TODO this is broken because of axis of collision.
                    /*
                    Vector2 pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
                    Vector2 axisOfCollision = pointOfCollision - myWorldOrigin;
                    Collision collision = new Collision(new CollisionPoint(pointOfCollision, this, other), axisOfCollision);
                    distAlongLine = (-b - sqrt) / (2 * a);
                    // Second point is not on line
                    if (distAlongLine < 0 || distAlongLine > 1)
                    {
                        return collision;
                    }
                    pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
                    axisOfCollision = pointOfCollision - myWorldOrigin;
                    collision.AddCollisionPoint(new CollisionPoint(pointOfCollision, this, other));
                    return collision;
                     * */
                    return null;
                }
            }
        }

        public override Collision CollidesWith(CircleCollider other)
        {
            Vector2 myOrigin = GetOrigin();
            Vector2 otherOrigin = other.GetOrigin();

            float distance = (otherOrigin - myOrigin).Length();

            if (distance < (m_fRadius + other.m_fRadius))
            {
                // One circle is completely inside the other.
                if (distance <= Math.Abs(m_fRadius - other.m_fRadius))
                {
                    return null;
                }
                float a = ((m_fRadius * m_fRadius) - (other.m_fRadius * other.m_fRadius) + (distance * distance)) / (2.0f * distance);
                // basically zero; one point of intersection
                if (Math.Abs(a - m_fRadius) < 0.5)
                {
                    Vector2 pointOfCollision = myOrigin + (otherOrigin - myOrigin) * (m_fRadius / other.m_fRadius);
                    Vector2 axisOfCollision = (otherOrigin - myOrigin);
                    axisOfCollision.Normalize();
                    Collision collision = new Collision(new CollisionPoint(pointOfCollision, this, other), axisOfCollision);
                    return collision;
                }
                float h = (float)Math.Sqrt((m_fRadius * m_fRadius) - (a * a));

                Vector2 middlePoint = myOrigin + (a * (myOrigin - otherOrigin) / distance);
                Vector2 pointOfCollision1 = new Vector2(middlePoint.X + h * (otherOrigin.X - myOrigin.X) / distance, middlePoint.Y + h * (otherOrigin.Y - myOrigin.Y) / distance);
                Vector2 pointOfCollision2 = new Vector2(middlePoint.X - h * (otherOrigin.X - myOrigin.X) / distance, middlePoint.Y - h * (otherOrigin.Y - myOrigin.Y) / distance);
                Vector2 myAxisOfCollision = (otherOrigin - myOrigin);
                myAxisOfCollision.Normalize();
                Collision myCollision = new Collision(new CollisionPoint(pointOfCollision1, this, other), myAxisOfCollision);
                myCollision.AddCollisionPoint(new CollisionPoint(pointOfCollision2, this, other));
                return myCollision;
            }
            return null;
        }
    }
}
