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
    class DebugColliderObject : GameObject
    {
        public override void Initialize()
        {
            base.Initialize();

            m_fScale = 1.0f;
            m_fRotation = 0.0f;
            m_vPosition = new Vector2(150.0f, 0.0f);

            // Add render component.
            String assetName = "debug_player";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
            Physics.Colliders.CircleCollider collider = new Physics.Colliders.CircleCollider(this, 5.0f, 64.5f);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, collider);
            AddComponent(physicsObject);
        }
    }
}
