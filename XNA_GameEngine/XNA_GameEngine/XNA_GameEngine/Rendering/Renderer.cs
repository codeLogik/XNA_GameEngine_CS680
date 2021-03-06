﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XNA_GameEngine.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_GameEngine.Rendering
{
    class Renderer
    {
        private static Renderer g_Renderer;
        private Color m_backgroundColor;
        private CoreMain m_mainGame;
        private SpriteBatch m_spriteBatch;
        private GraphicsDevice m_graphicsDevice;

        private LinkedList<RenderObject> m_renderObjects;

        public Renderer()
        {
            g_Renderer = null;
            m_backgroundColor = Color.Black;
        }

        public void Initialize(CoreMain mainGame, GraphicsDevice graphicsDevice)
        {
            m_mainGame = mainGame;
            m_graphicsDevice = graphicsDevice;
            m_spriteBatch = new SpriteBatch(m_graphicsDevice);
            m_renderObjects = new LinkedList<RenderObject>();
        }

        public static Renderer GetInstance()
        {
            if (g_Renderer == null)
            {
                g_Renderer = new Renderer();
            }

            return g_Renderer;
        }

        public void AddRenderObject(ICoreComponent coreComponent)
        {
            if (coreComponent != null && coreComponent.GetComponentType() == ICoreComponent.ComponentType.COMPONENT_Rendering)
            {
                // Have the render object load its asset and add it to the render object list.
                RenderObject renderObject = (RenderObject)coreComponent;
                renderObject.LoadAssetContent(m_mainGame);
                m_renderObjects.AddLast(renderObject);
            }
        }

        public void Render()
        {
            // Iterate through all of the render objects queued to be rendered.
            m_mainGame.GraphicsDevice.Clear(m_backgroundColor);

            m_spriteBatch.Begin();
            foreach (RenderObject renderObject in m_renderObjects)
            {
                renderObject.Render(m_spriteBatch);
            }
            m_spriteBatch.End();
        }
    }
}
