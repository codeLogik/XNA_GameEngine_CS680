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

                //TODO: Angular stuff
            }
            
            LinkedList<PhysicsObject> updatedObjects = PhysicsWorld.GetInstance().GetAlreadyUpdatedObjects();
            foreach (PhysicsObject physObj in updatedObjects)
            {
                ICollider other = physObj.m_collider;
                Collision collision = m_collider.CollidesWith(other);
                if (collision != null)
                {
                    LinkedList<CollisionPoint> points = collision.GetPoints();
                    Vector2 n = Vector2.Zero;
                    foreach(CollisionPoint point in points) {
                        n += point.AxisOfCollision;
                    }
                    XNA_GameEngine.Debug.DebugTools.Report("Number of points: " + points.Count());
                    n.Normalize();

                    float tmp1 = Vector2.Dot(m_vVelocity, n);
                    float tmp2 = Vector2.Dot(physObj.m_vVelocity, n);
                    if (m_bImmobile) tmp1 = 0.0f;
                    if (physObj.m_bImmobile) tmp2 = 0.0f;

                    double impulse = ((m_fElasticity + physObj.m_fElasticity + 1.0f) * (tmp2 - tmp1)) / ((1.0f / m_fMass) + (1.0f / physObj.m_fMass));
                    Vector2 impulseVector = (float)impulse * n;
                    m_vVelocity += (impulseVector / m_fMass);
                    physObj.m_vVelocity -= (impulseVector / physObj.m_fMass);
                }
            }
        }
    }
}
