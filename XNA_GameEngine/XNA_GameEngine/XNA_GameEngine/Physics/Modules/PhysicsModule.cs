using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics.Modules
{
    abstract class IPhysicsModule
    {
        protected PhysicsObject.ModuleType m_Type;

        public PhysicsObject.ModuleType GetModuleType()
        {
            return m_Type;
        }

        public abstract void Update(GameTime gametime, PhysicsObject physicsObject);
    }
}
