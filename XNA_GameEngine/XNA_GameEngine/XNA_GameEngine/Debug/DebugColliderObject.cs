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
            m_vPosition = new Vector2(300.0f, 300.0f);

            // Add render component.
            String assetName = "debug_player";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this);
            physicsObject.AddModule(new Physics.Modules.CollisionModule(new Physics.Colliders.CircleCollider(this, new Vector2(0.0f,0.0f), 20.0f), 0.5f));
            AddComponent(physicsObject);
        }
    }
}
