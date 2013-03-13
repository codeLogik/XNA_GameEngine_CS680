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
        private Vector2 m_vOrigin;
        private float m_fRadius;

        public CircleCollider(Vector2 origin, float radius)
        {
            m_vOrigin = origin;
            m_fRadius = radius;
        }

        public Vector2 GetOrigin()
        {
            return GetParent().GetParent().GetPosition();
        }

        public float GetRadius()
        {
            return m_fRadius;
        }

        public override float GetMomentOfInertia(float mass)
        {
            return mass * m_fRadius * m_fRadius / 2.0f;
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

        public override Collision CollidesWith(LineCollider other)
        {
            BoundingBox2D myBox = GetBoundingBox();
            BoundingBox2D otherBox = other.GetBoundingBox();

            // Trivial bounding box test
            if (!myBox.Overlap(otherBox))
            {
                return null;
            }

            Vector2 myWorldOrigin = GetOrigin();
            Vector2 otherPointA = other.TransformToWorld(other.GetLocalA()) - myWorldOrigin;
            Vector2 otherPointB = other.TransformToWorld(other.GetLocalB()) - myWorldOrigin;

            float a = (otherPointB.X - otherPointA.X) * (otherPointB.X - otherPointA.X) + (otherPointB.Y - otherPointA.Y) * (otherPointB.Y - otherPointA.Y);
            float b = 2 * (otherPointA.X * (otherPointB.X - otherPointA.X) + otherPointA.Y * (otherPointB.Y - otherPointA.Y));
            float c = otherPointA.X * otherPointA.X + otherPointA.Y * otherPointA.Y - m_fRadius * m_fRadius;

            float delta = (b * b - 4 * a * c);
            if (delta < 0)
            {
                return null;
            }
            // Basically zero
            else if (delta < 1.0f)
            {
                float distAlongLine = (-b) / (2 * a);
                if (distAlongLine < 0 || distAlongLine > 1)
                {
                    return null;
                }
                Vector2 pointOfCollision = other.TransformToWorld(other.GetLocalA()) + (distAlongLine * (other.TransformToWorld(other.GetLocalB()) - other.TransformToWorld(other.GetLocalA())));
                Vector2 axisOfCollision = pointOfCollision - myWorldOrigin;
                Collision collision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision, this, other));
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
                        Vector2 pointOfCollision = other.TransformToWorld(other.GetLocalA()) + (distAlongLine * (other.TransformToWorld(other.GetLocalB()) - other.TransformToWorld(other.GetLocalA())));
                        Vector2 axisOfCollision = pointOfCollision - myWorldOrigin;
                        Collision collision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision, this, other));
                        return collision;
                    }
                }
                // First point is on line
                else
                {
                    Vector2 pointOfCollision = other.TransformToWorld(other.GetLocalA()) + (distAlongLine * (other.TransformToWorld(other.GetLocalB()) - other.TransformToWorld(other.GetLocalA())));
                    Vector2 axisOfCollision = pointOfCollision - myWorldOrigin;
                    distAlongLine = (-b - sqrt) / (2 * a);
                    // Second point is not on line
                    if (distAlongLine < 0 || distAlongLine > 1)
                    {
                        Collision collision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision, this, other));
                        return collision;
                    }
                    pointOfCollision = (pointOfCollision + other.TransformToWorld(other.GetLocalA()) + (distAlongLine * (other.TransformToWorld(other.GetLocalB()) - other.TransformToWorld(other.GetLocalA())))) / 2.0f;
                    axisOfCollision = pointOfCollision - myWorldOrigin;
                    Collision averageCollision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision, this, other));
                    return averageCollision;
                }
            }
        }

        public override Collision CollidesWith(CircleCollider other)
        {
            Vector2 myOrigin = GetOrigin();
            Vector2 otherOrigin = other.GetOrigin();

            Vector2 directionOfCollision = otherOrigin - myOrigin;

            // Faster than bounding box test
            float distance = directionOfCollision.Length();
            if (distance < (m_fRadius + other.m_fRadius))
            {
                // One circle is completely inside the other.
                if (distance <= Math.Abs(m_fRadius - other.m_fRadius))
                {
                    return null;
                }

                directionOfCollision.Normalize();
                Vector2 pointOfCollision = myOrigin + (directionOfCollision * m_fRadius);
                Collision collision = new Collision(new CollisionPoint(pointOfCollision, directionOfCollision, this, other));
                return collision;
            }
            return null;
        }

        public override BoundingBox2D GetBoundingBox()
        {
            Vector2 origin = GetParent().GetParent().GetPosition();
            Vector2 topLeft = new Vector2(origin.X - m_fRadius, origin.Y - m_fRadius);
            Vector2 bottomRight = new Vector2(origin.X + m_fRadius, origin.Y + m_fRadius);
            return new BoundingBox2D(topLeft, bottomRight);
        }
    }
}
