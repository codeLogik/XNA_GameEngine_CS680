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

            m_fRotation = 0;
            m_vPosition = new Vector2(300.0f, 100.0f);

            // Add render component.
            String assetName = "square";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
          //  Physics.Colliders.CircleCollider collider = new Physics.Colliders.CircleCollider(64.5f);
            Physics.Colliders.SquareCollider collider = new Physics.Colliders.SquareCollider(new Vector2(129.0f));
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, 50.0f, collider);
            AddComponent(physicsObject);
        }
    }
}
