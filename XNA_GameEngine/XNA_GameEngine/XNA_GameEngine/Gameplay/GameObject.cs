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
        private Vector3 m_vPosition;
        private float m_fRotation;
        private float m_fScale;
        private ICoreComponent [] m_GOComponents;
        Guid m_GORef;

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


        public Vector3 GetPosition()
        {
            return m_vPosition;
        }

        public void SetPosition(Vector3 position)
        {
            m_vPosition = position;
        }

        public float GetScale()
        {
            return m_fScale;
        }

        public void SetScale(float scale)
        {
            m_fScale = scale;
        }

        public Guid GetRef()
        {
            return m_GORef;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public void Render()
        {
            // Code for setting up graphics data to be rendered.

        }

    }
}
