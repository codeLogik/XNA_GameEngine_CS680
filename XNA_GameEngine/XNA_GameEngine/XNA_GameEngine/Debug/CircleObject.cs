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
    class CircleObject : GameObject
    {
        private float m_fRadius;
        private float m_fMass;
        private Vector2 m_vInitialVelocity;

        public CircleObject(Vector2 position, float radius, double rotation, float mass, Vector2 initialVelocity)
        {
            m_vPosition = position;
            m_fRotation = rotation;
            m_fRadius = radius;
            m_fScale = new Vector2(radius / 64.5f);
            m_fMass = mass;
            m_vInitialVelocity = initialVelocity;
        }

        public override void Initialize()
        {
            base.Initialize();

            // Add render component.
            String assetName = "circle";
            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
            Physics.Colliders.CircleCollider collider = new Physics.Colliders.CircleCollider(m_fRadius);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, m_fMass, collider);
            physicsObject.SetVelocity(m_vInitialVelocity);
            AddComponent(physicsObject);
        }
    }
}
