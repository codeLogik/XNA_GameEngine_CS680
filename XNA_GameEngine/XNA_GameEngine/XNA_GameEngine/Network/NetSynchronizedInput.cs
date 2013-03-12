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
        private LocalInput m_previousLocalInput;

        public NetSynchronizedInput()
        {
            m_synchronizedStates = new InputState[XNA_GameEngine.CoreMain.MAX_PLAYERS];
            m_previousSynchronizedStates = new InputState[XNA_GameEngine.CoreMain.MAX_PLAYERS];
            m_localInput = new LocalInput();
            m_previousLocalInput = new LocalInput();
        }

        public void Initialize()
        {
            for (int i = 0; i < XNA_GameEngine.CoreMain.MAX_PLAYERS; i++)
            {
                m_synchronizedStates[i] = new InputState();
            }
        }

        public void UpdateLocalPlayer()
        {
            if (CoreMain.s_localPlayer >= 0)
            {
                m_localInput.Update();
                m_synchronizedStates[XNA_GameEngine.CoreMain.s_localPlayer] = m_localInput.GetLocalInputState();
            }
        }

        public void Update(InputState[] inputStates)
        {
            // Gather the synchronized input of the networked players and the local player.
            m_previousLocalInput = m_localInput;
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
                    m_synchronizedStates[i] = inputStates[i];
                }
            }
        }

        public bool IsKeyDown(int playerID, InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[playerID];

            return (currentInputState.m_inputState & (UInt16)keyState) != 0;
        }

        public bool IsLocalKeyDown(InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[CoreMain.s_localPlayer];

            return (currentInputState.m_inputState & (UInt16)keyState) != 0;
        }

        public bool IsKeyPressed(int playerID, InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[playerID];
            InputState previousInputState = m_previousSynchronizedStates[playerID];

            return ((currentInputState.m_inputState & (UInt16)keyState) != 0) && ((previousInputState.m_inputState & (UInt16)keyState) == 0);
        }

        public bool IsLocalKeyPressed(InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[CoreMain.s_localPlayer];
            InputState previousInputState = m_previousSynchronizedStates[CoreMain.s_localPlayer];

            return ((currentInputState.m_inputState & (UInt16)keyState) != 0) && ((previousInputState.m_inputState & (UInt16)keyState) == 0);
        }

        public bool IsKeyReleased(int playerID, InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[playerID];
            InputState previousInputState = m_previousSynchronizedStates[playerID];

            return ((currentInputState.m_inputState & (UInt16)keyState) == 0) && ((previousInputState.m_inputState & (UInt16)keyState) != 0);
        }

        public bool IsLocalKeyReleased(InputState.KeyboardStates keyState)
        {
            InputState currentInputState = m_synchronizedStates[CoreMain.s_localPlayer];
            InputState previousInputState = m_previousSynchronizedStates[CoreMain.s_localPlayer];

            return ((currentInputState.m_inputState & (UInt16)keyState) == 0) && ((previousInputState.m_inputState & (UInt16)keyState) != 0);
        }
    }
}
