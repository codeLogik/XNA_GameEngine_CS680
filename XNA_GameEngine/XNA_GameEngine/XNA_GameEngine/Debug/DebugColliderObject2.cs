﻿using System;
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
    class DebugColliderObject2 : GameObject
    {
        private String m_assetName;

        public DebugColliderObject2(String assetName)
        {
            m_assetName = assetName;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_fRotation = 0.0f;
            m_vPosition = new Vector2(300.0f, 250.0f);

            // Create render, networking, collision, and physics components.
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, m_assetName);
            Network.NetworkObject networkObject = new Network.NetworkObject(this);
            Physics.Colliders.CircleCollider collider = new Physics.Colliders.CircleCollider(64.5f);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, 10.0f, collider);
            
            // Add components
            AddComponent(physicsObject);
            AddComponent(renderObject);
            AddComponent(networkObject);
        }
    }
}
