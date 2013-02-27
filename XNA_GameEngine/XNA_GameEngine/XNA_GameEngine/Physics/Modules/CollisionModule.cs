using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNA_GameEngine.Physics.Colliders;

namespace XNA_GameEngine.Physics.Modules
{
    class CollisionModule : IPhysicsModule
    {
        private ICollider m_collider;
        private float m_fElasticity;
        public CollisionModule(ICollider collider, float elasticity)
        {
            m_collider = collider;
            m_fElasticity = elasticity / 2.0f;
            m_Type = PhysicsObject.ModuleType.MODULE_Collision;
        }

        public override void Update(GameTime gametime, PhysicsObject physicsObject)
        {
            LinkedList<PhysicsObject> updatedObjects = PhysicsWorld.GetInstance().GetAlreadyUpdatedObjects();
            foreach (PhysicsObject physObj in updatedObjects)
            {
                if (physObj.HasModule(PhysicsObject.ModuleType.MODULE_Collision))
                {
                    ICollider other = ((CollisionModule) physObj.GetModule(PhysicsObject.ModuleType.MODULE_Collision)).m_collider;
                    if (m_collider.CollidesWith(other))
                    {
                        XNA_GameEngine.Debug.DebugTools.Report("I collided!");
                    }
                }
            }
        }
    }
}
