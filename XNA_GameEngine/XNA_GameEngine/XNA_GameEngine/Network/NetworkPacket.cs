using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    [Serializable]
    abstract class NetworkPacket
    {
        public byte m_packetType;

        enum PacketType
        {
            PACKET_RequestObserve,
            PACKET_RequestJoin,
            PACKET_InputState,
            PACKET_GameState
        }

        public UInt64 m_currentFrameNumber;

        public NetworkPacket(UInt64 currentFrame)
        {
            m_currentFrameNumber = currentFrame;
        }
    }
}
