using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Core
{
    abstract class ICoreComponent
    {
        public enum ComponentType
        {
            COMPONENT_Physics = 0,
            COMPONENT_Rendering,
            COMPONENT_Networking,
            COMPONENT_Sound,
            COMPONENT_COUNT
        }

        protected GameObject m_ownerGO;
        protected ComponentType m_Type;

        public ICoreComponent(GameObject ownerGO)
        {
            m_ownerGO = ownerGO;
        }

        public ComponentType GetComponentType()
        {
            return m_Type;
        }

        public GameObject GetParent()
        {
            return m_ownerGO;
        }

        public abstract void Update(GameTime gameTime);
    }
}
