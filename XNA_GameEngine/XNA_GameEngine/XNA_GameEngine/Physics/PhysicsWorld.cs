using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Physics
{
    class PhysicsWorld
    {
        private LinkedList<PhysicsObject> m_physicsObjects;

        private static PhysicsWorld g_PhysicsWorld;

        public PhysicsWorld()
        {

        }

        public void Initialize()
        {

        }

        static public PhysicsWorld GetInstance()
        {
            if (g_PhysicsWorld == null)
            {
                return new PhysicsWorld();
            }
            else
            {
                return g_PhysicsWorld;
            }
        }

        public void Update()
        {
            // Simulate the physics for the frame.
        }
    }
}
