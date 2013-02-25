using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    class NFSM
    {
        public enum NetworkServerState
        {
            NOT_SERVER,
            SERVER_WAITING_FOR_PLAYERS,
            SERVER_IN_GAME,
            SERVER_DISCONNECTED
        }

        public enum NetworkClientState
        {
            NOT_CLIENT = 0,
            CLIENT_WAITING_FOR_SESSION,
            CLIENT_IN_GAME,
            CLIENT_DISCONNECTED
        }
            
    }
}
