using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    [Serializable]
    class NetGameState : NetworkPacket
    {
        public NetGOState[] m_netGOStates;

        public NetGameState(NetGOState[] netGOStates, UInt64 frameNumber)
            : base(frameNumber)
        {
            m_netGOStates = netGOStates;
        }
    }
}
