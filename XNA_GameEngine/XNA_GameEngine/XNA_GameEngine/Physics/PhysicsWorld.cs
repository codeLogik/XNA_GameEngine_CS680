using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNA_GameEngine.Core;

using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Physics
{
    class PhysicsWorld
    {
        private PhysicsScene m_scene;
        private QuadTree m_physicsObjects;
        private Vector2 m_vGravity;

        private static PhysicsWorld g_PhysicsWorld;

        public PhysicsWorld()
        {
            m_vGravity = Vector2.Zero;
        }

        public void Initialize(PhysicsScene scene)
        {
            m_scene = scene;
            m_physicsObjects = new QuadTree(m_scene.GetBoundaries());
        }

        public void SetGravity(Vector2 gravity)
        {
            m_vGravity = gravity;
        }

        public Vector2 GetGravity()
        {
            return m_vGravity;
        }

        static public PhysicsWorld GetInstance()
        {
            if (g_PhysicsWorld == null)
            {
                g_PhysicsWorld = new PhysicsWorld();
            }
        
            return g_PhysicsWorld;
        }

        public void AddPhysicsObject(ICoreComponent coreComponent)
        {
            if (coreComponent != null && coreComponent.GetComponentType() == ICoreComponent.ComponentType.COMPONENT_Physics)
            {
                m_physicsObjects.AddPhysicsObject((PhysicsObject)coreComponent);
            }
        }

        public void Update(GameTime gameTime)
        {
            m_physicsObjects.UpdateObjects(gameTime);
        }
    }
}
