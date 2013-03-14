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

        float m_fElasticity;
        Vector2 m_vSize;
        float m_fMass;

        public DebugPlayerObject(Vector2 position, Vector2 size, double rotation, float mass, float elasticity, String assetName)
        {
            m_vPosition = position;
            m_fRotation = rotation;
            m_vSize = size;
            m_fScale = size / new Vector2(129.0f);
            m_fMass = mass;
            m_fElasticity = elasticity;
			m_assetName = assetName;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_fRotation = 0.0f;
            m_vPosition = new Vector2(100.0f, 100.0f);

            // Create render and network components.
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, m_assetName);
            Network.NetworkObject networkObject = new Network.NetworkObject(this);
            
            // Create physics collider and physics object.
            Physics.Colliders.SquareCollider collider = new Physics.Colliders.SquareCollider(new Vector2(129.0f));
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this,m_fMass, collider);
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
