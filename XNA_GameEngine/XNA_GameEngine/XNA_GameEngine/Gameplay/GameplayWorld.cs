using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Gameplay
{
    class GameplayWorld
    {
        private Dictionary<Guid, GameObject> m_gameObjects;

        // Simulation tick count.
        UInt64 m_frameNumber;

        // Gameplay world singleton.
        static private GameplayWorld g_GameplayWorld;

        public GameplayWorld()
        {
            m_gameObjects = new Dictionary<Guid, GameObject>();
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

        public GameObject GetGameObjectOrNULL(Guid goRef)
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

        public void AddGameObject(GameObject go)
        {
            if (go != null)
            {
                Guid newObjGuid = Guid.NewGuid();
                m_gameObjects.Add(newObjGuid, go);

                // Add components to other modules
                Network.NetworkManager.GetInstance().AddNetworkObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Networking));
                Rendering.Renderer.GetInstance().AddRenderObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Rendering));
                Physics.PhysicsWorld.GetInstance().AddPhysicsObject(go.GetComponentByTypeOrNULL(Core.ICoreComponent.ComponentType.COMPONENT_Physics));
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
