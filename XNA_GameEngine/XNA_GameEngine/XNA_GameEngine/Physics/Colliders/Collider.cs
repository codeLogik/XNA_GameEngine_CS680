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
        public ICollider ThisCollider;
        public ICollider OtherCollider;

        public CollisionPoint(Vector2 worldLocation, ICollider thisCollider, ICollider other)
        {
            WorldLocation = worldLocation;
            ThisCollider = thisCollider;
            OtherCollider = other;
        }
    }

    class Collision
    {
        private LinkedList<CollisionPoint> m_collisionPoints;
        private Vector2 m_vAxisOfCollision;

        public Collision(CollisionPoint point, Vector2 axisOfCollision)
        {
            m_collisionPoints = new LinkedList<CollisionPoint>();
            m_collisionPoints.AddLast(point);
            m_vAxisOfCollision = axisOfCollision;
        }

        public Vector2 GetAxisOfCollision()
        {
            return m_vAxisOfCollision;
        }

        public void AddCollisionPoint(CollisionPoint point) {
            m_collisionPoints.AddLast(point);
        }

        public LinkedList<CollisionPoint> GetPoints()
        {
            return m_collisionPoints;
        }
    }

    abstract class ICollider
    {
        protected Vector2 m_vVelocity;
        protected Vector2 m_vAcceleration;
        protected float m_fAngularVelocity;
        protected float m_fAngularAcceleration;
        protected float m_fMass;
        protected float m_fElasticity;
        protected GameObject m_parent;

        public ICollider(GameObject gameObject, float mass)
        {
            m_parent = gameObject;

            m_fMass = mass;
            m_vVelocity = new Vector2(0.0f, 0.0f);
            m_vAcceleration = new Vector2(0.0f, 0.0f);
            m_fAngularVelocity = 0.0f;
            m_fAngularAcceleration = 0.0f;
            m_fElasticity = 0.5f;
        }

        public float GetElasticity()
        {
            return m_fElasticity;
        }

        public GameObject GetParent()
        {
            return m_parent;
        }

        public float GetMass()
        {
            return m_fMass;
        }

        public Vector2 GetVelocity()
        {
            return m_vVelocity;
        }

        public void SetVelocity(Vector2 velocity)
        {
            m_vVelocity = velocity;
        }

        public Vector2 TransformToWorld(Vector2 point)
        {
            return GetParent().GetPosition() + point;
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

        public abstract float GetMomentOfInertia();
        public abstract Collision CollidesWith(SquareCollider other);
        public abstract Collision CollidesWith(CircleCollider other);
        public abstract Collision CollidesWith(LineCollider other);
    }
}
