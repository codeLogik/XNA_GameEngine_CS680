using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Core;
using XNA_GameEngine.Physics.Colliders;
using XNA_GameEngine.Physics.Modules;

namespace XNA_GameEngine.Physics
{
    class PhysicsObject : ICoreComponent
    {
        public enum ModuleType
        {
            MODULE_ConstantVelocity = 0,
            MODULE_Collision,
            MODULE_COUNT
        }

        private Vector2 m_vCurrentVelocity;
        private Vector2 m_vProjectedVelocity;
        private float m_fMass;
        private IPhysicsModule[] m_modules;

        public PhysicsObject(GameObject parentGO)
            : base(parentGO)
        {
            m_modules = new IPhysicsModule[(int)ModuleType.MODULE_COUNT];

            m_vCurrentVelocity = new Vector2(0.0f, 0.0f);
            m_vProjectedVelocity = new Vector2(0.0f, 0.0f);
            m_Type = ComponentType.COMPONENT_Physics;
            m_ownerGO = parentGO;
        }

        public bool HasModule(PhysicsObject.ModuleType type) {
            return (m_modules[(int)type] != null);
        }

        public IPhysicsModule GetModule(PhysicsObject.ModuleType type)
        {
            return m_modules[(int)type];
        }

        public void AddModule(IPhysicsModule module)
        {
            m_modules[(int)module.GetModuleType()] = module;
        }

        public void AlterVelocity(Vector2 by)
        {
            m_vProjectedVelocity.X += by.X;
            m_vProjectedVelocity.Y += by.Y;
        }

        public void Initialize()
        {
            for (int i = 0; i < (int)PhysicsObject.ModuleType.MODULE_COUNT; i++)
            {
                m_modules[i] = null;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update the current velocity and object position
            m_vCurrentVelocity = m_vProjectedVelocity;
            double elapsedTimeInSec = gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 objectPosition = this.GetParent().GetPosition();
            objectPosition.X += (float)(elapsedTimeInSec * m_vCurrentVelocity.X);
            objectPosition.Y += (float)(elapsedTimeInSec * m_vCurrentVelocity.Y);
            this.GetParent().SetPosition(objectPosition);

            // Determine the projected velocity for the next update
            foreach (IPhysicsModule module in m_modules)
            {
                if (module != null)
                {
                    module.Update(gameTime, this);
                }
            }
        }
    }
}
