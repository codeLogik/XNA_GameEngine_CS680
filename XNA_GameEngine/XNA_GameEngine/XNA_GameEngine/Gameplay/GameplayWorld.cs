using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

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
                g_GameplayWorld = new GameplayWorld();
            }
            
            return g_GameplayWorld;
        }

        public void AddGameObject(GameObject go)
        {
            if (go != null)
            {
                m_gameObjects.AddLast(go);

                // Add components to other modules
                Network.NetworkManager.GetInstance().AddNetworkObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Networking));
                Rendering.Renderer.GetInstance().AddRenderObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Rendering));
                Physics.PhysicsWorld.GetInstance().AddPhysicsObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Physics));
            }
        }

        public void Update(GameTime gameTime)
        {
            // Handle updating all game objects registered in the gameplay world.
            foreach (GameObject go in m_gameObjects)
            {
                go.Update(gameTime);
            }
        }
    }
}
