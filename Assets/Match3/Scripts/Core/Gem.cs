namespace MiniIT.MATCH3
{
    /// <summary>
    /// Represents a gem placed on the game board.
    /// </summary>
    public class Gem
    {
        /// <summary>
        /// Type of this gem.
        /// </summary>
        private GemType gemType = GemType.Red;

        /// <summary>
        /// Position of this gem on the game board.
        /// </summary>
        private BoardPosition position;

        /// <summary>
        /// Gets the type of this gem.
        /// </summary>
        public GemType GemType
        {
            get
            {
                return gemType;
            }
        }

        /// <summary>
        /// Gets the current position of this gem on the game board.
        /// </summary>
        public BoardPosition Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Creates a gem with the specified type and board position.
        /// </summary>
        /// <param name="gemType">Type of the gem.</param>
        /// <param name="position">Initial position of the gem on the board.</param>
        public Gem(GemType gemType, BoardPosition position)
        {
            this.gemType = gemType;
            this.position = position;
        }

        /// <summary>
        /// Moves the gem to the specified board position.
        /// </summary>
        /// <param name="position">New position of the gem on the board.</param>
        public void MoveTo(BoardPosition position)
        {
            this.position = position;
        }
    }
}