using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Physics.Modules
{
    class ConstantVelocityModule : IPhysicsModule
    {
        private Vector2 m_vAcceleration;

        public ConstantVelocityModule(Vector2 acceleration)
        {
            m_Type = PhysicsObject.ModuleType.MODULE_ConstantVelocity;
            m_vAcceleration = acceleration;
        }

        public override void Update(GameTime gametime, PhysicsObject physicsObject)
        {
            double elapsedTimeInSec = gametime.ElapsedGameTime.TotalSeconds;
            physicsObject.AlterVelocity(new Vector2((float)(elapsedTimeInSec * m_vAcceleration.X), (float)(elapsedTimeInSec * m_vAcceleration.Y)));
        }
    }
}
