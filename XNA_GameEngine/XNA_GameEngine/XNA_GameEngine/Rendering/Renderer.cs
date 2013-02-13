using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNA_GameEngine.Rendering
{
    class Renderer
    {
        private static Renderer g_Renderer;
        private Color m_backgroundColor;
        private CoreMain m_mainGame;

        public Renderer()
        {
            g_Renderer = null;
            m_backgroundColor = Color.Black;
        }

        public void Initialize(CoreMain mainGame)
        {
            m_mainGame = mainGame;
        }

        public static Renderer GetInstance()
        {
            if (g_Renderer == null)
            {
                g_Renderer = new Renderer();
            }

            return g_Renderer;
        }

        public void Render(GameTime gameTime)
        {
            // Iterate through all of the render objects queued to be rendered.
            m_mainGame.GraphicsDevice.Clear(m_backgroundColor);
        }
    }
}
