using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    [Serializable]
    class NetInputState : NetworkPacket
    {
        public UInt16 m_playerID;
        public InputState m_inputState;

        public NetInputState(InputState inputState, UInt16 playerID)
            : base(inputState.m_currentFrameNumber)
        {
            m_playerID = playerID;
            m_inputState = inputState;
        }
    }
}
