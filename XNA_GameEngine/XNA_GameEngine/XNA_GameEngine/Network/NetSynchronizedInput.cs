using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XNA_GameEngine.Network
{
    class NetSynchronizedInput
    {
        private InputState [] m_synchronizedStates;
        private InputState[] m_previousSynchronizedStates;
        private LocalInput m_localInput;

        public NetSynchronizedInput()
        {
            m_synchronizedStates = new InputState[XNA_GameEngine.CoreMain.MAX_PLAYERS];
            m_previousSynchronizedStates = new InputState[XNA_GameEngine.CoreMain.MAX_PLAYERS];
            m_localInput = new LocalInput();
        }

        public void Update(GameTime gameTime)
        {
            // Gather the synchronized input of the networked players and the local player.
            m_localInput.Update();
            for (int i = 0; i < XNA_GameEngine.CoreMain.MAX_PLAYERS; i++)
            {
                // Cache previous synchronized states.
                m_previousSynchronizedStates[i] = m_synchronizedStates[i];
                if (i == XNA_GameEngine.CoreMain.s_localPlayer)
                {
                    // Get the local player input state.
                    m_synchronizedStates[i] = m_localInput.GetLocalInputState();
                }
                else
                {
                    // Gather up the networked player states.  This is a critical section part and must be locked
                    // from the networking threads.  Additionally, there is the possibility that the states have been
                    // gathered asynchronously for several frames due to network latency.  We must or all of the inputs
                    // together to be assured that all input is accounted for.  
                }
            }
        }

        public bool IsKeyDown(int playerID, InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[playerID];

            return (currentInputState.m_inputState & (UInt16)keyState) != 0;
        }

        public bool IsKeyPressed(int playerID, InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[playerID];
            InputState previousInputState = m_previousSynchronizedStates[playerID];

            return ((currentInputState.m_inputState & (UInt16)keyState) != 0) && ((previousInputState.m_inputState & (UInt16)keyState) == 0);
        }

        public bool IsKeyReleased(int playerID, InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[playerID];
            InputState previousInputState = m_previousSynchronizedStates[playerID];

            return ((currentInputState.m_inputState & (UInt16)keyState) == 0) && ((previousInputState.m_inputState & (UInt16)keyState) != 0);
        }
    }
}
