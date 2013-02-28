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

        public PhysicsObject(GameObject parentGO, ICollider collider)
            : base(parentGO)
        {
            m_Type = ComponentType.COMPONENT_Physics;
            m_ownerGO = parentGO;
            m_collider = collider;
            m_bImmobile = false;
        }

        public void Immobilize()
        {
            m_bImmobile = true;
        }

        public void Initialize()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (!m_bImmobile)
            {
                Vector2 forceSum = PhysicsWorld.GetInstance().GetGravity() * m_collider.GetMass();

                double elapsedTimeInSec = gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 originalPosition = GetParent().GetPosition();
                Vector2 originalVelocity = m_collider.GetVelocity();
                Vector2 newPosition = originalPosition + (originalVelocity * (float)elapsedTimeInSec);
                Vector2 newVelocity = originalVelocity + ((forceSum / m_collider.GetMass()) * (float)elapsedTimeInSec);

                GetParent().SetPosition(newPosition);
                m_collider.SetVelocity(newVelocity);

                //TODO: Angular stuff
            }
            
            LinkedList<PhysicsObject> updatedObjects = PhysicsWorld.GetInstance().GetAlreadyUpdatedObjects();
            foreach (PhysicsObject physObj in updatedObjects)
            {
                if (physObj.GetParent() != m_collider.GetParent())
                {
                    ICollider other = physObj.m_collider;
                    Collision collision = m_collider.CollidesWith(other);
                    if (collision != null)
                    {
                        Vector2 n = collision.GetAxisOfCollision();
                        float tmp1 = Vector2.Dot(m_collider.GetVelocity(), n);
                        float tmp2 = Vector2.Dot(other.GetVelocity(), n);
                        double impulse = ((m_collider.GetElasticity() + other.GetElasticity() + 1.0f) * (tmp2 - tmp1)) / ((1.0f / m_collider.GetMass()) + (1.0f / other.GetMass()));
                        Vector2 impulseVector = (float)impulse * n;
                        m_collider.SetVelocity(m_collider.GetVelocity() + (impulseVector / m_collider.GetMass()));
                        other.SetVelocity(other.GetVelocity() - (impulseVector / other.GetMass()));
                    }
                }
            }
        }
    }
}
