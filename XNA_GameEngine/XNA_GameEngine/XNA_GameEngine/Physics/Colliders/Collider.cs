using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics.Colliders
{
    struct CollisionPoint {
        public Vector2 WorldLocation;
        public Vector2 AxisOfCollision;
        public ICollider ThisCollider;
        public ICollider OtherCollider;

        public CollisionPoint(Vector2 worldLocation, Vector2 axisOfCollision, ICollider thisCollider, ICollider other)
        {
            WorldLocation = worldLocation;
            AxisOfCollision = axisOfCollision;
            ThisCollider = thisCollider;
            OtherCollider = other;
        }
    }

    class Collision
    {
        private LinkedList<CollisionPoint> m_collisionPoints;
        private Vector2 m_vResolveOverlap;

        public Collision()
        {
            m_collisionPoints = new LinkedList<CollisionPoint>();
            m_vResolveOverlap = Vector2.Zero;
        }

        public Collision(CollisionPoint point)
        {
            m_collisionPoints = new LinkedList<CollisionPoint>();
            m_collisionPoints.AddLast(point);
            m_vResolveOverlap = Vector2.Zero;
        }

        public void AddCollisionPoint(CollisionPoint point) {
            m_collisionPoints.AddLast(point);
        }

        public void SetResolveOverlap(Vector2 resolveOverlap)
        {
            m_vResolveOverlap = resolveOverlap;
        }

        public Vector2 GetResolveOverlap()
        {
            return m_vResolveOverlap;
        }

        public LinkedList<CollisionPoint> GetPoints()
        {
            return m_collisionPoints;
        }
    }

    abstract class ICollider
    {
        protected PhysicsObject m_parent;

        public ICollider()
        {
            m_parent = null;
        }

        public PhysicsObject GetParent()
        {
            return m_parent;
        }

        public void SetParent(PhysicsObject parent)
        {
            m_parent = parent;
        }

        public Vector2 TransformToWorld(Vector2 point)
        {
            // TODO this probably doesnt work.
            return GetParent().GetParent().GetPosition() + point;
        }

        public Collision CollidesWith(ICollider other)
        {
            if (other is SquareCollider)
            {
                return CollidesWith((SquareCollider)other);
            }
            else if (other is CircleCollider)
            {
                return CollidesWith((CircleCollider)other);
            }
            else if (other is LineCollider)
            {
                return CollidesWith((LineCollider)other);
            }
            return null;
        }

        public bool BoundingBoxesOverlap(Vector2 aTopLeft, Vector2 aBottomRight, Vector2 bTopLeft, Vector2 bBottomRight)
        {
            if (aTopLeft.X > bBottomRight.X ||
                aBottomRight.X < bTopLeft.X ||
                aTopLeft.Y > bBottomRight.Y ||
                aBottomRight.Y < bTopLeft.Y)
            {
                return false;
            }
            return true;
        }

        public abstract float GetMomentOfInertia(float mass);
        public abstract Collision CollidesWith(SquareCollider other);
        public abstract Collision CollidesWith(CircleCollider other);
        public abstract Collision CollidesWith(LineCollider other);
    }
}
