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

        public DebugPlayerObject()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            m_fScale = 1.0f;
            m_fRotation = 0.0f;
            m_vPosition = new Vector2(4.0f, 4.0f);
            m_fSpeed = 100.0f;

            // Add render component.
            String assetName = "debug_player";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this);
            physicsObject.AddModule(new Physics.Modules.ConstantVelocityModule(new Vector2(0.0f, 10.0f)));
            physicsObject.AddModule(new Physics.Modules.CollisionModule(new Physics.Colliders.CircleCollider(this, new Vector2(0.0f, 0.0f), 10.0f), 0.5f));
            AddComponent(physicsObject);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the net synch state for input.
            NetSynchronizedInput synchronizedInput = NetworkManager.GetInstance().GetNetSynchronizedInput();
            if(synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Up))
            {
                m_vPosition.Y -= m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Down))
            {
                m_vPosition.Y += m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Right))
            {
                m_vPosition.X += m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Left))
            {
                m_vPosition.X -= m_fSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
