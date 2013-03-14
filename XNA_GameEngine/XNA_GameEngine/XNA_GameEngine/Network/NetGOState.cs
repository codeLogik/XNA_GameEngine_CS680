using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    [Serializable]
    class NetGOState : NetworkPacket
    {
        public NetworkDataTypes.NVec2 m_velocity;
        public double m_rotation;
        public /*Guid*/int m_goRef;

        public NetGOState(UInt64 currentFrame)
            : base(currentFrame)
        {
        }
    }
}
