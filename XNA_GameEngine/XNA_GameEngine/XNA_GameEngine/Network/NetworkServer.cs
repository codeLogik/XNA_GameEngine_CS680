using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    class NetworkServer
    {
        public LinkedList<InputState> m_NetInputStates;
        public LinkedList<NetworkObject> m_networkObjects;
        private int m_port;

        public NetworkServer(int port)
        {
            this.m_port = port;
            m_NetInputStates = new LinkedList<InputState>();
            m_networkObjects = new LinkedList<NetworkObject>();
        }

        public void RunGameLobby()
        {
            // Wait for connections of incoming player and observer requests.  Spawn the server thread as well
        }

        public void RunServer()
        {
            // TODO @tom: This is the loop that runs so long as the server state is not disconnect.
            for (; ; )
            {
                // We need to collect network game states from the UDP socket and accumulate the states into
                // the appropriate buffers for the various systems within the engine to handle.  All of this is
                // done with locking and unlocking the buffers as these are within the critical section.
            }
        }
    }
}
