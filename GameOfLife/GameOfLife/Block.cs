namespace GameOfLife
{
    // Required namespaces
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Block object
    /// </summary>
    public class Block
    {
        /// <summary>
        /// The blocks position
        /// </summary>
        public Vector2 BlockPosition;

        /// <summary>
        /// The blocks texture
        /// </summary>
        private Texture2D BlockTexture;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="content">The content manager to use when loading in the block texture</param>
        /// <param name="pos">The blocks starting position</param>
        public Block(ContentManager content, Vector2 pos)
        {
            // Load in the block's texture
            BlockTexture = content.Load<Texture2D>("Block");
            // Set the block's position
            BlockPosition = pos;
        }

        /// <summary>
        /// Draw function to draw the block on the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the block
            spriteBatch.Draw(BlockTexture, BlockPosition, Color.White);
        }
    }
}
