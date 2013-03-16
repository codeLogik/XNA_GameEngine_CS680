using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Core;
using XNA_GameEngine.Physics.Colliders;

namespace XNA_GameEngine.Physics
{
    class PhysicsObject : ICoreComponent
    {
        private ICollider m_collider;
        private bool m_bImmobile;
        protected Vector2 m_vVelocity;
        protected Vector2 m_vAcceleration;
        protected float m_fAngularVelocity;
        protected float m_fAngularAcceleration;
        protected float m_fMass;
        protected float m_fElasticity;

        public PhysicsObject(GameObject parentGO, float mass, ICollider collider)
            : base(parentGO)
        {
            m_Type = ComponentType.COMPONENT_Physics;
            m_ownerGO = parentGO;
            m_collider = collider;
            collider.SetParent(this);
            m_bImmobile = false;

            m_fMass = mass;
            m_vVelocity = new Vector2(0.0f, 0.0f);
            m_vAcceleration = new Vector2(0.0f, 0.0f);
            m_fAngularVelocity = 0.0f;
            m_fAngularAcceleration = 0.0f;
            m_fElasticity = 0.5f;
        }

        public void Immobilize()
        {
            m_bImmobile = true;
        }

        public ICollider GetCollider()
        {
            return m_collider;
        }

        public float GetElasticity()
        {
            return m_fElasticity;
        }

        public void SetElasticity(float elasticity)
        {
            m_fElasticity = (elasticity / 2.0f);
        }

        public float GetMass()
        {
            return m_fMass;
        }

        public float GetAngularVelocity()
        {
            return m_fAngularVelocity;
        }

        public void SetAngularVelocity(float angularVelocity)
        {
            if (!m_bImmobile)
            {
                m_fAngularVelocity = angularVelocity;
            }
        }

        public Vector2 GetVelocity()
        {
            return m_vVelocity;
        }

        public float GetSpeed()
        {
            return m_vVelocity.Length();
        }

        public void SetVelocity(Vector2 velocity)
        {
            if (!m_bImmobile)
            {
                m_vVelocity = velocity;
            }
        }

        public void SetPosition(Vector2 position)
        {
            if (position != GetPosition())
            {
                m_collider.ClearBoundingBox();
                GetParent().SetPosition(position);
            }
        }

        public Vector2 GetPosition()
        {
            return GetParent().GetPosition();
        }

        public double GetRotation()
        {
            return GetParent().GetRotation();
        }

        public void SetRotation(double rotation)
        {
            if (rotation != GetRotation())
            {
                m_collider.ClearBoundingBox();
                GetParent().SetRotation(rotation);
            }
        }

        public void Initialize()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (!m_bImmobile)
            {
                Vector2 forceSum = PhysicsWorld.GetInstance().GetGravity() * m_fMass;

                double elapsedTimeInSec = gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 originalPosition = GetPosition();
                Vector2 originalVelocity = m_vVelocity;
                Vector2 newPosition = originalPosition + (originalVelocity * (float)elapsedTimeInSec);
                Vector2 newVelocity = originalVelocity + ((forceSum / m_fMass) * (float)elapsedTimeInSec);

                SetPosition(newPosition);
                m_vVelocity = newVelocity;

                double originalRotation = GetRotation();
                double newRotation = originalRotation + (m_fAngularVelocity * elapsedTimeInSec);
                while (newRotation > 2 * Math.PI)
                {
                    newRotation = newRotation - (2 * Math.PI);
                }

                while (newRotation < -2 * Math.PI)
                {
                    newRotation = newRotation + (2 * Math.PI);
                }
                SetRotation(newRotation);
            }
        }

        public void CollideWith(PhysicsObject physObj)
        {
            ICollider other = physObj.m_collider;
            Collision collision = m_collider.CollidesWith(other);
            if (collision != null)
            {
        //        XNA_GameEngine.Debug.DebugTools.Report("");

                // This only ever returns one point now
                CollisionPoint point = collision.GetCollisionPoint();
                Vector2 axisOfCollision = point.AxisOfCollision;
                Vector2 pointOfCollision = point.WorldLocation;

                // Get impulse
                Vector2 distanceFromCenterA = pointOfCollision - GetParent().GetPosition();
                Vector2 distanceFromCenterB = pointOfCollision - physObj.GetParent().GetPosition();
                Vector2 initalTotalVelocityA = m_vVelocity + new Vector2(-m_fAngularVelocity * distanceFromCenterA.Y, m_fAngularVelocity * distanceFromCenterA.X);
                if (m_bImmobile) initalTotalVelocityA = Vector2.Zero;
                Vector2 initalTotalVelocityB = physObj.m_vVelocity + new Vector2(-physObj.m_fAngularVelocity * distanceFromCenterB.Y, physObj.m_fAngularVelocity * distanceFromCenterB.X);
                if (physObj.m_bImmobile) initalTotalVelocityB = Vector2.Zero;

                Vector2 relativeVelocity = initalTotalVelocityB - initalTotalVelocityA;

                double crossA = distanceFromCenterA.X * axisOfCollision.Y - distanceFromCenterA.Y * axisOfCollision.X;
                double crossB = distanceFromCenterB.X * axisOfCollision.Y - distanceFromCenterB.Y * axisOfCollision.X;
                double momentOfInertiaA = m_collider.GetMomentOfInertia(m_fMass);
                double momentOfInertiaB = physObj.m_collider.GetMomentOfInertia(physObj.m_fMass);

                double impulse = Vector2.Dot(((m_fElasticity + physObj.m_fElasticity + 1.0f) * (initalTotalVelocityB - initalTotalVelocityA)), axisOfCollision) / ((1.0f / m_fMass) + (1.0f / physObj.m_fMass) + ((crossA * crossA) / momentOfInertiaA) + ((crossB * crossB) / momentOfInertiaB));
                Vector2 impulseVector = (float)impulse * axisOfCollision;
                    
                // Apply impulse
                SetVelocity(GetVelocity() + (impulseVector / m_fMass));
                physObj.SetVelocity(physObj.GetVelocity() - (impulseVector / physObj.m_fMass));
                    
          //       XNA_GameEngine.Debug.DebugTools.Report("Axis of Colliision: " + axisOfCollision); 
                //  XNA_GameEngine.Debug.DebugTools.Report("Point of Collisin: " + pointOfCollision);

                crossA = distanceFromCenterA.X * impulseVector.Y - distanceFromCenterA.Y * impulseVector.X;
                SetAngularVelocity(GetAngularVelocity() + (float) (crossA / m_collider.GetMomentOfInertia(m_fMass)));

                crossB = distanceFromCenterB.X * impulseVector.Y - distanceFromCenterB.Y * impulseVector.X;
                physObj.SetAngularVelocity(physObj.GetAngularVelocity() - (float) (crossB / physObj.m_collider.GetMomentOfInertia(physObj.m_fMass)));

                    
                // Collision Correction Component
                Vector2 resolveOverlap = collision.GetResolveOverlap();
            //XNA_GameEngine.Debug.DebugTools.Report("Overlap Distance: " + resolveOverlap);
                if(!m_bImmobile && !physObj.m_bImmobile) {
                    SetPosition(GetParent().GetPosition() - (resolveOverlap / 2.0f));
                    physObj.SetPosition(physObj.GetParent().GetPosition() + (resolveOverlap / 2.0f));
                }
                else if (!m_bImmobile)
                {
                    SetPosition(GetParent().GetPosition() - resolveOverlap);
                }
                else if (!physObj.m_bImmobile)
                {
                    physObj.SetPosition(physObj.GetParent().GetPosition() + resolveOverlap);
                }
            }
        }
    }
}
