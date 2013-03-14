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
    class SquareObject : GameObject
    {
        private Vector2 m_vSize;
        private float m_fMass;
        private Vector2 m_vInitialVelocity;
        private float m_fElasticity;
        private Color m_color;

        public SquareObject(Vector2 position, Vector2 size, double rotation, float mass, float elasticity, Vector2 initialVelocity, Color color)
        {
            m_vPosition = position;
            m_fRotation = rotation;
            m_vSize = size;
            m_fScale = size / new Vector2(129.0f);
            m_fMass = mass;
            m_vInitialVelocity = initialVelocity;
            m_fElasticity = elasticity;
            m_color = color;
        }

        public override void Initialize()
        {
            base.Initialize();

            String assetName = "square";
            if (m_color == Color.Red)
            {
                assetName = "red_square";
            }

            Rendering.RenderObject renderObject = new Rendering.RenderObject(this, assetName);
            AddComponent(renderObject);
            Physics.Colliders.SquareCollider collider = new Physics.Colliders.SquareCollider(m_vSize);
            Physics.PhysicsObject physicsObject = new Physics.PhysicsObject(this, m_fMass, collider);
            physicsObject.SetVelocity(m_vInitialVelocity);
            AddComponent(physicsObject);
        }
    }
}
