using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_GameEngine.Core;
using System.Net;

using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Network
{
    class NetworkManager
    {
        private NetworkThread networkThread;

        private Dictionary</*Guid*/int, NetworkObject> m_GONetComponents;
        
        private NetSynchronizedInput m_netSynchronizedInput;

        // Network game states
        
        

        // Network manager singleton.
        private static NetworkManager g_NetworkManager;

        
        public NetworkManager()
        {
            m_GONetComponents = new Dictionary</*Guid*/int, NetworkObject>();
            m_netSynchronizedInput = new NetSynchronizedInput();
            g_NetworkManager = null;
            networkThread = null;
        }

        public void Initialize()
        {
            m_netSynchronizedInput.Initialize();

            // Initialize the network thread for either a server, client, or observer.
            if (CoreMain.isServer)
            {
                networkThread = new NetworkServer();
            }
            else // if(Client)
            {
                networkThread = new NetworkClient();
            }

            networkThread.InitializeThread();
        }

        static public NetworkManager GetInstance()
        {
            if(g_NetworkManager == null)
            {
                g_NetworkManager = new NetworkManager();
            }
            
            return g_NetworkManager;
        }

        public void AddNetworkObject(ICoreComponent networkObject)
        {
            if (networkObject != null && networkObject.GetComponentType() == ICoreComponent.ComponentType.COMPONENT_Networking)
            {
                m_GONetComponents[networkObject.GetParent().GetRef()] = (NetworkObject)networkObject;
            }
        }

        public void Update()
        {
            if (CoreMain.isServer)
            {
                // Get the input states from the server thread and add into array of input states.
                System.Diagnostics.Debug.Assert(networkThread is NetworkServer, "Server expects to have a network server thread!");
                NetworkServer networkServer = (NetworkServer)networkThread;
                LinkedList<NetGOState> netGOStates = null;
                networkServer.GetReceivedGOStates(ref netGOStates);


                //InputState[] inputStates = new InputState[CoreMain.MAX_PLAYERS];
                // TODO @tom: Should this check have to be here????
                if (netGOStates != null)
                {
                    // Build the player array of network inputs.
                    foreach (NetGOState netGOState in netGOStates)
                    {
                        // Get the network object for the netGOState and update it.
                        NetworkObject netObj = m_GONetComponents[netGOState.m_goRef];
                        netObj.UpdateFromNetwork(netGOState);
                    }
                }

                // Update the network synchronized input with the newly received input states.
                //m_netSynchronizedInput.Update(inputStates);
                m_netSynchronizedInput.UpdateLocalPlayer();
            }
            else //if(Client)
            {
                // Get the network game state from the client thread.
                System.Diagnostics.Debug.Assert(networkThread is NetworkClient, "Client expects to have a network client thread!");
                NetworkClient netClient = (NetworkClient)networkThread;

                NetGameState netGameState = netClient.GetNetGameState();

                if (netGameState != null)
                {
                    NetGOState[] netStates = netGameState.m_netGOStates;
                    foreach (NetGOState netGOstate in netStates)
                    {
                        // Update the network object for the go associated with this state.
                        NetworkObject netObject = m_GONetComponents[netGOstate.m_goRef];
                        if (netObject.GetParent().GetPlayerID() == CoreMain.s_localPlayer)
                        {
                            Debug.DebugTools.Report("[Network] (packet): NetGO Position is x: " + netGOstate.m_position.x + " y: " + netGOstate.m_position.y);
                        }
                        netObject.UpdateFromNetwork(netGOstate);
                    }
                }

                // Update the local input state
                m_netSynchronizedInput.UpdateLocalPlayer();
            }
        }

        public void PostUpdate()
        {
            if (CoreMain.isServer)
            {
                // TODO @tom:  Handle sending out
                // Get the input states from the server thread and add into array of input states.
                System.Diagnostics.Debug.Assert(networkThread is NetworkServer, "Server expects to have a network server thread!");
                NetworkServer networkServer = (NetworkServer)networkThread;

                // Handle setting up the simulated NetGameState from the network objects that we are watching.
                List<NetworkObject> goNetObjects = m_GONetComponents.Values.ToList();
                NetGOState[] netGOStates = new NetGOState[goNetObjects.Count];
                for (int i = 0; i < goNetObjects.Count; i++)
                {
                    netGOStates[i] = goNetObjects[i].GetCurrentNetState();
                }

                if (GameplayWorld.GetInstance().GetCurrentFrameNumber() % 10 == 0)
                {
                    // Queue the new network game state for the network server.
                    networkServer.SendState(new NetGameState(netGOStates, Gameplay.GameplayWorld.GetInstance().GetCurrentFrameNumber()));
                }
            }
            else if(!CoreMain.isObserver)
            {
                // TODO @tom: Handle sending local player state to the server.
                GameObject playerGO = GameplayWorld.GetInstance().GetPlayerGameObjectOrNULL(CoreMain.s_localPlayer);
                NetworkObject netObj = (NetworkObject)playerGO.GetComponentByTypeOrNULL(ICoreComponent.ComponentType.COMPONENT_Networking);
                NetGOState netGOstate = netObj.GetCurrentNetState();

                // Send the state to the server
                // Get the network game state from the client thread.
                System.Diagnostics.Debug.Assert(networkThread is NetworkClient, "Client expects to have a network client thread!");
                NetworkClient netClient = (NetworkClient)networkThread;

                netClient.SendState(netGOstate);
                if (netObj.GetParent().GetPlayerID() == CoreMain.s_localPlayer)
                {
                    Debug.DebugTools.Report("[Network] (packet): NetGO sent Position is x: " + netGOstate.m_position.x + " y: " + netGOstate.m_position.y);
                }
            }
        }

        public NetSynchronizedInput GetNetSynchronizedInputState()
        {
            return m_netSynchronizedInput;
        }
    }
}
