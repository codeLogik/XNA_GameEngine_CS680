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

        // Since lines technically have two normals, the user has to specify one.
        public LineCollider(Vector2 relativePointA, Vector2 relativePointB)
        {
            m_vRelativePointA = relativePointA;
            m_vRelativePointB = relativePointB;
        }

        public Vector2 GetWorldPointA()
        {
            return GetParent().GetParent().GetPosition() + m_vRelativePointA;
        }

        public Vector2 GetWorldPointB()
        {
            return GetParent().GetParent().GetPosition() + m_vRelativePointB;
        }

        public override float GetMomentOfInertia(float mass)
        {
            return mass * ((m_vRelativePointB - m_vRelativePointA).LengthSquared() + 1) / 12.0f;
        }

        public override Collision CollidesWith(LineCollider other)
        {
            Vector2 myPointA = GetWorldPointA();
            Vector2 myPointB = GetWorldPointB();
            Vector2 otherPointA = other.GetWorldPointA();
            Vector2 otherPointB = other.GetWorldPointB();
            
            // Trivial bounding box test
            Vector2 myTopLeft = new Vector2();
            Vector2 myBottomRight = new Vector2();
            Vector2 otherTopLeft = new Vector2();
            Vector2 otherBottomRight = new Vector2();
            if(myPointA.X > myPointB.X) {
                myTopLeft.X = myPointB.X;
                myBottomRight.X = myPointA.X;
            }
            if(myPointA.Y > myPointB.Y) {
                myTopLeft.Y = myPointB.Y;
                myBottomRight.Y = myPointA.Y;
            }
            if(otherPointA.X > otherPointB.X) {
                otherTopLeft.X = myPointB.X;
                otherBottomRight.X = otherPointA.X;
            }
            if(otherPointA.Y > otherPointB.Y) {
                otherTopLeft.Y = otherPointB.Y;
                otherBottomRight.Y = otherPointA.Y;
            }
            if(!base.BoundingBoxesOverlap(myTopLeft, myBottomRight, otherTopLeft, otherBottomRight)) {
                return null;
            }
           // XNA_GameEngine.Debug.DebugTools.Report("collision!");

            float denominator = (otherPointB.Y - otherPointA.Y) * (myPointB.X - myPointA.X) - (otherPointB.X - otherPointA.X) * (myPointB.Y - myPointA.Y);
            if (denominator < 0.0001f)
            {
                return null;
            }
            float distAlongThis = ((otherPointB.X - otherPointA.X) * (myPointA.Y - otherPointA.Y) - (otherPointB.Y - otherPointA.Y) * (myPointA.X - otherPointA.X)) / denominator;
            if (distAlongThis < 0.01f || distAlongThis > 0.99f)
            {
                return null;
            }
            float distAlongOther = ((myPointB.X - myPointA.X) * (myPointA.Y - otherPointA.Y) - (myPointB.Y - myPointA.Y) * (myPointA.X - otherPointA.X)) / denominator;
            if (distAlongOther < 0.01f || distAlongOther > 0.99f)
            {
                return null;
            }

          //  XNA_GameEngine.Debug.DebugTools.Report("collision reported!");

            Vector2 pointOfCollision = myPointA + (distAlongThis * (myPointB - myPointA));
            Collision collision = new Collision(new CollisionPoint(pointOfCollision, Vector2.Zero, this, other));
            return collision;
        }

        public override Collision CollidesWith(SquareCollider other)
        {
            Vector2 otherTopLeft = other.GetTopLeft();
            Vector2 otherBottomRight = other.GetBottomRight();

            // Trivial bounding box test
            Vector2 myTopLeft = new Vector2();
            Vector2 myBottomRight = new Vector2();
            if (GetWorldPointA().X > GetWorldPointB().X)
            {
                myTopLeft.X = GetWorldPointB().X;
                myBottomRight.X = GetWorldPointA().X;
            }
            else
            {
                myTopLeft.X = GetWorldPointA().X;
                myBottomRight.X = GetWorldPointB().X;
            }
            if (GetWorldPointA().Y > GetWorldPointB().Y)
            {
                myTopLeft.Y = GetWorldPointB().Y;
                myBottomRight.Y = GetWorldPointA().Y;
            }
            else
            {
                myTopLeft.Y = GetWorldPointA().Y;
                myBottomRight.Y = GetWorldPointB().Y;
            }

            // Trivial bounding box test
            if (!base.BoundingBoxesOverlap(myTopLeft, myBottomRight, otherTopLeft, otherBottomRight))
            {
                return null;
            }

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
                        Vector2 axisOfCollision = other.GetParent().GetParent().GetPosition() - location;
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

        public override Collision CollidesWith(CircleCollider other)
        {
            Vector2 otherWorldOrigin = other.GetOrigin();
            Vector2 myPointA = GetWorldPointA() - otherWorldOrigin;
            Vector2 myPointB = GetWorldPointB() - otherWorldOrigin;
           
            // Trivial bounding box test
            Vector2 myTopLeft = new Vector2();
            Vector2 myBottomRight = new Vector2();
            if (GetWorldPointA().X > GetWorldPointB().X)
            {
                myTopLeft.X = GetWorldPointB().X;
                myBottomRight.X = GetWorldPointA().X;
            }
            else
            {
                myTopLeft.X = GetWorldPointA().X;
                myBottomRight.X = GetWorldPointB().X;
            }
            if (GetWorldPointA().Y > GetWorldPointB().Y)
            {
                myTopLeft.Y = GetWorldPointB().Y;
                myBottomRight.Y = GetWorldPointA().Y;
            }
            else
            {
                myTopLeft.Y = GetWorldPointA().Y;
                myBottomRight.Y = GetWorldPointB().Y;
            }
            Vector2 otherTopLeft = new Vector2(otherWorldOrigin.X - other.GetRadius(), otherWorldOrigin.Y - other.GetRadius());
            Vector2 otherBottomRight = new Vector2(otherWorldOrigin.X + other.GetRadius(), otherWorldOrigin.Y + other.GetRadius());
            
            if (!base.BoundingBoxesOverlap(myTopLeft, myBottomRight, otherTopLeft, otherBottomRight))
            {
                return null;
            }

            float a = (myPointB.X - myPointA.X) * (myPointB.X - myPointA.X) + (myPointB.Y - myPointA.Y) * (myPointB.Y - myPointA.Y);
            float b = 2 * (myPointA.X * (myPointB.X - myPointA.X) + myPointA.Y * (myPointB.Y - myPointA.Y));
            float c = myPointA.X * myPointA.X + myPointA.Y * myPointA.Y - other.GetRadius() * other.GetRadius();

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
                Vector2 pointOfCollision = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                Vector2 axisOfCollision = other.GetOrigin() - pointOfCollision;
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
                        Vector2 pointOfCollision = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                        Vector2 axisOfCollision = other.GetOrigin() - pointOfCollision;
                        Collision collision = new Collision(new CollisionPoint(pointOfCollision, axisOfCollision, this, other));
                        return collision;
                    }
                }
                // First point is on line
                else
                {
                    Vector2 pointOfCollision1 = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                    Vector2 axisOfCollision = other.GetOrigin() - pointOfCollision1;
                    Collision collision = new Collision(new CollisionPoint(pointOfCollision1, axisOfCollision, this, other));
                    distAlongLine = (-b - sqrt) / (2 * a);
                    // Second point is not on line
                    if (distAlongLine < 0 || distAlongLine > 1)
                    {
                        return collision;
                    }
                    
                    Vector2 pointOfCollision2 = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                    Vector2 axisOfCollision2 = other.GetOrigin() - pointOfCollision2;
                    collision.AddCollisionPoint(new CollisionPoint(pointOfCollision2, axisOfCollision2, this, other));
                    return collision;
                }
            }
        }
    }
}
