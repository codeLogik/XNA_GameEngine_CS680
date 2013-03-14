﻿using System;
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

        public Vector2 GetVelocity()
        {
            return m_vVelocity;
        }

        public void SetVelocity(Vector2 velocity)
        {
            if (!m_bImmobile)
            {
                m_vVelocity = velocity;
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
                Vector2 originalPosition = GetParent().GetPosition();
                Vector2 originalVelocity = m_vVelocity;
                Vector2 newPosition = originalPosition + (originalVelocity * (float)elapsedTimeInSec);
                Vector2 newVelocity = originalVelocity + ((forceSum / m_fMass) * (float)elapsedTimeInSec);

                GetParent().SetPosition(newPosition);
                m_vVelocity = newVelocity;

                double originalRotation = GetParent().GetRotation();
                double newRotation = originalRotation + (m_fAngularVelocity * elapsedTimeInSec);
                while (newRotation > 2 * Math.PI)
                {
                    newRotation = newRotation - (2 * Math.PI);
                }

                while (newRotation < -2 * Math.PI)
                {
                    newRotation = newRotation + (2 * Math.PI);
                }
                GetParent().SetRotation(newRotation);
            }
            
            LinkedList<PhysicsObject> updatedObjects = PhysicsWorld.GetInstance().GetAlreadyUpdatedObjects();
            foreach (PhysicsObject physObj in updatedObjects)
            {
                ICollider other = physObj.m_collider;
                Collision collision = m_collider.CollidesWith(other);
                if (collision != null)
                {
               //     XNA_GameEngine.Debug.DebugTools.Report("");

                    // This only ever returns one point now
                    CollisionPoint point = collision.GetCollisionPoint();
                    Vector2 axisOfCollision = point.AxisOfCollision;
                    Vector2 pointOfCollision = point.WorldLocation;

                    // Get impulse
                    float tmp1 = Vector2.Dot(m_vVelocity, axisOfCollision);
                    float tmp2 = Vector2.Dot(physObj.m_vVelocity, axisOfCollision);
                    if (m_bImmobile) tmp1 = 0.0f;
                    if (physObj.m_bImmobile) tmp2 = 0.0f;

                    double impulse = ((m_fElasticity + physObj.m_fElasticity + 1.0f) * (tmp2 - tmp1)) / ((1.0f / m_fMass) + (1.0f / physObj.m_fMass));
                    Vector2 impulseVector = (float)impulse * axisOfCollision;
                    /*
                    XNA_GameEngine.Debug.DebugTools.Report("Impulse Vector: " + impulseVector);

                    XNA_GameEngine.Debug.DebugTools.Report("My Velocity: " + m_vVelocity);
                    XNA_GameEngine.Debug.DebugTools.Report("Other Velocity: " + physObj.m_vVelocity);
                    */
                    // Apply impulse
                    SetVelocity(GetVelocity() + (impulseVector / m_fMass));
                    physObj.SetVelocity(physObj.GetVelocity() - (impulseVector / physObj.m_fMass));
                    /*
                    XNA_GameEngine.Debug.DebugTools.Report("Axis of Colliision: " + axisOfCollision); 
                    XNA_GameEngine.Debug.DebugTools.Report("Point of Collisin: " + pointOfCollision);
                    */
                    Vector2 originToCollision = pointOfCollision - GetParent().GetPosition();
                    double crossProduct = impulseVector.X * originToCollision.Y - impulseVector.Y * originToCollision.X;
                    m_fAngularVelocity -= (float) (crossProduct / m_collider.GetMomentOfInertia(m_fMass));

                    originToCollision = pointOfCollision - physObj.GetParent().GetPosition();
                    crossProduct = impulseVector.X * originToCollision.Y - impulseVector.Y * originToCollision.X;
                    physObj.m_fAngularVelocity += ((float)crossProduct / physObj.m_collider.GetMomentOfInertia(physObj.m_fMass));

                    Vector2 resolveOverlap = collision.GetResolveOverlap();
                    XNA_GameEngine.Debug.DebugTools.Report("Overlap Distance: " + resolveOverlap);
                    if(!m_bImmobile && !physObj.m_bImmobile) {
                        GetParent().SetPosition(GetParent().GetPosition() - (resolveOverlap / 2.0f));
                        physObj.GetParent().SetPosition(physObj.GetParent().GetPosition() + (resolveOverlap / 2.0f));
                    }
                    else if (!m_bImmobile)
                    {
                        GetParent().SetPosition(GetParent().GetPosition() - resolveOverlap);
                    }
                    else if (!physObj.m_bImmobile)
                    {
                        physObj.GetParent().SetPosition(physObj.GetParent().GetPosition() + resolveOverlap);
                    }
                    
                }
            }
        }
    }
}
