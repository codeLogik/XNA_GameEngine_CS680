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
            LineCollider[] otherSides = new LineCollider[4];

            otherSides[0] = new LineCollider(new Vector2(-(other.GetSize().X / 2.0f), -(other.GetSize().Y / 2.0f)), new Vector2((other.GetSize().X / 2.0f), -(other.GetSize().Y / 2.0f)));
            otherSides[0].SetParent(other.GetParent());
            otherSides[1] = new LineCollider(new Vector2(-(other.GetSize().X / 2.0f), (other.GetSize().Y / 2.0f)), new Vector2((other.GetSize().X / 2.0f), (other.GetSize().Y / 2.0f)));
            otherSides[1].SetParent(other.GetParent());
            otherSides[2] = new LineCollider(new Vector2(-(other.GetSize().X / 2.0f), -(other.GetSize().Y / 2.0f)), new Vector2(-(other.GetSize().X / 2.0f), (other.GetSize().Y / 2.0f)));
            otherSides[2].SetParent(other.GetParent());
            otherSides[3] = new LineCollider(new Vector2((other.GetSize().X / 2.0f), -(other.GetSize().Y / 2.0f)), new Vector2((other.GetSize().X / 2.0f), (other.GetSize().Y / 2.0f)));
            otherSides[3].SetParent(other.GetParent());

            Collision collision = new Collision();
            for (int i = 0; i < 4; i++)
            {
                Collision thisCollision = CollidesWith(otherSides[i]);
                if (thisCollision != null)
                {
                    foreach (CollisionPoint point in thisCollision.GetPoints())
                    {
                        Vector2 location = point.WorldLocation;
                        Vector2 axisOfCollision = location - GetParent().GetParent().GetPosition();
                        collision.AddCollisionPoint(new CollisionPoint(location, axisOfCollision, this, other));
                    }
                }
            }
            if (collision.GetPoints().Count == 0)
            {
                return null;
            }

            return collision;
        }

        public override Collision CollidesWith(LineCollider other)
        {
            Vector2 myWorldOrigin = GetOrigin();
            Vector2 otherPointA = other.GetWorldPointA() - myWorldOrigin;
            Vector2 otherPointB = other.GetWorldPointB() - myWorldOrigin;

            
            // Trivial bounding box test
            Vector2 otherTopLeft = new Vector2();
            Vector2 otherBottomRight = new Vector2();
            if (other.GetWorldPointA().X > other.GetWorldPointB().X)
            {
                otherTopLeft.X = other.GetWorldPointB().X;
                otherBottomRight.X = other.GetWorldPointA().X;
            }
            else
            {
                otherTopLeft.X = other.GetWorldPointA().X;
                otherBottomRight.X = other.GetWorldPointB().X;
            }
            if (other.GetWorldPointA().Y > other.GetWorldPointB().Y)
            {
                otherTopLeft.Y = other.GetWorldPointB().Y;
                otherBottomRight.Y = other.GetWorldPointA().Y;
            }
            else
            {
                otherTopLeft.Y = other.GetWorldPointA().Y;
                otherBottomRight.Y = other.GetWorldPointB().Y;
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
            else if (delta < 1.0f)
            {
                float distAlongLine = (-b) / (2 * a);
                if (distAlongLine < 0 || distAlongLine > 1)
                {
                    return null;
                }
                Vector2 pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
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
                        Vector2 pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
                        Vector2 axisOfCollision = pointOfCollision - myWorldOrigin;
                        Collision collision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision, this, other));
                        return collision;
                    }
                }
                // First point is on line
                else
                {
                    Vector2 pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
                    Vector2 axisOfCollision1 = pointOfCollision - myWorldOrigin;
                    Collision collision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision1, this, other));
                    distAlongLine = (-b - sqrt) / (2 * a);
                    // Second point is not on line
                    if (distAlongLine < 0 || distAlongLine > 1)
                    {
                        return collision;
                    }
                    pointOfCollision = other.GetWorldPointA() + (distAlongLine * (other.GetWorldPointB() - other.GetWorldPointA()));
                    Vector2 axisOfCollision2 = pointOfCollision - myWorldOrigin;
                    collision.AddCollisionPoint(new CollisionPoint(pointOfCollision, axisOfCollision2, this, other));
                    /*
                    Vector2 unitToAverage = axisOfCollision1 + axisOfCollision2;
                    unitToAverage.Normalize();
                    Vector2 collisionProjection = Vector2.Dot(axisOfCollision2, unitToAverage) * unitToAverage;
                    Vector2 resolveOverlap = 
                    */

                    return collision;
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
                    Collision collision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision, this, other));
                    return collision;
                }
                float h = (float)Math.Sqrt((m_fRadius * m_fRadius) - (a * a));

                Vector2 middlePoint = myOrigin + (a * (myOrigin - otherOrigin) / distance);
                Vector2 pointOfCollision1 = new Vector2(middlePoint.X + h * (otherOrigin.X - myOrigin.X) / distance, middlePoint.Y + h * (otherOrigin.Y - myOrigin.Y) / distance);
                Vector2 pointOfCollision2 = new Vector2(middlePoint.X - h * (otherOrigin.X - myOrigin.X) / distance, middlePoint.Y - h * (otherOrigin.Y - myOrigin.Y) / distance);
                Vector2 myAxisOfCollision1 = (pointOfCollision1 - myOrigin);
                Vector2 myAxisOfCollision2 = (pointOfCollision2 - myOrigin);
                myAxisOfCollision1.Normalize();
                myAxisOfCollision2.Normalize();
                Collision myCollision = new Collision(new CollisionPoint(pointOfCollision1, myAxisOfCollision1, this, other));
                myCollision.AddCollisionPoint(new CollisionPoint(pointOfCollision2, myAxisOfCollision2, this, other));
                return myCollision;
            }
            return null;
        }
    }
}
