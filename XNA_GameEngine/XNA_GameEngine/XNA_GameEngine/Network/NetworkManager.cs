﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA_GameEngine.Core;

namespace XNA_GameEngine.Network
{
    class NetworkManager
    {
        private String m_serverURL;
        private int m_port;
        private Dictionary<Guid, NetworkObject> m_GONetComponents;
        private NetSynchronizedInput m_netSynchronizedInput;

        static NetworkManager g_NetworkManager;

        public NetworkManager()
        {
            m_serverURL = null;
            m_port = 0;
            m_GONetComponents = new Dictionary<Guid, NetworkObject>();
            m_netSynchronizedInput = new NetSynchronizedInput();
            g_NetworkManager = null;
        }

        public void Initialize(String serverURL, int port)
        {
            m_serverURL = serverURL;
            m_port = port;
            m_netSynchronizedInput.Initialize();
        }

        static public NetworkManager GetInstance()
        {
            if(g_NetworkManager == null)
            {
                g_NetworkManager = new NetworkManager();
            }
            
            return g_NetworkManager;
        }

        public NetSynchronizedInput GetNetSynchronizedInput()
        {
            return m_netSynchronizedInput;
        }

        public void AddNetworkObject(ICoreComponent networkObject)
        {
            if (networkObject != null && networkObject.GetComponentType() == ICoreComponent.ComponentType.COMPONENT_Networking)
            {
                m_GONetComponents[networkObject.GetParent().GetRef()] = (NetworkObject)networkObject;
            }
        }

        public void StartServerLobby()
        {
            // Starts the thread for listening on requests to join lobby or observe game.
        }

        public void RequestJoinSession(/*IP Address*/)
        {
            // Query the server at the IP address to join the session.
        }

        public void StartServer()
        {
            // Initialize and start the server thread as well as the lobby thread.
        }

        public void StartClient()
        {

        }

        public void Update()
        {
            // Ticked once per frame.  Handle processing of the data from Send/Receive packet.
            m_netSynchronizedInput.Update();
        }
    }
}
