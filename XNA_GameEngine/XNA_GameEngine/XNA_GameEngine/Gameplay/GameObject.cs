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
        private Matrix transform;
        private LinkedList<ICoreComponent> m_GOComponents;

        public GameObject()
        {
            m_GOComponents = new LinkedList<ICoreComponent>();
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
