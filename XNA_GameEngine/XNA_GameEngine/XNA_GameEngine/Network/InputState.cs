using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_GameEngine.Network
{
    [Serializable]
    class InputState
    {
        public UInt16 m_inputState;
        public UInt64 m_currentFrameNumber;

        public enum KeyboardStates
        {
            KEYBOARD_Up = (1<<0),
            KEYBOARD_Down = (1<<1),
            KEYBOARD_Left = (1<<2),
            KEYBOARD_Right = (1<<3),
            KEYBOARD_Space = (1<<4),
            KEYBOARD_Enter = (1<<5),
            KEYBOARD_Control = (1<<6)
        }

        public InputState()
        {
            m_inputState = 0;
            m_currentFrameNumber = 0;
        }
    }
}
