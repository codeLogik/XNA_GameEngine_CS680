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
        protected Vector2 m_vSize;

        public SquareCollider(Vector2 size)
        {
            m_vSize = size;
        }

        public Vector2 GetTopLeft()
        {
            return new Vector2(GetParent().GetParent().GetPosition().X - (m_vSize.X / 2.0f), GetParent().GetParent().GetPosition().Y - (m_vSize.Y / 2.0f));
        }

        public Vector2 GetBottomRight()
        {
            return new Vector2(GetParent().GetParent().GetPosition().X + (m_vSize.X / 2.0f), GetParent().GetParent().GetPosition().Y + (m_vSize.Y / 2.0f));
        }

        public Vector2 GetSize()
        {
            return m_vSize;
        }

        public override float GetMomentOfInertia(float mass)
        {
            return mass * (m_vSize.Y * m_vSize.Y + m_vSize.X + m_vSize.X) / 12.0f;
        }

        public override Collision CollidesWith(SquareCollider other)
        {
            Vector2 myTopLeft = GetTopLeft();
            Vector2 myBottomRight = GetBottomRight();
            Vector2 otherTopLeft = other.GetTopLeft();
            Vector2 otherBottomRight = other.GetBottomRight();

            // Trivial bounding box test
            if (!base.BoundingBoxesOverlap(myTopLeft, myBottomRight, otherTopLeft, otherBottomRight))
            {
                return null;
            }
            //XNA_GameEngine.Debug.DebugTools.Report("Boxes overlap!");
            LineCollider[] mySides = new LineCollider[4];
            LineCollider[] otherSides = new LineCollider[4];

            mySides[0] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)));
            mySides[0].SetParent(GetParent());
            mySides[1] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[1].SetParent(GetParent());
            mySides[2] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2(-(m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[2].SetParent(GetParent());
            mySides[3] = new LineCollider(new Vector2((m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[3].SetParent(GetParent());

            otherSides[0] = new LineCollider(new Vector2(-(other.m_vSize.X / 2.0f), -(other.m_vSize.Y / 2.0f)), new Vector2((other.m_vSize.X / 2.0f), -(other.m_vSize.Y / 2.0f)));
            otherSides[0].SetParent(other.GetParent());
            otherSides[1] = new LineCollider(new Vector2(-(other.m_vSize.X / 2.0f), (other.m_vSize.Y / 2.0f)), new Vector2((other.m_vSize.X / 2.0f), (other.m_vSize.Y / 2.0f)));
            otherSides[1].SetParent(other.GetParent());
            otherSides[2] = new LineCollider(new Vector2(-(other.m_vSize.X / 2.0f), -(other.m_vSize.Y / 2.0f)), new Vector2(-(other.m_vSize.X / 2.0f), (other.m_vSize.Y / 2.0f)));
            otherSides[2].SetParent(other.GetParent());
            otherSides[3] = new LineCollider(new Vector2((other.m_vSize.X / 2.0f), -(other.m_vSize.Y / 2.0f)), new Vector2((other.m_vSize.X / 2.0f), (other.m_vSize.Y / 2.0f)));
            otherSides[3].SetParent(other.GetParent());

            Collision collision = new Collision();
            Vector2 axisOfCollision = other.GetParent().GetParent().GetPosition() - GetParent().GetParent().GetPosition();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Collision thisCollision = mySides[i].CollidesWith(otherSides[j]);
                    if (thisCollision != null)
                    {
                        foreach (CollisionPoint point in thisCollision.GetPoints())
                        {
                            Vector2 location = point.WorldLocation;
                            
                            collision.AddCollisionPoint(new CollisionPoint(location, axisOfCollision, this, other));
                        }
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
            Vector2 myTopLeft = new Vector2(GetParent().GetParent().GetPosition().X - (m_vSize.X / 2.0f), GetParent().GetParent().GetPosition().Y - (m_vSize.Y / 2.0f));
            Vector2 myBottomRight = new Vector2(GetParent().GetParent().GetPosition().X + (m_vSize.X / 2.0f), GetParent().GetParent().GetPosition().Y + (m_vSize.Y / 2.0f));

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

            // Trivial bounding box test
            if (!base.BoundingBoxesOverlap(myTopLeft, myBottomRight, otherTopLeft, otherBottomRight))
            {
                return null;
            }
            
            LineCollider[] mySides = new LineCollider[4];

            mySides[0] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)));
            mySides[0].SetParent(GetParent());
            mySides[1] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[1].SetParent(GetParent());
            mySides[2] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2(-(m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[2].SetParent(GetParent());
            mySides[3] = new LineCollider(new Vector2((m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[3].SetParent(GetParent());

            Collision collision = new Collision();
            for (int i = 0; i < 4; i++)
            {
                Collision thisCollision = mySides[i].CollidesWith(other);
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

        public override Collision CollidesWith(CircleCollider other)
        {
            LineCollider[] mySides = new LineCollider[4];

            mySides[0] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)));
            mySides[0].SetParent(GetParent());
            mySides[1] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[1].SetParent(GetParent());
            mySides[2] = new LineCollider(new Vector2(-(m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2(-(m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[2].SetParent(GetParent());
            mySides[3] = new LineCollider(new Vector2((m_vSize.X / 2.0f), -(m_vSize.Y / 2.0f)), new Vector2((m_vSize.X / 2.0f), (m_vSize.Y / 2.0f)));
            mySides[3].SetParent(GetParent());

            Collision collision = new Collision();
            for (int i = 0; i < 4; i++)
            {
                Collision thisCollision = mySides[i].CollidesWith(other);
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
    }
}
