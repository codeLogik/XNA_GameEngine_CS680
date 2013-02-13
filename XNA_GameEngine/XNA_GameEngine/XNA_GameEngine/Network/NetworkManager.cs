using System;
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
        private LinkedList<ICoreComponent> m_GONetComponents;

        static NetworkManager g_NetworkManager;

        public NetworkManager()
        {
            m_serverURL = null;
            m_port = 0;
            m_GONetComponents = new LinkedList<ICoreComponent>();
            g_NetworkManager = null;
        }

        public void Initialize(String serverURL, int port)
        {
            m_serverURL = serverURL;
            m_port = port;
        }

        static public NetworkManager GetInstance()
        {
            if(g_NetworkManager == null)
            {
                g_NetworkManager = new NetworkManager();
            }
            
            return g_NetworkManager;
        }

#if Server
        void ReceivePacket()
        {
            // Receive buffer and process
        }
#else
        private void SendPacket(String buffer)
        {
            // Send the data off to the server
        }
#endif

        public void Update()
        {
            // Ticked once per frame.  Handle processing of the data from Send/Receive packet.
        }
    }
}
