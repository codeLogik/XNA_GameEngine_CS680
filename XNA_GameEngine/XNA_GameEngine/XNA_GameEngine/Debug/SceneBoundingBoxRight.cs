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
    class SceneBoundingBoxRight : GameObject
    {
        private float m_fElasticity;
        public SceneBoundingBoxRight()
        {
            m_fElasticity = 0.0f;
        }

        public void SetElasticity(float elasticity)
        {
            m_fElasticity = elasticity;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_fRotation = 0.0f;
            m_vPosition = new Vector2(0.0f, 0.0f);

            Physics.Colliders.LineCollider collider = new Physics.Colliders.LineCollider(new Vector2(960.0f, 0.0f), new Vector2(960.0f, 680.0f));
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, float.MaxValue, collider);
            physicsObject.SetElasticity(m_fElasticity);
            physicsObject.Immobilize();
            AddComponent(physicsObject);
        }
    }
}
