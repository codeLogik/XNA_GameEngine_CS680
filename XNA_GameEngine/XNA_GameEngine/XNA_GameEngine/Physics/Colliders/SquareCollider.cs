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
        protected Vector2[] m_vertices;
        protected Vector2[] m_sideNormals;
        protected Vector2 m_vSize;

        public SquareCollider(Vector2 size)
        {
            Vector2 halfSize = size / 2.0f;

            // Vertices in counter-clockwise order.
            m_vertices = new Vector2[] {
                new Vector2(halfSize.X, -halfSize.Y),
                new Vector2(-halfSize.X, -halfSize.Y),
                new Vector2(-halfSize.X, halfSize.Y),
                new Vector2(halfSize.X, halfSize.Y)
            };

            m_sideNormals = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                int j = i + 1;
                if (i == 3)
                {
                    j = 0;
                }
                Vector2 sideDirection = m_vertices[j] - m_vertices[i];
                float tempX = sideDirection.X;
                sideDirection.X = sideDirection.Y;
                sideDirection.Y = -tempX;
                sideDirection.Normalize();
                m_sideNormals[i] = sideDirection;
            }

            m_vSize = size;
        }

        public Vector2 GetSize()
        {
            return m_vSize;
        }

        public override float GetMomentOfInertia(float mass)
        {
            return mass * (m_vSize.X * m_vSize.X + m_vSize.Y + m_vSize.Y) / 12.0f;
        }

        public Boolean IsPointInside(Vector2 point)
        {
            Vector2 toPoint = point - GetParent().GetPosition();
            Matrix rotation = Matrix.CreateRotationZ((float)-GetParent().GetRotation());
            toPoint = Vector2.Transform(toPoint, rotation);

            if (toPoint.X < m_vertices[1].X || toPoint.X > m_vertices[3].X || toPoint.Y < m_vertices[1].Y || toPoint.Y > m_vertices[3].Y)
            {
                return false;
            }
            return true;
        }
        
        public override Collision CollidesWith(SquareCollider other)
        {
            BoundingBox2D myBox = GetBoundingBox();
            BoundingBox2D otherBox = other.GetBoundingBox();
            // Trivial bounding box test
            if (!myBox.Overlap(otherBox))
            {
                return null;
            }

            LineCollider[] mySides = new LineCollider[] {
                new LineCollider(m_vertices[0], m_vertices[1]),
                new LineCollider(m_vertices[1], m_vertices[2]),
                new LineCollider(m_vertices[2], m_vertices[3]),
                new LineCollider(m_vertices[3], m_vertices[0])
            };

            LineCollider[] otherSides = new LineCollider[] {
                new LineCollider(other.m_vertices[0], other.m_vertices[1]),
                new LineCollider(other.m_vertices[1], other.m_vertices[2]),
                new LineCollider(other.m_vertices[2], other.m_vertices[3]),
                new LineCollider(other.m_vertices[3], other.m_vertices[0])
            };

            // Make sure the LineColliders can transform and rotate their points to the world coordinates
            for (int i = 0; i < 4; i++)
            {
                mySides[i].SetParent(GetParent());
                otherSides[i].SetParent(other.GetParent());
            }

            Vector2[] collisionPoints = new Vector2[2];
            int p = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Collision thisCollision = mySides[i].CollidesWith(otherSides[j]);
                    if (thisCollision != null)
                    {
                        collisionPoints[p] = thisCollision.GetCollisionPoint().WorldLocation;
                        p++;
                        if (p == 2)
                        {
                            Vector2 location = (collisionPoints[0] + collisionPoints[1]) / 2;
                            Vector2 axisOfCollision = collisionPoints[1] - collisionPoints[0];

                            float tempX = axisOfCollision.X;
                            axisOfCollision.X = -axisOfCollision.Y;
                            axisOfCollision.Y = tempX;
                            axisOfCollision.Normalize();
                            Vector2 outOfSelf = location - GetParent().GetPosition();
                            if (Vector2.Dot(axisOfCollision, outOfSelf) < 0)
                            {
                                axisOfCollision = -axisOfCollision;
                            }


                            // ResolveOverlap will be the deepest corner into the other square
                            Vector2 resolveOverlap = Vector2.Zero;
                            for (int k = 0; k < 4; k++)
                            {
                                Vector2 worldVertex = base.TransformToWorld(m_vertices[k]);
                                Vector2 dirToPoint = worldVertex - location;
                                if (Vector2.Dot(axisOfCollision, dirToPoint) > 0)
                                {
                                    Vector2 temp = (Vector2.Dot(axisOfCollision, dirToPoint) * axisOfCollision);
                                    if (temp.LengthSquared() > resolveOverlap.LengthSquared())
                                    {
                                        resolveOverlap = temp;
                                    }
                                }
                            }

                            // ResolveOverlap2 will be the deepest other corner into my square
                            Vector2 resolveOverlap2 = Vector2.Zero;
                            for (int k = 0; k < 4; k++)
                            {
                                Vector2 worldVertex = other.TransformToWorld(other.m_vertices[k]);
                                Vector2 dirToPoint = worldVertex - location;
                                if (Vector2.Dot(-axisOfCollision, dirToPoint) > 0)
                                {
                                    Vector2 temp = (Vector2.Dot(-axisOfCollision, dirToPoint) * -axisOfCollision);
                                    if (temp.LengthSquared() > resolveOverlap.LengthSquared())
                                    {
                                        resolveOverlap2 = temp;
                                    }
                                }
                            }

                            resolveOverlap -= resolveOverlap2;

                            Collision collision = new Collision(new CollisionPoint(location, axisOfCollision, this, other), resolveOverlap);
                            return collision;                                                               
                        }
                    }
                }
            }

            return null;
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

            LineCollider[] mySides = new LineCollider[] {
                new LineCollider(m_vertices[0], m_vertices[1]),
                new LineCollider(m_vertices[1], m_vertices[2]),
                new LineCollider(m_vertices[2], m_vertices[3]),
                new LineCollider(m_vertices[3], m_vertices[0])
            };

            // Make sure the LineColliders can transform and rotate their points to the world coordinates
            for (int i = 0; i < 4; i++)
            {
                mySides[i].SetParent(GetParent());
            }

            Vector2[] collisionPoints = new Vector2[2];
            int p = 0;
            int collidingEdge = -1;
            for (int i = 0; i < 4; i++)
            {
                Collision thisCollision = mySides[i].CollidesWith(other);
                if (thisCollision != null)
                {
                    collidingEdge = i;
                    collisionPoints[p] = thisCollision.GetCollisionPoint().WorldLocation;
                    p++;
                    
                    // A line can only intersect with a square at most at 2 places
                    if (p == 2)
                    {
                        Vector2 location = (collisionPoints[0] + collisionPoints[1]) / 2;
                        Vector2 axisOfCollision = collisionPoints[1] - collisionPoints[0];

                        float tempX = axisOfCollision.X;
                        axisOfCollision.X = -axisOfCollision.Y;
                        axisOfCollision.Y = tempX;
                        axisOfCollision.Normalize();
                        Vector2 outOfSelf = location - GetParent().GetPosition();
                        if (Vector2.Dot(axisOfCollision, outOfSelf) < 0)
                        {
                            axisOfCollision = -axisOfCollision;
                        }

                        Vector2 resolveOverlap = Vector2.Zero;
                        for (i = 0; i < 4; i++)
                        {
                            Vector2 worldVertex = base.TransformToWorld(m_vertices[i]);
                            Vector2 dirToPoint = worldVertex - location;
                            if (Vector2.Dot(axisOfCollision, dirToPoint) > 0)
                            {
                                Vector2 temp = (Vector2.Dot(axisOfCollision, dirToPoint) * axisOfCollision);
                                if (temp.LengthSquared() > resolveOverlap.LengthSquared())
                                {
                                    resolveOverlap = temp;
                                }
                            }
                        }

                        Collision collision = new Collision(new CollisionPoint(location, axisOfCollision, this, other), resolveOverlap);
                        return collision;
                    }
                }
            }
            if (p == 1)
            {
                Vector2 location = collisionPoints[0];
                Matrix rotation = Matrix.CreateRotationZ((float)GetParent().GetRotation());
                Vector2 axisOfCollision = Vector2.Transform(m_sideNormals[collidingEdge], rotation);

                Vector2 pointA = other.TransformToWorld(other.GetLocalA());
                Vector2 pointB = other.TransformToWorld(other.GetLocalB());

                Vector2 resolveOverlap = Vector2.Zero;
                if (IsPointInside(pointA)) {
                    resolveOverlap = Vector2.Dot(-axisOfCollision, pointA - location) * -axisOfCollision;
                }
                else if (IsPointInside(pointB)) {
                    resolveOverlap = Vector2.Dot(-axisOfCollision, pointB - location) * -axisOfCollision;
                }

                Collision collision = new Collision(new CollisionPoint(location, axisOfCollision, this, other), resolveOverlap);
                return collision;
            }
            return null;
        }

        public override Collision CollidesWith(CircleCollider other)
        {
            BoundingBox2D myBox = GetBoundingBox();
            BoundingBox2D otherBox = other.GetBoundingBox();

            // Trivial bounding box test
            if (!myBox.Overlap(otherBox))
            {
                return null;
            }

            LineCollider[] mySides = new LineCollider[] {
                new LineCollider(m_vertices[0], m_vertices[1]),
                new LineCollider(m_vertices[1], m_vertices[2]),
                new LineCollider(m_vertices[2], m_vertices[3]),
                new LineCollider(m_vertices[3], m_vertices[0])
            };

            // Make sure the LineColliders can transform and rotate their points to the world coordinates
            for (int i = 0; i < 4; i++)
            {
                mySides[i].SetParent(GetParent());
            }

            Vector2 location = Vector2.Zero;
            int p = 0;
            for (int i = 0; i < 4; i++)
            {
                Collision thisCollision = mySides[i].CollidesWith(other);
                if (thisCollision != null)
                {
                    CollisionPoint point = thisCollision.GetCollisionPoint();
                    p++;
                    location += point.WorldLocation;
                }
            }
            if (p == 0)
            {
                return null;
            }

            location = location / p;
            Vector2 axisOfCollision = other.GetOrigin() - location;
            axisOfCollision.Normalize();

            // ResolveOverlap will be the deepest corner into the circle
            Vector2 resolveOverlap = Vector2.Zero;
            for (int i = 0; i < 4; i++)
            {
                Vector2 worldVertex = base.TransformToWorld(m_vertices[i]);
                Vector2 dirToPoint = worldVertex - location;
                if (Vector2.Dot(axisOfCollision, dirToPoint) > 0)
                {
                    Vector2 temp = (Vector2.Dot(axisOfCollision, dirToPoint) * axisOfCollision);
                    if (temp.LengthSquared() > resolveOverlap.LengthSquared())
                    {
                        resolveOverlap = temp;
                    }
                }
            }

            // Add the distance that the circle overlaps into the square
            resolveOverlap += ((location - other.GetOrigin()) - (-axisOfCollision) * other.GetRadius());

            Collision collision = new Collision(new CollisionPoint(location, axisOfCollision, this, other), resolveOverlap);
            return collision;
        }

        public override BoundingBox2D GetBoundingBox()
        {
            if (m_boundingBox == null)
            {
                Vector2 vertex = base.TransformToWorld(m_vertices[0]);
                Vector2 topLeft = vertex;
                Vector2 bottomRight = vertex;

                for (int i = 1; i < 4; i++)
                {
                    vertex = base.TransformToWorld(m_vertices[i]);
                    topLeft = Vector2.Min(topLeft, vertex);
                    bottomRight = Vector2.Max(bottomRight, vertex);
                }

                m_boundingBox = new BoundingBox2D(topLeft, bottomRight);
            }
            return m_boundingBox;
        }
    }
}
