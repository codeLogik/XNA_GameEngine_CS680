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
    class SceneBoundingBoxTop : GameObject
    {
        public override void Initialize()
        {
            base.Initialize();

            m_fScale = 1.0f;
            m_fRotation = 0.0f;
            m_vPosition = new Vector2(0.0f, 0.0f);

            Physics.Colliders.LineCollider collider = new Physics.Colliders.LineCollider(Vector2.Zero, new Vector2(960.0f, 0.0f));
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, 1000000000000.0f, collider);
            physicsObject.SetElasticity(0.8f);
            physicsObject.Immobilize();
            AddComponent(physicsObject);
        }
    }
}
