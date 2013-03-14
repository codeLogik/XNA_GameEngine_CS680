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
        static public bool isServer = false;

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
            NetworkManager.GetInstance().Initialize("localhost", 8888);
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
            Random random = new Random();

            // Set this to change which predefined demo is shown:
            // 0: Gaseous Demo (many small circles)
            // 1: Small mass object hitting large mass object Demo
            // 2: large object squashing small object demo (low Velocity)
            // 3: large object squashing small object demo (medium Velocity)
            // 4: large object squashing small object demo (High Velocity)
            // 5: One Circle object, controllable by arrow keys

            int demoNumber = 5;

            // Gaseous Demo
            if (demoNumber == 0)
            {
                PhysicsWorld.GetInstance().SetGravity(Vector2.Zero);
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = new Vector2((float)(random.NextDouble() * 200), (float)(random.NextDouble() * 200));
                        Debug.CircleObject circleObject = new Debug.CircleObject(new Vector2(51.0f + j * 60, 51.0f + i * 60), 25.0f, 0.0f, 10.0f, velocity);
                        circleObject.Initialize();
                        GameplayWorld.GetInstance().AddGameObject(circleObject);
                    }
                }
            }
            else if (demoNumber == 1)
            {
                PhysicsWorld.GetInstance().SetGravity(Vector2.Zero);
                Vector2 velocity = new Vector2(500.0f, 0.0f);
                Debug.CircleObject circleObject = new Debug.CircleObject(new Vector2(51.0f, 340.0f), 25.0f, 0.0f, (float)(4 * Math.PI * 25.0f * 25.0f), velocity);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
                circleObject = new Debug.CircleObject(new Vector2(480.0f, 340.0f), 200.0f, 0.0f, (float)(4 * Math.PI * 200.0f * 200.0f), Vector2.Zero);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
            }
            else if (demoNumber == 2)
            {
                PhysicsWorld.GetInstance().SetGravity(Vector2.Zero);
                Debug.CircleObject circleObject = new Debug.CircleObject(new Vector2(51.0f, 340.0f), 25.0f, 0.0f, (float)(4 * Math.PI * 25.0f * 25.0f), Vector2.Zero);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
                Vector2 velocity = new Vector2(-50.0f, 0.0f);
                circleObject = new Debug.CircleObject(new Vector2(480.0f, 340.0f), 200.0f, 0.0f, (float)(4 * Math.PI * 200.0f * 200.0f), velocity);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
            }
            else if (demoNumber == 3)
            {
                PhysicsWorld.GetInstance().SetGravity(Vector2.Zero);
                Debug.CircleObject circleObject = new Debug.CircleObject(new Vector2(51.0f, 340.0f), 25.0f, 0.0f, (float)(4 * Math.PI * 25.0f * 25.0f), Vector2.Zero);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
                Vector2 velocity = new Vector2(-200.0f, 0.0f);
                circleObject = new Debug.CircleObject(new Vector2(480.0f, 340.0f), 200.0f, 0.0f, (float)(4 * Math.PI * 200.0f * 200.0f), velocity);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
            }
            else if (demoNumber == 4)
            {
                PhysicsWorld.GetInstance().SetGravity(Vector2.Zero);
                Debug.CircleObject circleObject = new Debug.CircleObject(new Vector2(51.0f, 340.0f), 25.0f, 0.0f, (float)(4 * Math.PI * 25.0f * 25.0f), Vector2.Zero);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
                Vector2 velocity = new Vector2(-500.0f, 0.0f);
                circleObject = new Debug.CircleObject(new Vector2(480.0f, 340.0f), 200.0f, 0.0f, (float)(4 * Math.PI * 200.0f * 200.0f), velocity);
                circleObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(circleObject);
            }
            else if (demoNumber == 5)
            {
                PhysicsWorld.GetInstance().SetGravity(Vector2.Zero);
                Debug.DebugPlayerObject playerObject = new Debug.DebugPlayerObject();
                playerObject.Initialize();
                GameplayWorld.GetInstance().AddGameObject(playerObject);
            }

            Debug.SceneBoundingBoxTop top = new Debug.SceneBoundingBoxTop();
            Debug.SceneBoundingBoxBottom bottom = new Debug.SceneBoundingBoxBottom();
            Debug.SceneBoundingBoxLeft left = new Debug.SceneBoundingBoxLeft();
            Debug.SceneBoundingBoxRight right = new Debug.SceneBoundingBoxRight();

            bottom.Initialize();
            top.Initialize();
            left.Initialize();
            right.Initialize();

            GameplayWorld.GetInstance().AddGameObject(left);
            GameplayWorld.GetInstance().AddGameObject(right);
            GameplayWorld.GetInstance().AddGameObject(bottom);
            GameplayWorld.GetInstance().AddGameObject(top);
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
