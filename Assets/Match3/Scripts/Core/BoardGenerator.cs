using System;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Generates the initial state of a match-3 game board.
    /// </summary>
    public class BoardGenerator
    {
        private Random random = null;
        private int gemTypeCount = 0;

        /// <summary>
        /// Initializes a board generator using all available gem types.
        /// </summary>
        public BoardGenerator()
            : this(Enum.GetValues(typeof(GemType)).Length)
        {
        }
        
        /// <summary>
        /// Creates a board generator with the specified number of available gem types.
        /// </summary>
        /// <param name="gemTypeCount">Number of gem types available for generation.</param>
        public BoardGenerator(int gemTypeCount)
        {
            int availableGemTypeCount = Enum.GetValues(typeof(GemType)).Length;

            if (gemTypeCount < 3 || gemTypeCount > availableGemTypeCount)
            {
                throw new ArgumentOutOfRangeException(nameof(gemTypeCount), gemTypeCount, "Gem type count is outside the available range.");
            }

            this.gemTypeCount = gemTypeCount;
            random = new Random();
        }

        /// <summary>
        /// Determines whether placing a gem would create an initial match.
        /// </summary>
        /// <param name="board">Board being generated.</param>
        /// <param name="position">Position being filled.</param>
        /// <param name="gemType">Gem type to test.</param>
        /// <returns>True if the placement creates a horizontal or vertical match.</returns>
        private bool CreatesInitialMatch(
            Board board,
            BoardPosition position,
            GemType gemType)
        {
            bool horizontalMatch = false;
            bool verticalMatch = false;

            if (position.Column >= 2)
            {
                Gem firstGem = board.GetGem(
                    new BoardPosition(position.Column - 1, position.Row));

                Gem secondGem = board.GetGem(
                    new BoardPosition(position.Column - 2, position.Row));

                horizontalMatch =
                    firstGem != null &&
                    secondGem != null &&
                    firstGem.GemType == gemType &&
                    secondGem.GemType == gemType;
            }

            if (position.Row >= 2)
            {
                Gem firstGem = board.GetGem(
                    new BoardPosition(position.Column, position.Row - 1));

                Gem secondGem = board.GetGem(
                    new BoardPosition(position.Column, position.Row - 2));

                verticalMatch =
                    firstGem != null &&
                    secondGem != null &&
                    firstGem.GemType == gemType &&
                    secondGem.GemType == gemType;
            }

            return horizontalMatch || verticalMatch;
        }

        /// <summary>
        /// Selects a random gem type that does not create an initial match.
        /// </summary>
        /// <param name="board">Board being generated.</param>
        /// <param name="position">Position being filled.</param>
        /// <returns>Valid gem type for the specified position.</returns>
        private GemType GetValidGemType(
            Board board,
            BoardPosition position)
        {
            int startIndex = random.Next(0, gemTypeCount);

            for (int offset = 0; offset < gemTypeCount; ++offset)
            {
                int typeIndex =
                    (startIndex + offset) % gemTypeCount;

                GemType gemType = (GemType)typeIndex;

                if (!CreatesInitialMatch(board, position, gemType))
                {
                    return gemType;
                }
            }

            throw new InvalidOperationException(
                "Unable to find a valid gem type.");
        }

        /// <summary>
        /// Generates a board without initial matching sequences.
        /// </summary>
        /// <param name="width">Board width.</param>
        /// <param name="height">Board height.</param>
        /// <returns>Generated logical board.</returns>
        public Board Generate(int width, int height)
        {
            Board board = new Board(width, height);

            for (int row = 0; row < board.Height; ++row)
            {
                for (int column = 0; column < board.Width; ++column)
                {
                    BoardPosition position =
                        new BoardPosition(column, row);

                    GemType gemType =
                        GetValidGemType(board, position);

                    Gem gem =
                        new Gem(gemType, position);

                    board.SetGem(position, gem);
                }
            }

            return board;
        }
    }
}