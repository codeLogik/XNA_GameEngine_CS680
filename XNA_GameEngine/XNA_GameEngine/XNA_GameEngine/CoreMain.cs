using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XNA_GameEngine.Gameplay;
using XNA_GameEngine.Physics;
using XNA_GameEngine.Network;
using XNA_GameEngine.Rendering;

namespace XNA_GameEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CoreMain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public CoreMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This will include the various singleton managers for the different modules and for
        /// the worlds that the game objects and components will live in.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the singleton component managers and worlds.
            NetworkManager.GetInstance().Initialize("localhost", 8888);
            PhysicsWorld.GetInstance().Initialize();
            GameplayWorld.GetInstance().Initialize();
            Renderer.GetInstance().Initialize(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Game Loop
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //  Update all of the core modules
            GameplayWorld.GetInstance().Update();
            NetworkManager.GetInstance().Update();
            PhysicsWorld.GetInstance().Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is where rendering related calls are fired off.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Have render perform rendering.
            Renderer.GetInstance().Render(gameTime);

            base.Draw(gameTime);
        }
    }
}
