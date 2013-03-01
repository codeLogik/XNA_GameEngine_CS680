using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    [Serializable]
    class NetGOState : NetworkPacket
    {
        public NetworkDataTypes.NVec2 m_position;
        public Guid m_goRef;

        public NetGOState(UInt64 currentFrame)
            : base(currentFrame)
        {
        }
    }
}
