using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using XNA_GameEngine.Core;

namespace XNA_GameEngine.Gameplay
{
    abstract class GameObject
    {
        private Vector3 m_position;
        private float m_rotation;
        private ICoreComponent [] m_GOComponents;

        public GameObject()
        {
            m_GOComponents = new ICoreComponent[(int)ICoreComponent.ComponentType.COMPONENT_COUNT];
        }

        public void Initialize()
        {
            for (int i = 0; i < (int)ICoreComponent.ComponentType.COMPONENT_COUNT; i++)
            {
                m_GOComponents[i] = null;
            }
        }

        public ICoreComponent GetComponentByTypeOrNULL(ICoreComponent.ComponentType componentType)
        {
            return m_GOComponents[(int)componentType];
        }

        public void AddComponent(ICoreComponent component)
        {
            m_GOComponents[(int)component.GetComponentType()] = component;
        }

        public void RemoveComponent(ICoreComponent component)
        {
            int componentType = (int)component.GetComponentType();
            if (m_GOComponents[componentType] == component)
            {
                m_GOComponents[componentType] = null;
            }
        }
          

        public void Update()
        {

        }

        public void Render()
        {
            // Code for setting up graphics data to be rendered.

        }

    }
}
