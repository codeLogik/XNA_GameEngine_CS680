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
        private Physics.PhysicsObject m_physicsObject;
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
            m_vPosition = new Vector2(100.0f, 100.0f);

            // Add render component.
            //String assetName = "square";
            //String assetName = "debug_player";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, m_assetName);
            Network.NetworkObject networkObject = new Network.NetworkObject(this);
            
        //    Physics.Colliders.CircleCollider collider = new Physics.Colliders.CircleCollider(m_vPosition, 64.5f);
            Physics.Colliders.SquareCollider collider = new Physics.Colliders.SquareCollider(new Vector2(129.0f));
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, 50.0f, collider);
            m_physicsObject = physicsObject;
            
            // Add components to the game object.
            AddComponent(physicsObject);
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
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(0.0f, -5.0f));
            }
            if (synchronizedInput.IsKeyDown(m_localPlayeID, InputState.KeyboardStates.KEYBOARD_Down))
            {
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(0.0f, 5.0f));
            }
            if (synchronizedInput.IsKeyDown(m_localPlayeID, InputState.KeyboardStates.KEYBOARD_Right))
            {
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(5.0f, 0.0f));
            }
            if (synchronizedInput.IsKeyDown(m_localPlayeID, InputState.KeyboardStates.KEYBOARD_Left))
            {
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(-5.0f, 0.0f));
            }
        }
    }
}
