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
        public LineCollider(GameObject gameObject, float mass, Vector2 relativePointA, Vector2 relativePointB)
            : base(gameObject, mass)
        {
            m_vRelativePointA = relativePointA;
            m_vRelativePointB = relativePointB;
        }

        public Vector2 GetWorldPointA()
        {
            return GetParent().GetPosition() + m_vRelativePointA;
        }

        public Vector2 GetWorldPointB()
        {
            return GetParent().GetPosition() + m_vRelativePointB;
        }

        public override float GetMomentOfInertia()
        {
            return 0.0f;
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

            float denominator = (otherPointB.Y - otherPointA.Y) * (myPointB.X - myPointA.X) - (otherPointB.X - otherPointA.X) * (myPointB.Y - myPointA.Y);
            if (denominator == 0)
            {
                return null;
            }
            float distAlongThis = ((otherPointB.X - otherPointA.X) * (myPointA.Y - otherPointA.Y) - (otherPointB.Y - otherPointA.Y) * (myPointA.X - otherPointA.X)) / denominator;
            if (distAlongThis < 0 || distAlongThis > 1)
            {
                return null;
            }
            float distAlongOther = ((myPointB.X - myPointA.X) * (myPointA.Y - otherPointA.Y) - (myPointB.Y - myPointA.Y) * (myPointA.X - otherPointA.X)) / denominator;
            if (distAlongOther < 0 || distAlongOther > 1)
            {
                return null;
            }

            Vector2 pointOfCollision = myPointA + (distAlongThis * (myPointB - myPointA));
            Collision collision = new Collision(new CollisionPoint(pointOfCollision, this, other), Vector2.Zero);
            return collision;
        }

        public override Collision CollidesWith(SquareCollider other)
        {
            return null;
        }

        public override Collision CollidesWith(CircleCollider other)
        {
            Vector2 otherWorldOrigin = other.GetOrigin();
            Vector2 myPointA = GetWorldPointA() - otherWorldOrigin;
            Vector2 myPointB = GetWorldPointB() - otherWorldOrigin;

            // Trivial bounding box test
            Vector2 myTopLeft = new Vector2();
            Vector2 myBottomRight = new Vector2();
            if (GetWorldPointA().X > GetWorldPointA().X)
            {
                myTopLeft.X = GetWorldPointA().X;
                myBottomRight.X = GetWorldPointA().X;
            }
            if (GetWorldPointA().Y > GetWorldPointA().Y)
            {
                myTopLeft.Y = GetWorldPointA().Y;
                myBottomRight.Y = GetWorldPointA().Y;
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
            else if (delta < 0.5f)
            {
                float distAlongLine = (-b) / (2 * a);
                if (distAlongLine < 0 || distAlongLine > 1)
                {
                    return null;
                }
                Vector2 pointOfCollision = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                Vector2 axisOfCollision = pointOfCollision - other.GetOrigin();
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
                        Vector2 pointOfCollision = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                        Vector2 axisOfCollision = pointOfCollision - other.GetOrigin();
                        Collision collision = new Collision(new CollisionPoint(pointOfCollision, this, other), axisOfCollision);
                        return collision;
                    }
                }
                // First point is on line
                else
                {
                    Vector2 pointOfCollision = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                    Vector2 axisOfCollision = pointOfCollision - other.GetOrigin();
                    Collision collision = new Collision(new CollisionPoint(pointOfCollision, this, other), axisOfCollision);
                    distAlongLine = (-b - sqrt) / (2 * a);
                    // Second point is not on line
                    if (distAlongLine < 0 || distAlongLine > 1)
                    {
                        return collision;
                    }
                    // TODO this is broken because of axis of collision
                    /*
                    pointOfCollision = GetWorldPointA() + (distAlongLine * (GetWorldPointB() - GetWorldPointA()));
                    axisOfCollision = pointOfCollision - other.GetOrigin();
                    collision.AddCollisionPoint(new CollisionPoint(pointOfCollision, this, other));
                    return collision;
                     * */
                    return null;
                }
            }
        }
    }
}
