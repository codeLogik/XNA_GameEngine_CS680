using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Core;

namespace XNA_GameEngine.Physics
{
    class PhysicsObject : ICoreComponent
    {
        private Vector2 m_vVelocity;
        private float m_fMass;
        private ICollider m_collision;

        public PhysicsObject(GameObject parentGO)
            : base(parentGO)
        {
            m_Type = ComponentType.COMPONENT_Physics;
            m_ownerGO = parentGO;
        }

        public void Initialize()
        {

        }

    }
}
