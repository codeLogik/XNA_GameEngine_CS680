using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using XNA_GameEngine.Core;
using XNA_GameEngine.Gameplay;

namespace XNA_GameEngine.Rendering
{
    class RenderObject : ICoreComponent
    {
        Texture2D m_sprite;
        float m_fScale;
        double m_fRotation;
        Vector2 m_renderPosition;
        String m_assetName;

        public RenderObject(GameObject parentGO, String assetName)
            : base(parentGO)
        {
            m_fScale = 1.0f;
            m_fRotation = 0.0f;
            m_assetName = assetName;
            m_renderPosition = Vector2.Zero;
            m_Type = ComponentType.COMPONENT_Rendering;
        }

        public void LoadAssetContent(Game game)
        {
            m_sprite = game.Content.Load<Texture2D>(m_assetName);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO @tom: Add logic to convert the world position into render position.
            m_renderPosition = m_ownerGO.GetPosition();
            m_fRotation = m_ownerGO.GetRotation();

            m_fScale = m_ownerGO.GetScale();
        }

        public void Render(SpriteBatch spriteBatch)
        {
            // Draw the sprite to the sprite batch for the renderer.
            spriteBatch.Draw(
                m_sprite,
                m_renderPosition,
                null,
                Color.White,
                (float)m_fRotation,
                new Vector2(m_sprite.Width / 2.0f, m_sprite.Height / 2.0f),
                m_fScale,
                SpriteEffects.None,
                0
                );
        }

    }
}
