using System;
using System.Collections.Generic;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Finds matching gem sequences on the game board.
    /// </summary>
    public class MatchFinder
    {
        /// <summary>
        /// Adds a board position to the match collection if it is not already present.
        /// </summary>
        /// <param name="matches">Collection of matched positions.</param>
        /// <param name="position">Position to add.</param>
        private void AddMatchPosition(List<BoardPosition> matches, BoardPosition position)
        {
            if (!matches.Contains(position))
            {
                matches.Add(position);
            }
        }

        /// <summary>
        /// Finds a horizontal match starting at the specified board position.
        /// </summary>
        /// <param name="board">Game board to inspect.</param>
        /// <param name="position">Start position of the potential match.</param>
        /// <param name="gemType">Gem type to compare.</param>
        /// <param name="matches">Collection receiving matched positions.</param>
        private void FindHorizontalMatch(
            Board board,
            BoardPosition position,
            GemType gemType,
            List<BoardPosition> matches)
        {
            if (position.Column + 2 >= board.Width)
            {
                return;
            }

            BoardPosition secondPosition =
                new BoardPosition(position.Column + 1, position.Row);

            BoardPosition thirdPosition =
                new BoardPosition(position.Column + 2, position.Row);

            Gem secondGem = board.GetGem(secondPosition);
            Gem thirdGem = board.GetGem(thirdPosition);

            if (secondGem == null || thirdGem == null)
            {
                return;
            }

            if (secondGem.GemType != gemType || thirdGem.GemType != gemType)
            {
                return;
            }

            AddMatchPosition(matches, position);
            AddMatchPosition(matches, secondPosition);
            AddMatchPosition(matches, thirdPosition);
        }

        /// <summary>
        /// Finds a vertical match starting at the specified board position.
        /// </summary>
        /// <param name="board">Game board to inspect.</param>
        /// <param name="position">Start position of the potential match.</param>
        /// <param name="gemType">Gem type to compare.</param>
        /// <param name="matches">Collection receiving matched positions.</param>
        private void FindVerticalMatch(
            Board board,
            BoardPosition position,
            GemType gemType,
            List<BoardPosition> matches)
        {
            if (position.Row + 2 >= board.Height)
            {
                return;
            }

            BoardPosition secondPosition =
                new BoardPosition(position.Column, position.Row + 1);

            BoardPosition thirdPosition =
                new BoardPosition(position.Column, position.Row + 2);

            Gem secondGem = board.GetGem(secondPosition);
            Gem thirdGem = board.GetGem(thirdPosition);

            if (secondGem == null || thirdGem == null)
            {
                return;
            }

            if (secondGem.GemType != gemType || thirdGem.GemType != gemType)
            {
                return;
            }

            AddMatchPosition(matches, position);
            AddMatchPosition(matches, secondPosition);
            AddMatchPosition(matches, thirdPosition);
        }

        /// <summary>
        /// Finds all matching gem positions on the game board.
        /// </summary>
        /// <param name="board">Game board to inspect.</param>
        /// <returns>Collection of positions included in matches of three or more gems.</returns>
        public List<BoardPosition> FindMatches(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            List<BoardPosition> matches = new List<BoardPosition>();

            for (int row = 0; row < board.Height; ++row)
            {
                for (int column = 0; column < board.Width; ++column)
                {
                    BoardPosition position = new BoardPosition(column, row);
                    Gem gem = board.GetGem(position);

                    if (gem == null)
                    {
                        continue;
                    }

                    FindHorizontalMatch(board, position, gem.GemType, matches);
                    FindVerticalMatch(board, position, gem.GemType, matches);
                }
            }

            return matches;
        }
    }
}