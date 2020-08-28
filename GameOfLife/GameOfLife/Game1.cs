namespace GameOfLife
{
    // Required namespaces
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Main game class
    /// </summary>
    public class Game1 : Game
    {
        #region Private members

        /// <summary>
        /// The graphics device
        /// </summary>
        private GraphicsDeviceManager graphics;
        /// <summary>
        /// The spritebatch
        /// </summary>
        private SpriteBatch spriteBatch;
        /// <summary>
        /// The game backround
        /// </summary>
        private Texture2D Background;

        /// <summary>
        /// Amount of frames
        /// </summary>
        private long Frames = 0;
        /// <summary>
        /// The game time
        /// </summary>
        private double Time = 0;

        /// <summary>
        /// Current map
        /// </summary>
        private int[,] Map;
        /// <summary>
        /// Blocks on the map
        /// </summary>
        private List<Block> Blocks;

        /// <summary>
        /// The previous key down
        /// </summary>
        private KeyboardState PrevKeyDown;
        /// <summary>
        /// Tells if the game is paused
        /// </summary>
        private bool GamePaused = true;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Game1()
        {
            // Set the window title
            this.Window.Title = "Conway's Game Of Life - Paused";
            // Set the mouse visibility
            this.IsMouseVisible = Settings.IsMouseVisible;

            // Create a new grapgics device
            graphics = new GraphicsDeviceManager(this);     
            // Set the content manager's root directory
            Content.RootDirectory = "Content";


            // Set the screen height
            graphics.PreferredBackBufferHeight = Settings.SCREEN_HEIGHT;
            // Set the screen width
            graphics.PreferredBackBufferWidth = Settings.SCREEN_WIDTH;

            // Reset the game board
            ResetBoard();
        }

        /// <summary>
        /// Initialize function
        /// </summary>
        protected override void Initialize()
        {
            // Call the base function
            base.Initialize();
        }

        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set the backround
            Background = Content.Load<Texture2D>("Grid");
        }

        /// <summary>
        /// Game update function
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            // If space button has been pressed...
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !PrevKeyDown.IsKeyDown(Keys.Space))
            {
                // Pause the game if it was running, and unpause if it was running
                GamePaused = !GamePaused;
                // Reset the frame counter if the game was started
                if (!GamePaused) Frames = 0;
            }

            // If the Plus key was pressed...
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus)) 
                // Increase the game speed
                Settings.GAMETIME -= 1;
            
            // If the minus key was pressed...
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) 
                // Decrease the game speed
                Settings.GAMETIME += 1;

            // If the game is paused
            if (GamePaused)
            {
                // If the C key is pressed
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                    // Reset the game
                    ResetBoard();

                // Get the mouse X position on the board
                int x = Mouse.GetState().Position.X / 10 * 10;
                // Get the mouse Y position on the board
                int y = Mouse.GetState().Position.Y / 10 * 10;

                // If the mouse left button was pressed and the mouse is in the map
                if (Mouse.GetState().LeftButton == ButtonState.Pressed 
                    && x >= 0 && x <= Map.Length
                    && y >= 0 && y <= Map.Length) {
                    // Place a 1 at the mouse cordinates in the map
                    Map[y / 10, x / 10] = 1;
                    // Add a new block to that position
                    Blocks.Add(new Block(Content, new Vector2(x, y)));
                }
                // If the mouse right button was pressed and the mouse is in the map
                else if (Mouse.GetState().RightButton == ButtonState.Pressed
                    && x >= 0 && x <= Map.Length
                    && y >= 0 && y <= Map.Length) {
                    // Place a 0 at the mouse cordinates in the map
                    Map[y / 10, x / 10] = 0;
                    // Remove that block at that position
                    Blocks.Remove(Blocks.FirstOrDefault(o => o.BlockPosition == new Vector2(x, y)));
                }
            }
            // Else...
            else
            {
                // If game should update...
                if (Time >= Settings.GAMETIME)
                {
                    // Calculate the new board
                    CalculateNewBoard();
                    // Reset the game time
                    Time = 0;
                    // Increase the frame counter by one
                    Frames++;
                }

                // Increase the game time
                Time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // Update the game title
            this.Window.Title = "Conway's Game Of Life - " + ((GamePaused) ? "Paused" : "Running") + " - Frames: " + Frames;

            // Set the previous key down
            PrevKeyDown = Keyboard.GetState();
            // Call the base function
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw function to draw content to the screen
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();

            spriteBatch.Draw(Background, Vector2.Zero, Color.Black);
            foreach (Block b in Blocks)
                b.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates how the board should be next
        /// </summary>
        private void CalculateNewBoard()
        {
            // Reset the blocks list
            Blocks = new List<Block>();
            // Create a temo board
            int[,] TempBoard = new int[Map.GetLength(0), Map.GetLength(1)];

            // Loop through all spots on the map
            for (int y = 0; y < Map.GetLength(0); y++) // Y position
                for (int x = 0; x < Map.GetLength(1); x++) // X positions
                {
                    // The neighbours count
                    int Neighbours = 0;

                    // Calculate the upper row
                    if ((y - 1 >= 0) && (x - 1 >= 0) && Map[y - 1, x - 1] == 1) Neighbours++;
                    if ((y - 1 >= 0) && Map[y - 1, x] == 1) Neighbours++;
                    if ((y - 1 >= 0) && (x + 1 < Map.GetLength(0)) && Map[y - 1, x + 1] == 1) Neighbours++;

                    // Calculate the middle row
                    if ((x - 1 >= 0) && Map[y, x - 1] == 1) Neighbours++;
                    if ((x + 1 < Map.GetLength(1)) && Map[y, x + 1] == 1) Neighbours++;

                    // Calculate the lower row
                    if ((y + 1 < Map.GetLength(0)) && (x - 1 >= 0) && Map[y + 1, x - 1] == 1) Neighbours++;
                    if ((y + 1 < Map.GetLength(0)) && Map[y + 1, x] == 1) Neighbours++;
                    if ((y + 1 < Map.GetLength(0)) && (x + 1 < Map.GetLength(1)) && Map[y + 1, x + 1] == 1) Neighbours++;

                    // Unpopulate if...
                    if ((Neighbours <= 1 || Neighbours >= 4) && Map[y, x] == 1)
                        // Unpopulate the spot on the map
                        TempBoard[y, x] = 0;

                    // Populate if...
                    if (( (Neighbours == 2 || Neighbours == 3)  && Map[y, x] == 1) ||
                          (Map[y, x]  == 0 && Neighbours == 3)  && Map[y, x] == 0) {
                        // Create a new block and add it to the blocks count
                        Blocks.Add(new Block(Content, new Vector2(10 * x, 10 * y)));
                        // Populate the spot on the map
                        TempBoard[y, x] = 1;
                    }
                }

            // Set the new map
            this.Map = TempBoard;
        }

        /// <summary>
        /// Resets the game board
        /// </summary>
        private void ResetBoard()
        {
            // Clear all blocks
            Blocks = new List<Block>();
            // Create a new map with the size 250x250
            Map = new int[150, 150];

            // Loop through the mao and place zeros in all spots
            for (int y = 0; y < Map.GetLength(0); y++)
                for (int x = 0; x < Map.GetLength(1); x++)
                    Map[y, x] = 0;
        }
    }
}
