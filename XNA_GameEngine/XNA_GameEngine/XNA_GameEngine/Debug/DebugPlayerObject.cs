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
        private Physics.Colliders.ICollider m_collider;

        public DebugPlayerObject()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            m_fScale = 1.0f;
            m_fRotation = 0.0f;
            m_vPosition = new Vector2(0.0f, 0.0f);
            m_fSpeed = 100.0f;

            // Add render component.
            String assetName = "debug_player";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
            Physics.Colliders.CircleCollider collider = new Physics.Colliders.CircleCollider(this, 50.0f, 64.5f);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, collider);
            m_collider = collider;
            AddComponent(physicsObject);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the net synch state for input.
            NetSynchronizedInput synchronizedInput = NetworkManager.GetInstance().GetNetSynchronizedInput();
            if(synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Up))
            {
                m_collider.SetVelocity(m_collider.GetVelocity() + new Vector2(0.0f,-5.0f));
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Down))
            {
                m_collider.SetVelocity(m_collider.GetVelocity() + new Vector2(0.0f, 5.0f));
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Right))
            {
                m_collider.SetVelocity(m_collider.GetVelocity() + new Vector2(5.0f, 0.0f));
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Left))
            {
                m_collider.SetVelocity(m_collider.GetVelocity() + new Vector2(-5.0f, 0.0f));
            }
        }
    }
}
