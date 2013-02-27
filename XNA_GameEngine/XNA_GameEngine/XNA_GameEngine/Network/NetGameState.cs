using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    [Serializable]
    class NetGameState : NetworkPacket
    {
        public NetworkObject[] m_networkObjects;

        public NetGameState(NetworkObject[] networkObjects, UInt64 frameNumber)
            : base(frameNumber)
        {
            m_networkObjects = networkObjects;
        }
    }
}
