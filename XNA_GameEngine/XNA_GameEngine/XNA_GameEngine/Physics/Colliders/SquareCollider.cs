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
        protected float m_vSize;

        public SquareCollider(float size)
        {
            float halfSize = size / 2;

            // Vertices in counter-clockwise order.
            m_vertices = new Vector2[] {
                new Vector2(halfSize, -halfSize),
                new Vector2(-halfSize, -halfSize),
                new Vector2(-halfSize, halfSize),
                new Vector2(halfSize, halfSize)
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
                sideDirection.Y = -sideDirection.Y;
                sideDirection.Normalize();
                m_sideNormals[i] = sideDirection;
            }

            m_vSize = size;
        }

        public float GetSize()
        {
            return m_vSize;
        }

        public override float GetMomentOfInertia(float mass)
        {
            return mass * (m_vSize * m_vSize + m_vSize + m_vSize) / 12.0f;
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

            //XNA_GameEngine.Debug.DebugTools.Report("Boxes overlap!");
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
                        CollisionPoint point = thisCollision.GetCollisionPoint();
                        if (p >= 2)
                        {
                            // Two rectangles can only intersect at 2 points max.
                            return null;
                        }
                        collisionPoints[p] = point.WorldLocation;
                        p++;
                    }
                }
            }
            if (p == 0)
            {
                return null;
            }

            Vector2 location = collisionPoints[0];
            Vector2 axisOfCollision = Vector2.Zero;
            if (p > 1)
            {
                XNA_GameEngine.Debug.DebugTools.Report("Collision Point 1: " + collisionPoints[0]);
                XNA_GameEngine.Debug.DebugTools.Report("Collision Point 2: " + collisionPoints[1]);
                location = (collisionPoints[0] + collisionPoints[1]) / 2;
                axisOfCollision = collisionPoints[1] - collisionPoints[0];

                XNA_GameEngine.Debug.DebugTools.Report("Initial Axis of Collision: " + axisOfCollision);

                float tempX = axisOfCollision.X;
                axisOfCollision.X = -axisOfCollision.Y;
                axisOfCollision.Y = tempX;
                axisOfCollision.Normalize();
                XNA_GameEngine.Debug.DebugTools.Report("My Rotation: " + GetParent().GetParent().GetRotation());
                Vector2 outOfSelf = location - GetParent().GetParent().GetPosition();
                if(Vector2.Dot(axisOfCollision, outOfSelf) < 0) {
                    axisOfCollision = -axisOfCollision;
                }
                Collision collision = new Collision(new CollisionPoint(location, axisOfCollision, this, other));
                return collision;
            }
            else
            {
                XNA_GameEngine.Debug.DebugTools.Report("This actually happened... okay");
                return null;
            }
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

            Vector2 squareOrigin = GetParent().GetParent().GetPosition();
            Vector2 linePA = other.TransformToWorld(other.GetLocalA());
            Vector2 linePB = other.TransformToWorld(other.GetLocalB());

            Vector2 toPointA = linePA - squareOrigin;
            Vector2 dirLine = linePB - linePA;
            dirLine.Normalize();

            Vector2 perpToLine = toPointA - (dirLine * Vector2.Dot(toPointA, dirLine));

            Matrix matrix = Matrix.CreateRotationZ(-((float)GetParent().GetParent().GetRotation()));
            Vector2 rotatedDirection = Vector2.Transform(perpToLine, matrix);

            float halfSize = m_vSize / 2.0f;
            Vector2 pointOnRectangle = Vector2.Max(rotatedDirection, new Vector2(-halfSize, -halfSize));
            pointOnRectangle = Vector2.Min(pointOnRectangle, new Vector2(halfSize, halfSize));

            if (perpToLine.LengthSquared() <= pointOnRectangle.LengthSquared())
            {
                pointOnRectangle = base.TransformToWorld(pointOnRectangle);
                Vector2 axisOfCollision = -perpToLine;
                axisOfCollision.Normalize();
                Collision collision = new Collision(new CollisionPoint(pointOnRectangle, axisOfCollision, this, other));
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

            Vector2 direction = other.GetOrigin() - GetParent().GetParent().GetPosition();

            Matrix matrix = Matrix.CreateRotationZ(-((float)GetParent().GetParent().GetRotation()));
            Vector2 rotatedDirection = Vector2.Transform(direction, matrix);

            float halfSize = m_vSize / 2.0f;
            Vector2 pointOnRectangle = Vector2.Max(rotatedDirection, new Vector2(-halfSize, -halfSize));
            pointOnRectangle = Vector2.Min(pointOnRectangle, new Vector2(halfSize, halfSize));

            float distance = direction.Length();
            if (distance <= (pointOnRectangle.Length() + other.GetRadius()))
            {
                pointOnRectangle = base.TransformToWorld(pointOnRectangle);

                Vector2 axisOfCollision = other.GetOrigin() - pointOnRectangle;
                axisOfCollision.Normalize();
                Collision collision = new Collision(new CollisionPoint(pointOnRectangle, axisOfCollision, this, other));
                return collision;
            }

            return null;
        }

        public override BoundingBox2D GetBoundingBox()
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
            
            return new BoundingBox2D(topLeft, bottomRight);
        }
    }
}
