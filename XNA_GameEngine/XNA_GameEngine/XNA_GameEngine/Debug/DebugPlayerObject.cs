using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Network;

namespace XNA_GameEngine.Debug
{
    class DebugPlayerObject : GameObject
    {
        private float m_fSpeed;
        private String m_assetName;

        public DebugPlayerObject(String assetName)
        {
            m_assetName = assetName;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_fScale = 1.0f;
            m_fRotation = 0.0f;
            m_vPosition = new Vector2(4.0f, 4.0f);
            m_fSpeed = 100.0f;

            // Add render component.
            //String assetName = "debug_player";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, m_assetName);
            Network.NetworkObject networkObject = new Network.NetworkObject(this);
            AddComponent(renderObject);
            AddComponent(networkObject);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the net synch state for input.
            NetSynchronizedInput synchronizedInput = NetworkManager.GetInstance().GetNetSynchronizedInputState();
            if(synchronizedInput.IsKeyDown(m_localPlayeID, InputState.KeyboardStates.KEYBOARD_Up))
            {
                m_vPosition.Y -= m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (synchronizedInput.IsKeyDown(m_localPlayeID, InputState.KeyboardStates.KEYBOARD_Down))
            {
                m_vPosition.Y += m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (synchronizedInput.IsKeyDown(m_localPlayeID, InputState.KeyboardStates.KEYBOARD_Right))
            {
                m_vPosition.X += m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (synchronizedInput.IsKeyDown(m_localPlayeID, InputState.KeyboardStates.KEYBOARD_Left))
            {
                m_vPosition.X -= m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
