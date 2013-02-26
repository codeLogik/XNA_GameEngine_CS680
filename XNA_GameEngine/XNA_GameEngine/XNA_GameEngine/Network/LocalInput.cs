using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace XNA_GameEngine.Network
{
    class LocalInput
    {
        private KeyboardState m_keyboardState;
        private KeyboardState m_previousKeyboardState;
        private InputState m_localInputState;

        public LocalInput()
        {
            m_localInputState = new InputState();
        }

        public InputState GetLocalInputState()
        {
            return m_localInputState;
        }

        public void Update()
        {
            // Cache previous state and get new state for this frame.
            m_previousKeyboardState = m_keyboardState;
            m_keyboardState = Keyboard.GetState();

            // TODO @tom: Build local input state
            m_localInputState.m_inputState = 0;
            if (m_keyboardState.IsKeyDown(Keys.Down))
                m_localInputState.m_inputState |= (UInt16)InputState.KeyboardStates.KEYBOARD_Down;
            if (m_keyboardState.IsKeyDown(Keys.Up))
                m_localInputState.m_inputState |= (UInt16)InputState.KeyboardStates.KEYBOARD_Up;
            if (m_keyboardState.IsKeyDown(Keys.Left))
                m_localInputState.m_inputState |= (UInt16)InputState.KeyboardStates.KEYBOARD_Left;
            if (m_keyboardState.IsKeyDown(Keys.Right))
                m_localInputState.m_inputState |= (UInt16)InputState.KeyboardStates.KEYBOARD_Right;
        }

    }
}
