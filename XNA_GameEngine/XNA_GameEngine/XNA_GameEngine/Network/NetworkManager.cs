using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_GameEngine.Core;
using System.Net;

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
                LinkedList<NetInputState> netInputStates = null;
                networkServer.GetReceivedInputStates(ref netInputStates);


                InputState[] inputStates = new InputState[CoreMain.MAX_PLAYERS];
                // TODO @tom: Should this check have to be here????
                if (netInputStates != null)
                {
                    // Build the player array of network inputs.
                    foreach (NetInputState netInputState in netInputStates)
                    {
                        // If the input state has not been filled in yet for the player, then add it.  Otherwise
                        // accumulate the input together for the multiple input states received for that player.
                        UInt16 playerID = netInputState.m_playerID;
                        if (inputStates[playerID] == null)
                        {
                            inputStates[playerID] = netInputState.m_inputState;
                        }
                        else
                        {
                            inputStates[playerID] = inputStates[playerID] + netInputState.m_inputState;
                        }
                    }
                }

                // Update the network synchronized input with the newly received input states.
                m_netSynchronizedInput.Update(inputStates);
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
                        Debug.DebugTools.Report("[Network] (packet): NetGO Position is x: " + netGOstate.m_position.x + " y: " + netGOstate.m_position.y);
                        netObject.UpdateFromNetwork(netGOstate);
                    }
                }
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
                    goNetObjects[i].Update(null);
                    netGOStates[i] = goNetObjects[i].GetNetState();
                }

                // Queue the new network game state for the network server.
                networkServer.QueueSimulatedGameState(new NetGameState(netGOStates, Gameplay.GameplayWorld.GetInstance().GetCurrentFrameNumber()));
            }
            else if(!CoreMain.isObserver)
            {
                // TODO @tom: Handle sending local player state to the server.
            }
        }

        public NetSynchronizedInput GetNetSynchronizedInputState()
        {
            return m_netSynchronizedInput;
        }
    }
}
