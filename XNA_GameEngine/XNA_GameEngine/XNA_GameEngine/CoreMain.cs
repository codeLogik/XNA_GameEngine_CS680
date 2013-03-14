using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using XNA_GameEngine.Sound;

namespace XNA_GameEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CoreMain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;

        static public int MAX_PLAYERS = 4;
        static public int s_localPlayer = 0;
        
        public CoreMain()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_graphics.PreferredBackBufferHeight = 680;
            m_graphics.PreferredBackBufferWidth = 960;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This will include the various singleton managers for the different modules and for
        /// the worlds that the game objects and components will live in.
        /// </summary>
        protected override void Initialize()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            // Initialize the singleton component managers and worlds.
            NetworkManager.GetInstance().Initialize();
            PhysicsWorld.GetInstance().Initialize();
            PhysicsWorld.GetInstance().SetGravity(new Vector2(0.0f, 100.0f));
            GameplayWorld.GetInstance().Initialize();
            Renderer.GetInstance().Initialize(this, GraphicsDevice);
            SoundManager.GetInstance().Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Debug.DebugPlayerObject playerObject = new Debug.DebugPlayerObject("square");
            Debug.DebugPlayerObject playerObject2 = new Debug.DebugPlayerObject("square2");
            Debug.DebugColliderObject collider = new Debug.DebugColliderObject("circle");
            Debug.DebugColliderObject2 collider2 = new Debug.DebugColliderObject2("circle");
            Debug.SceneBoundingBoxTop top = new Debug.SceneBoundingBoxTop();
            Debug.SceneBoundingBoxBottom bottom = new Debug.SceneBoundingBoxBottom();
            Debug.SceneBoundingBoxLeft left = new Debug.SceneBoundingBoxLeft();
            Debug.SceneBoundingBoxRight right = new Debug.SceneBoundingBoxRight();
            
            // Initialize the new objects
            playerObject.Initialize();
            playerObject2.Initialize();
            collider.Initialize();
            collider2.Initialize();
            bottom.Initialize();
            top.Initialize();
            left.Initialize();
            right.Initialize();

            // Set player ID's for player objects.
            playerObject.SetPlayerID(0);
            playerObject2.SetPlayerID(1);

            // Add game objects to gameplay world.
            GameplayWorld.GetInstance().AddGameObject(playerObject);
            GameplayWorld.GetInstance().AddGameObject(playerObject2);
            GameplayWorld.GetInstance().AddGameObject(collider);
            GameplayWorld.GetInstance().AddGameObject(collider2);
            GameplayWorld.GetInstance().AddGameObject(left);
            GameplayWorld.GetInstance().AddGameObject(right);
            GameplayWorld.GetInstance().AddGameObject(bottom);
            GameplayWorld.GetInstance().AddGameObject(top);

            // Set the starting position for the second player.            
            playerObject2.SetPosition(new Vector2(280.0f, 380.0f));
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
            GameplayWorld.GetInstance().Update(gameTime);
            NetworkManager.GetInstance().Update();
            PhysicsWorld.GetInstance().Update(gameTime);
            SoundManager.GetInstance().Update();

            // Run post update on any modules that need post update called on them.
            NetworkManager.GetInstance().PostUpdate();

            // Update the base XNA framework.
            base.Update(gameTime);
        }

        /// <summary>
        /// This is where rendering related calls are fired off.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Have render perform rendering.
            Renderer.GetInstance().Render();

            base.Draw(gameTime);
        }
    }
}
