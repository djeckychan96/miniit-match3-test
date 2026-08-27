using System;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Represents a position of a cell on the game board.
    /// </summary>
    public struct BoardPosition : IEquatable<BoardPosition>
    {
        private int column;
        private int row;

        /// <summary>
        /// Gets the column index of the board position.
        /// </summary>
        public int Column
        {
            get
            {
                return column;
            }
        }

        /// <summary>
        /// Gets the row index of the board position.
        /// </summary>
        public int Row
        {
            get
            {
                return row;
            }
        }

        /// <summary>
        /// Creates a board position with the specified column and row.
        /// </summary>
        /// <param name="column">Column index on the board.</param>
        /// <param name="row">Row index on the board.</param>
        public BoardPosition(int column, int row)
        {
            this.column = column;
            this.row = row;
        }

        /// <summary>
        /// Determines whether another position is directly adjacent to this position.
        /// </summary>
        /// <param name="other">Position to compare with.</param>
        /// <returns>True if the positions share one horizontal or vertical side.</returns>
        public bool IsAdjacentTo(BoardPosition other)
        {
            int columnDistance = Math.Abs(column - other.column);
            int rowDistance = Math.Abs(row - other.row);

            return columnDistance + rowDistance == 1;
        }

        /// <summary>
        /// Determines whether this position has the same coordinates as another position.
        /// </summary>
        /// <param name="other">Position to compare with.</param>
        /// <returns>True if both positions have the same column and row.</returns>
        public bool Equals(BoardPosition other)
        {
            return column == other.column && row == other.row;
        }

        /// <summary>
        /// Determines whether this position is equal to the specified object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>True if the object is a board position with the same coordinates.</returns>
        public override bool Equals(object obj)
        {
            if (obj is BoardPosition)
            {
                BoardPosition other = (BoardPosition)obj;

                return Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code based on the board coordinates.
        /// </summary>
        /// <returns>Hash code of the board position.</returns>
        public override int GetHashCode()
        {
            int hashCode = column;
            hashCode = (hashCode * 397) ^ row;

            return hashCode;
        }
        
        /// <summary>
        /// Determines whether two board positions have the same coordinates.
        /// </summary>
        /// <param name="left">First position to compare.</param>
        /// <param name="right">Second position to compare.</param>
        /// <returns>True if both positions have the same coordinates.</returns>
        public static bool operator ==(BoardPosition left, BoardPosition right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two board positions have different coordinates.
        /// </summary>
        /// <param name="left">First position to compare.</param>
        /// <param name="right">Second position to compare.</param>
        /// <returns>True if the positions have different coordinates.</returns>
        public static bool operator !=(BoardPosition left, BoardPosition right)
        {
            return !left.Equals(right);
        }
    }
}