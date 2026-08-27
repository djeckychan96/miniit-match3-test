using System;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Represents the logical state of the match-3 game board.
    /// </summary>
    public class Board
    {
        private int width = 0;
        private int height = 0;

        /// <summary>
        /// Gets the gem storage and creates it on first access.
        /// </summary>
        private Gem[,] Gems
        {
            get
            {
                if (gems == null)
                {
                    gems = new Gem[width, height];
                }

                return gems;
            }
        }

        /// <summary>
        /// Determines whether the specified position is inside the board bounds.
        /// </summary>
        /// <param name="position">Board position to validate.</param>
        /// <returns>True if the position is inside the board.</returns>
        public bool IsInside(BoardPosition position)
        {
            return position.Column >= 0 &&
                position.Column < width &&
                position.Row >= 0 &&
                position.Row < height;
        }

        /// <summary>
        /// Returns the gem placed at the specified board position.
        /// </summary>
        /// <param name="position">Position of the gem on the board.</param>
        /// <returns>Gem at the specified position, or null if the cell is empty.</returns>
        public Gem GetGem(BoardPosition position)
        {
            if (!IsInside(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position is outside the board bounds.");
            }

            return Gems[position.Column, position.Row];
        }

        /// <summary>
        /// Places a gem at the specified board position.
        /// </summary>
        /// <param name="position">Position on the game board.</param>
        /// <param name="gem">Gem to place, or null to clear the cell.</param>
        public void SetGem(BoardPosition position, Gem gem)
        {
            if (!IsInside(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position is outside the board bounds.");
            }

            Gems[position.Column, position.Row] = gem;

            if (gem != null)
            {
                gem.MoveTo(position);
            }
        }

        /// <summary>
        /// Swaps gems located at two adjacent board positions.
        /// </summary>
        /// <param name="firstPosition">First position to swap.</param>
        /// <param name="secondPosition">Second position to swap.</param>
        public void SwapGems(BoardPosition firstPosition, BoardPosition secondPosition)
        {
            if (!firstPosition.IsAdjacentTo(secondPosition))
            {
                throw new ArgumentException("Board positions must be adjacent.");
            }

            Gem firstGem = GetGem(firstPosition);
            Gem secondGem = GetGem(secondPosition);

            SetGem(firstPosition, secondGem);
            SetGem(secondPosition, firstGem);
        }

        /// <summary>
        /// Stores gems placed on the game board.
        /// </summary>
        private Gem[,] gems = null;

        /// <summary>
        /// Gets the number of columns on the board.
        /// </summary>
        public int Width
        {
            get
            {
                return width;
            }
        }

        /// <summary>
        /// Gets the number of rows on the board.
        /// </summary>
        public int Height
        {
            get
            {
                return height;
            }
        }

        /// <summary>
        /// Creates a game board with the specified dimensions.
        /// </summary>
        /// <param name="width">Number of columns on the board.</param>
        /// <param name="height">Number of rows on the board.</param>
        public Board(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Board width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Board height must be greater than zero.");
            }

            this.width = width;
            this.height = height;
        }
    }
}