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
        protected Vector2 m_vPosition;
        protected float m_fRotation;
        protected float m_fScale;
        protected ICoreComponent [] m_GOComponents;
        //Guid m_GORef;
        int m_GORef;

        public GameObject()
        {
            m_GOComponents = new ICoreComponent[(int)ICoreComponent.ComponentType.COMPONENT_COUNT];
            m_vPosition = Vector2.Zero;
            m_fScale = 1.0f;
            m_fRotation = 0.0f;
        }

        public virtual void Initialize()
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


        public Vector2 GetPosition()
        {
            return m_vPosition;
        }

        public void SetPosition(Vector2 position)
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

        public /*Guid*/ int GetRef()
        {
            return m_GORef;
        }

        public void SetRef(int refNew)
        {
            m_GORef = refNew;
        }

        public virtual void Update(GameTime gameTime)
        {
            // Run update on all of the components
            foreach (ICoreComponent coreComponent in m_GOComponents)
            {
                if (coreComponent != null)
                {
                    coreComponent.Update(gameTime);
                }
            } 
        }

        public virtual void Render()
        {
            // Code for setting up graphics data to be rendered.

        }

    }
}
