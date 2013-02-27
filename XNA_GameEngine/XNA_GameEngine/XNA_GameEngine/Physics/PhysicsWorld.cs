﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNA_GameEngine.Core;

using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Physics
{
    class PhysicsWorld
    {
        private LinkedList<PhysicsObject> m_physicsObjects;
        private LinkedList<PhysicsObject> m_updatedPhysicsObjects;

        private static PhysicsWorld g_PhysicsWorld;

        public PhysicsWorld()
        {
            m_physicsObjects = new LinkedList<PhysicsObject>();
            m_updatedPhysicsObjects = new LinkedList<PhysicsObject>();
        }

        public void Initialize()
        {

        }

        static public PhysicsWorld GetInstance()
        {
            if (g_PhysicsWorld == null)
            {
                g_PhysicsWorld = new PhysicsWorld();
            }
        
            return g_PhysicsWorld;
        }

        public LinkedList<PhysicsObject> GetAlreadyUpdatedObjects()
        {
            return m_updatedPhysicsObjects;
        }

        public void AddPhysicsObject(ICoreComponent coreComponent)
        {
            if (coreComponent != null && coreComponent.GetComponentType() == ICoreComponent.ComponentType.COMPONENT_Physics)
            {
                m_physicsObjects.AddLast((PhysicsObject)coreComponent);
            }
        }

        public void Update(GameTime gameTime)
        {
            m_updatedPhysicsObjects.Clear();
            // Simulate the physics for the frame.
            foreach (PhysicsObject phyObj in m_physicsObjects)
            {
                phyObj.Update(gameTime);
                m_updatedPhysicsObjects.AddLast(phyObj);
            }
        }
    }
}
