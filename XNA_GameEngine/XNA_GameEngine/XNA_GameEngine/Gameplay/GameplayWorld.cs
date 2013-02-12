using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Gameplay
{
    class GameplayWorld
    {
        private LinkedList<GameObject> m_gameObjects;
        static private GameplayWorld g_GameplayWorld;

        public GameplayWorld()
        {
            m_gameObjects = new LinkedList<GameObject>();
        }

        public void Initialize()
        {

        }

        static public GameplayWorld GetInstance()
        {
            if (g_GameplayWorld == null)
            {
                return new GameplayWorld();
            }
            else
            {
                return g_GameplayWorld;
            }
        }

        public void Update()
        {
            // Handle updating all game objects registered in the gameplay world.
            foreach (GameObject go in m_gameObjects)
            {
                go.Update();
            }
        }
    }
}
