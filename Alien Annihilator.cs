using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;


namespace Alien_Annihilator
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        // -- SOUND ---
        // enemy spawn sound
        SoundEffect enemySpawnSound;
        SoundEffect BackgroundSound;
        
        
        // --- GRAPHICS ---
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle safeBounds;
        const float safeAreaPortion = 0.05f;
     
        // --- CHARACTER ---
        // Used for character sprite
     
        // Used to store current character position
        Vector2 PlayerShipPosition;
        // Used to calculate next position of character given user input
        const int characterSpeed = 5;
        // Used to state character has collided withe enemy
        bool characterHit = false;
        bool characterDirectionLeft = false;

        // --- ENEMIES ---
        // Used for enemy sprite
        // The list of enemies currently in the game
        List<Vector2> enemyPositions = new List<Vector2>();
        List<Vector2> enemyDirections = new List<Vector2>();

        // --- FIRE ---
        List<Vector2> firePositions = new List<Vector2>();
        List<Vector2> fireDirections = new List<Vector2>();
        Texture2D fireTexture;
        float fireSpeed = 10.0f;

        // Probability that an enemy will be spawned
        float enemySpawnProbability = 0.15f;
        bool spawnFromLeft = false;
    
        // Speed of enemey
        const int enemySpeed = 10;
        //Used to generate random colours
        Random random = new Random();
        // Current enemy colour
        Color enemyColor = Color.White;
        bool enemyHit;
       

        // --- IN GAME INFORMATION ---
        // Player score
        long highScore;
        long score;
  
     
       
        // Font used to display score
        SpriteFont scoreFont;
        //Used to hold current on-screen information
       String scoreText = "SCORE: 0";
        String highScoreText = "HI: 0";


        Texture2D GameBackgroundTexture;
        Texture2D GameMenuTexture;
        Texture2D PauseMenuTexture; 
        Texture2D LoadingMenuTexture;
        Texture2D ObjectivesMenuTexture;
        Texture2D GameOverMenuTexture;
        Texture2D Asteroid1Texture;
        Texture2D Asteroid2Texture;
        Texture2D PlayerShipTexture;
        Texture2D MainspaceshipspriteTexture;
        Texture2D PlayerShip2Texture;
        Texture2D PlayerShip3Texture;
        Texture2D Background3Texture;
        Texture2D Background4Texture;
        Texture2D Background5Texture;
        Texture2D GameBackground2Texture;

        // -- GAME STATE ---
        enum GameState
        {
            GameMenu = 0,
            GamePlay = 1,
            GameOver = 2,
            GamePause = 3,
            ObjectivesMenu = 4,
            GameBackground = 5,
            GameBackground2 = 6,
            Asteroid1 = 7,
            Asteroid2 = 8,
            PlayerShip = 9,
            Mainspaceship = 10,
            Playership2 = 11,
            Background3Menu = 12,
            Background4Menu = 13,
            PlayerShip3 = 14,
            PlayerShip4 = 15,

        }
        GameState currentGameState;


        // LEVELS
        int levelNumber = 1;
        int enemyHitCount = 0;

        /// <summary>
        /// Constructor for Game
        /// This is called when the Game object is created.
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
          //  graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            

            // Calculate safe bounds based on current resolution
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            safeBounds = new Rectangle(
                (int)(viewport.Width * safeAreaPortion),
                (int)(viewport.Height * safeAreaPortion),
                (int)(viewport.Width * (1 - 2 * safeAreaPortion)),
                (int)(viewport.Height * (1 - 2 * safeAreaPortion)));

            // Start the player in the center along the bottom of the screen
            PlayerShipPosition.X = (safeBounds.Width - PlayerShipTexture.Width) / 2;
            PlayerShipPosition.Y = safeBounds.Height - PlayerShipTexture.Height;


            // Reset score
            score = 0;
            highScore = 0;


            // Set the initial game state
            currentGameState = GameState.GameMenu;
            BackgroundSound.Play();

        }
        ///<summary>
        /// LoadContent will be called once per game and is the place to loas
        /// all of your content.
        /// </summary>




        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load textures

     
            // create the sound effect
            enemySpawnSound = Content.Load<SoundEffect>("Fire");
            BackgroundSound = Content.Load<SoundEffect>("Background Sound");

            // Game Menus
            GameMenuTexture = Content.Load<Texture2D>("Game Menu");
            PauseMenuTexture = Content.Load<Texture2D>("Pause Menu");
            LoadingMenuTexture = Content.Load<Texture2D>("Loading Menu");
            ObjectivesMenuTexture = Content.Load<Texture2D>("Objectives Menu");
            GameOverMenuTexture = Content.Load<Texture2D>("Game Over");

            // Fire 
            fireTexture = Content.Load<Texture2D>("shoot");        

            // Player 
            PlayerShipTexture = Content.Load<Texture2D>("Player Ship");

            // Enemies     
            Asteroid1Texture = Content.Load<Texture2D>("Asteroid 1");
            Asteroid2Texture = Content.Load<Texture2D>("Asteroid 2");          
            MainspaceshipspriteTexture = Content.Load<Texture2D>("Main spaceship sprite");
            PlayerShip2Texture = Content.Load<Texture2D>("Player Ship 2");
            PlayerShip3Texture = Content.Load<Texture2D>("Player Ship 3");

            // Game Backgrounds
            GameBackgroundTexture = Content.Load<Texture2D>("Game Background");
            GameBackground2Texture = Content.Load<Texture2D>("Game Background 2");
            Background3Texture = Content.Load<Texture2D>("Background 3");
            Background4Texture = Content.Load<Texture2D>("Background 4");
            Background5Texture = Content.Load<Texture2D>("Background 5");
            
            // create the font
            scoreFont = Content.Load<SpriteFont>("scoreFontDescription");
        }

        ///<summary>
        ///Allows the game to run logic such as updating the world,
        ///checking for collisions, gathering input, and playing audio.
        ///</summary>
        ///<param name="GameTime">Provides a snapshot of timing values</param>
        protected override void Update(GameTime gameTime)
        {

            //Get input
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            // use game state
            switch (currentGameState)
            {

                case GameState.ObjectivesMenu:



                    // Allows the game to exit
                    if (gamePad.Buttons.Back == ButtonState.Pressed ||
                        keyboard.IsKeyDown(Keys.Back))
                    {
                        this.Exit();
                    }
                    if (gamePad.Buttons.Start == ButtonState.Pressed ||
                        keyboard.IsKeyDown(Keys.M))
                    {
                        // Start the player in the center along the bottom of the screen
                        PlayerShipPosition.X = (safeBounds.Width - PlayerShipTexture.Width) / 2;
                        PlayerShipPosition.Y = safeBounds.Height - PlayerShipTexture.Height;

                        // Set the game state to play
                        currentGameState = GameState.GameMenu;

                        // Rest score
                        score = 0;
                        highScore = 0;
                    }
                    break;
                    
                case GameState.GameMenu:
                    // Allows return to main menu
                    if (gamePad.Buttons.Start == ButtonState.Pressed ||
                         keyboard.IsKeyDown(Keys.S))
                    {





                        levelNumber = 1;
                        enemyHitCount = 0;
                        // Start the player in the center along the bottom of the screen
                        PlayerShipPosition.X = (safeBounds.Width - PlayerShipTexture.Width) / 2;
                        PlayerShipPosition.Y = safeBounds.Height - PlayerShipTexture.Height;
                        score = 0;
                

                        currentGameState = GameState.GamePlay;
                    }

                    // Allows return to main menu
                    if (gamePad.Buttons.Y == ButtonState.Pressed ||
                         keyboard.IsKeyDown(Keys.O))
                    {
                        currentGameState = GameState.ObjectivesMenu;
                    }

                    if (gamePad.Buttons.X == ButtonState.Pressed ||
                         keyboard.IsKeyDown(Keys.E))
                    {
                        Exit();
                    }


                    break;


                case GameState.GameOver:
                    // Allows return to main menu
                    if (gamePad.Buttons.B == ButtonState.Pressed ||
                         keyboard.IsKeyDown(Keys.M))
                    {
                        currentGameState = GameState.GameMenu;
                    }

                    break;


                case GameState.GamePause:
                    // Allows return to main menu
                    if (gamePad.Buttons.B == ButtonState.Pressed ||
                         keyboard.IsKeyDown(Keys.C))
                    {
                        currentGameState = GameState.GamePlay;
                    }
                  


                    break;


                case GameState.GamePlay:

                    // Allows the game to exit
                    if (gamePad.Buttons.Y == ButtonState.Pressed || keyboard.IsKeyDown(Keys.P))
                    {
                        currentGameState = GameState.GamePause;
                    }
                 
                   

                    //Move the player left and right with arrow keys or d-pad
                    if (keyboard.IsKeyDown(Keys.Left) || gamePad.DPad.Left == ButtonState.Pressed)
                    {
                        PlayerShipPosition.X -= characterSpeed;
                        characterDirectionLeft = true;
                    }
                    if (keyboard.IsKeyDown(Keys.Right) || gamePad.DPad.Right == ButtonState.Pressed)
                    {
                        PlayerShipPosition.X += characterSpeed;
                        characterDirectionLeft = false;
                    }

                    //Move the player up and down with arrow keys or d-pad
                    if (keyboard.IsKeyDown(Keys.Down) || gamePad.DPad.Down == ButtonState.Pressed)
                    {
                        PlayerShipPosition.Y += characterSpeed;
                    }
                    if (keyboard.IsKeyDown(Keys.Up) || gamePad.DPad.Up == ButtonState.Pressed)
                    {
                        PlayerShipPosition.Y -= characterSpeed;
                    }

                    if (keyboard.IsKeyDown(Keys.F) || gamePad.Buttons.A == ButtonState.Pressed)
                    {
                        enemySpawnSound.Play();
                        firePositions.Add(new Vector2(PlayerShipPosition.X, PlayerShipPosition.Y));
                        if (characterDirectionLeft)
                        {
                            fireDirections.Add(new Vector2(-fireSpeed, 0.0f));
                        }
                        else
                        {
                            fireDirections.Add(new Vector2(fireSpeed, 0.0f));
                        }
                    }
                    


                    // Prevent the character from moving off of the screen
                    PlayerShipPosition.X = MathHelper.Clamp(PlayerShipPosition.X,
                          safeBounds.Left, safeBounds.Right - PlayerShipTexture.Width);
                    // Prevent the character from moving off of the screen
                    PlayerShipPosition.Y = MathHelper.Clamp(PlayerShipPosition.Y,
                          safeBounds.Top, safeBounds.Bottom - PlayerShipTexture.Height);

                    // Get the bounding rectangle of the character
                    Rectangle characterRectangle =
                    new Rectangle((int)PlayerShipPosition.X, (int)PlayerShipPosition.Y,
                        PlayerShipTexture.Width, PlayerShipTexture.Height);
                    if (gamePad.Buttons.X == ButtonState.Pressed || keyboard.IsKeyDown(Keys.M))
                    {
                        currentGameState = GameState.GameMenu;
                    }

                    // Spawn new enemy
                    if (random.NextDouble() < enemySpawnProbability)
                    {
                        //float x = (float)random.NextDouble() *
                        //    (Window.ClientBounds.Width - enemyTexture.Width);
                        float y = (float)random.NextDouble() *
                            (Window.ClientBounds.Height - Asteroid1Texture.Height);

                        if (spawnFromLeft)
                        {
                            if (levelNumber == 1)
                                enemyPositions.Add(new Vector2(-Asteroid1Texture.Width, y));

                            else if (levelNumber == 2)
                                enemyPositions.Add(new Vector2(-Asteroid2Texture.Width, y));

                            else if (levelNumber == 3)
                                enemyPositions.Add(new Vector2(-MainspaceshipspriteTexture.Width, y));

                            else if (levelNumber == 4)
                                enemyPositions.Add(new Vector2(-PlayerShip2Texture.Width, y));
                            else if (levelNumber == 5)
                                enemyPositions.Add(new Vector2(-PlayerShip3Texture.Width, y));
                            else 
                                enemyPositions.Add(new Vector2(-PlayerShip2Texture.Width, y));

                            enemyDirections.Add(new Vector2(enemySpeed, 0.0f));
                            enemyDirections.Add(new Vector2(-enemySpeed, 6.5f));
                            enemyDirections.Add(new Vector2(-enemySpeed, -6.5f));
                            spawnFromLeft = false;
                        }
                        else
                        {
                            if (levelNumber == 1)
                                enemyPositions.Add(new Vector2(Window.ClientBounds.Width + Asteroid1Texture.Width, y));
                            else if (levelNumber == 2)
                                enemyPositions.Add(new Vector2(Window.ClientBounds.Width + Asteroid2Texture.Width, y));
                            else if (levelNumber == 3)
                                enemyPositions.Add(new Vector2(Window.ClientBounds.Width + MainspaceshipspriteTexture.Width, y));
                            else if (levelNumber == 4)
                                enemyPositions.Add(new Vector2(Window.ClientBounds.Width + PlayerShip2Texture.Width, y));
                            else if (levelNumber == 5)
                                enemyPositions.Add(new Vector2(Window.ClientBounds.Width + PlayerShip3Texture.Width, y));
                            else
                                enemyPositions.Add(new Vector2(Window.ClientBounds.Width + Asteroid1Texture.Width, y));

                            enemyDirections.Add(new Vector2(-enemySpeed, 0.0f));
                            enemyDirections.Add(new Vector2(enemySpeed, 6.5f));
                            spawnFromLeft = true;
                        }

                    }
                 

                    // Update each shot
                    enemyHit = false;
                    for (int i = 0; i < firePositions.Count; i++)

                    {
                        // Animate this bullet
                        firePositions[i] =
                            new Vector2(firePositions[i].X + fireDirections[i].X,
                            firePositions[i].Y + fireDirections[i].Y);
                        
                        // Get the bounding rectangle of this shot
                        Rectangle fireRectangle =
                            new Rectangle((int)firePositions[i].X, (int)firePositions[i].Y,
                                fireTexture.Width, fireTexture.Height);

                        for (int j = 0; j < enemyPositions.Count; j++)
                        {
                            // GEt the bounding rectangle of this enemy
                            Rectangle enemyRectangle =
                                new Rectangle((int)enemyPositions[j].X, (int)enemyPositions[j].Y,
                                    Asteroid1Texture.Width, Asteroid1Texture.Height);

                            // Check collisions
                            if (enemyRectangle.Intersects(fireRectangle))
                            {
                                enemyPositions.RemoveAt(j);
                                enemyDirections.RemoveAt(j);
                                enemyHit = true;

                                enemyHitCount++;
                                levelNumber = (int)(enemyHitCount / 05) + 1;
                               
                                j--;
                                break;
                            }
                        }

                        if (enemyHit)
                        {
                            firePositions.RemoveAt(i);
                            fireDirections.RemoveAt(i);
                            i--;
                        }
                        // Remove this enemy if it has fallen off the screen
                        else if (firePositions[i].X < 0 || firePositions[i].X > Window.ClientBounds.Width)
                        {
                            firePositions.RemoveAt(i);
                            fireDirections.RemoveAt(i);
                            // When removing an enemy, the next enemy will have the same index
                            // as the current enemy. Decrement i to prevent skipping as enemy.
                            i--;
                        }
                    }

                    // Update each enemy
                    characterHit = false;
                    for (int i = 0; i < enemyPositions.Count; i++)
                    {
                        // Animate this enemy
                        enemyPositions[i] =
                            new Vector2(enemyPositions[i].X + enemyDirections[i].X,
                            enemyPositions[i].Y + enemyDirections[i].Y);

                        // GEt the bounding rectangle of this enemy
                        Rectangle enemyRectangle =
                            new Rectangle((int)enemyPositions[i].X, (int)enemyPositions[i].Y,
                                Asteroid1Texture.Width, Asteroid1Texture.Height);

                        // Check collision with character
                        if (characterRectangle.Intersects(enemyRectangle))
                            characterHit = true;
                        // Remove this enemy if it has fallen off the screen
                        if (enemyPositions[i].Y > Window.ClientBounds.Height)
                        {
                            enemyPositions.RemoveAt(i);
                            enemyDirections.RemoveAt(i);
                            // When removing an enemy, the next enemy will have the same index
                            // as the current enemy. Decrement i to prevent skipping as enemy.
                        }
                    }


                    //this.graphics.IsFullScreen = true;
                    //this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    //this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                    // Rest game if character has been hit
                    if (characterHit)
                    {
                        // check for highscore
                     
                        if (score > highScore)
                            highScore = score;
                       

                       
                        // reset score to zero
                        score = 0;
                        


                        // empty the enemies list
                        enemyPositions = new List<Vector2>();
                        enemyDirections = new List<Vector2>();



                        currentGameState = GameState.GameOver;

                    }
                    else
                    {

                       //  update score (character has not been hit)
                        score = score + 1;
                       
                    }
                    // update on-screen information variables
                    scoreText = "SCORE: " + score.ToString();
                  highScoreText = "HI: " + highScore.ToString();

                    break;
            }

            base.Update(gameTime);
        }

        ///<summary>
        ///This is called when the game should draw itself
        ///</summary>
        ///<param name=" gameTime">Provides a snapshot of taking values</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            new Vector2(0, 0);


            // use game state
            switch (currentGameState)
            {
                case GameState.GameMenu:
                    // add code to draw game menu screen
                    // 'Open' the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    spriteBatch.Draw(GameMenuTexture, new Vector2(0, 0), Color.White);

                    // 'Close' the sprite batch
                    spriteBatch.End();
                    break;

                case GameState.GamePause:
                    // add code to draw game menu screen
                    // 'Open' the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    spriteBatch.Draw(PauseMenuTexture, new Vector2(0, 0), Color.White);

                    // 'Close' the sprite batch
                    spriteBatch.End();
                    break;

                case GameState.GameBackground:
                    // add code to draw game menu screen
                    // 'Open' the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    spriteBatch.Draw(GameBackgroundTexture, new Vector2(0, 0), Color.White);

                    // 'Close' the sprite batch
                    spriteBatch.End();
                    break;

                case GameState.GameBackground2:
                    // add code to draw game menu screen
                    // 'Open' the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    spriteBatch.Draw(GameBackground2Texture, new Vector2(0, 0), Color.White);

                    // 'Close' the sprite batch
                    spriteBatch.End();
                    break;



                case GameState.GameOver:
                    // add code to draw game over screen
                    // 'Open' the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    spriteBatch.Draw(GameOverMenuTexture, new Vector2(0, 0), Color.White);

                    // 'Close' the sprite batch
                    spriteBatch.End();
                    break;

                case GameState.ObjectivesMenu:
                    // add code to draw game over screen
                    // 'Open' the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    spriteBatch.Draw(ObjectivesMenuTexture, new Vector2(0, 0), Color.White);

                    // 'Close' the sprite batch
                    spriteBatch.End();
                    break;

              

                case GameState.Background3Menu:
                    // add code to draw game over screen
                    // 'Open' the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    spriteBatch.Draw(Background3Texture, new Vector2(0, 0), Color.White);

                    // 'Close' the sprite batch
                    spriteBatch.End();
                    break;



                case GameState.GamePlay:

                    //// 'Open the sprite batch
                    //spriteBatch.Begin();

                    //// Draw character
                    //spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
                    //spriteBatch.Draw(characterTexture, characterPosition, Color.White);



                    //// Draw enemies
                    //foreach (Vector2 enemyPosition in enemyPositions)
                    //    spriteBatch.Draw(enemyTexture, enemyPosition, enemyColor);

                    //// Draw Fire
                    //foreach (Vector2 firePosition in firePositions)
                    //    spriteBatch.Draw(fireTexture, firePosition, enemyColor);


                    //// Draw on-screen game inforamtion
                    //spriteBatch.DrawString(scoreFont, scoreText, new Vector2(30, 30), Color.Black);
                    //spriteBatch.DrawString(scoreFont, highScoreText,
                    //new Vector2((float)safeBounds.Right - scoreFont.MeasureString(highScoreText).X,
                    //30), Color.Black);
                    //spriteBatch.DrawString(scoreFont, gameName,
                    //new Vector2((float)safeBounds.Right / 2 - scoreFont.MeasureString(gameName).X / 2,
                    //30), Color.Black);

                    //// 'Close' the sprite batch
                    //spriteBatch.End();

                    // 'Open the sprite batch
                    spriteBatch.Begin();

                    // Draw background
                    if (levelNumber == 1)
                        spriteBatch.Draw(GameBackgroundTexture, new Vector2(0, 0), Color.White);
                    else if (levelNumber == 2)
                        spriteBatch.Draw(GameBackground2Texture, new Vector2(0, 0), Color.White);
                    else if (levelNumber == 3)
                        spriteBatch.Draw(Background3Texture, new Vector2(0, 0), Color.White);
                    else if (levelNumber == 4)
                        spriteBatch.Draw(Background4Texture, new Vector2(0, 0), Color.White);
                    else if (levelNumber == 5)
                        spriteBatch.Draw(Background5Texture, new Vector2(0, 0), Color.White);
                    else
                        spriteBatch.Draw(GameBackgroundTexture, new Vector2(0, 0), Color.White);


                    if (characterDirectionLeft)
                        spriteBatch.Draw(PlayerShipTexture, PlayerShipPosition, null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                    else
                        spriteBatch.Draw(PlayerShipTexture, PlayerShipPosition, Color.White);

                    foreach (Vector2 firePosition in firePositions)
                        spriteBatch.Draw(fireTexture, firePosition, Color.White);

                    // Draw enemies
                    for (int i = 0; i < enemyPositions.Count; i++)
                    {

                        bool flip = enemyDirections[i].X > 0;
                        SpriteEffects sf;
                        if (flip)
                            sf = SpriteEffects.None;
                        else
                            sf = SpriteEffects.FlipHorizontally;

                        if (levelNumber == 1)
                            spriteBatch.Draw(Asteroid1Texture, enemyPositions[i], null, enemyColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, sf, 0.0f);
                        if (levelNumber == 2)
                            spriteBatch.Draw(Asteroid2Texture, enemyPositions[i], null, enemyColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, sf, 0.0f);
                        if (levelNumber == 3)
                            spriteBatch.Draw(MainspaceshipspriteTexture, enemyPositions[i], null, enemyColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, sf, 0.0f);
                        if (levelNumber == 4)
                            spriteBatch.Draw(PlayerShip2Texture, enemyPositions[i], null, enemyColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, sf, 0.0f);
                        if (levelNumber == 5)
                            spriteBatch.Draw(PlayerShip3Texture, enemyPositions[i], null, enemyColor, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, sf, 0.0f);
                        

                    }

             //        Draw on-screen game inforamtion
                    spriteBatch.DrawString(scoreFont, scoreText, new Vector2(30, 30), Color.Red);
                  spriteBatch.DrawString(scoreFont, highScoreText,
                    new Vector2((float)safeBounds.Right - scoreFont.MeasureString(highScoreText).X,
                   30), Color.Red);


                    // 'Close' the sprite batch
                    spriteBatch.End();

                    break;
            }
            base.Draw(gameTime);
        }

    }
}


