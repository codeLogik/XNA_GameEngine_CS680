using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics
{
    class PhysicsObject
    {
        private Vector3 m_vVelocity;
        private float m_fMass;
        private ICollider m_collision;

        private GameObject m_GOParent;

        public PhysicsObject(GameObject parentGO)
        {
            m_GOParent = parentGO;
        }

        public void Initialize()
        {

        }

    }
}
