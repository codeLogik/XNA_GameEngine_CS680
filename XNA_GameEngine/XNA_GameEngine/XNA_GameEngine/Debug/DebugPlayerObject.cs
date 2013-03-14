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

        float m_fElasticity;
        Vector2 m_vSize;
        float m_fMass;

        public DebugPlayerObject(Vector2 position, Vector2 size, double rotation, float mass, float elasticity)
        {
            m_vPosition = position;
            m_fRotation = rotation;
            m_vSize = size;
            m_fScale = size / new Vector2(129.0f);
            m_fMass = mass;
            m_fElasticity = elasticity;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_fRotation = 0.0f;
            m_vPosition = new Vector2(100.0f, 100.0f);

            // Add render component.
            String assetName = "square";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
            Physics.Colliders.SquareCollider collider = new Physics.Colliders.SquareCollider(m_vSize);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, m_fMass, collider);
            physicsObject.SetElasticity(m_fElasticity);
            m_physicsObject = physicsObject;
            AddComponent(physicsObject);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the net synch state for input.
            NetSynchronizedInput synchronizedInput = NetworkManager.GetInstance().GetNetSynchronizedInput();
            if(synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Up))
            {
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(0.0f, -5.0f));
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Down))
            {
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(0.0f, 5.0f));
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Right))
            {
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(5.0f, 0.0f));
            }
            if (synchronizedInput.IsLocalKeyDown(InputState.KeyboardStates.KEYBOARD_Left))
            {
                m_physicsObject.SetVelocity(m_physicsObject.GetVelocity() + new Vector2(-5.0f, 0.0f));
            }
        }
    }
}
