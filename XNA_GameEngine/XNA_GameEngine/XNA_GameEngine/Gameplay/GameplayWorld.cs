using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Gameplay
{
    class GameplayWorld
    {
        private Dictionary</*Guid*/int, GameObject> m_gameObjects;
        private Dictionary<int, GameObject> m_playerGO;

        // Simulation tick count.
        UInt64 m_frameNumber;

        // Gameplay world singleton.
        static private GameplayWorld g_GameplayWorld;

        public GameplayWorld()
        {
            m_gameObjects = new Dictionary</*Guid*/int, GameObject>();
            m_playerGO = new Dictionary<int, GameObject>();
        }

        public void Initialize()
        {
            m_frameNumber = 0;
        }

        static public GameplayWorld GetInstance()
        {
            if (g_GameplayWorld == null)
            {
                g_GameplayWorld = new GameplayWorld();
            }
            
            return g_GameplayWorld;
        }

        public UInt64 GetCurrentFrameNumber()
        {
            return m_frameNumber;
        }

        public GameObject GetGameObjectOrNULL(/*Guid*/int goRef)
        {
            // TODO @tom: Make more efficient!
            GameObject go = null;
            if (m_gameObjects.TryGetValue(goRef, out go))
            {
                return go;
            }
            else
            {
                return null;
            }
        }

        public GameObject GetPlayerGameObjectOrNULL(int playerID)
        {
            GameObject go = null;
            if (m_playerGO.TryGetValue(playerID, out go))
            {
                return go;
            }
            else
            {
                return null;
            }
        }

        public void AddGameObject(GameObject go)
        {
            if (go != null)
            {
                //Guid newObjGuid = Guid.NewGuid();
                //m_gameObjects.Add(newObjGuid, go);
                m_gameObjects.Add(go.GetRef(), go);

                // Add components to other modules
                Network.NetworkManager.GetInstance().AddNetworkObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Networking));
                Rendering.Renderer.GetInstance().AddRenderObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Rendering));
                Physics.PhysicsWorld.GetInstance().AddPhysicsObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Physics));

                // Add to the player map if this is a player object.
                if (go.GetPlayerID() >= 0)
                {
                    m_playerGO.Add(go.GetPlayerID(), go);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            m_frameNumber++;
            // Handle updating all game objects registered in the gameplay world.
            List<GameObject> gameObjects = m_gameObjects.Values.ToList();
            foreach (GameObject go in gameObjects)
            {
                go.Update(gameTime);
            }
        }
    }
}
