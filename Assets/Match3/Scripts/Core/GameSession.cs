using System;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Stores game progress and evaluates win and lose conditions.
    /// </summary>
    public class GameSession
    {
        private int movesRemaining = 0;
        private GemType targetGemType = GemType.Red;
        private int targetAmount = 0;
        private int collectedAmount = 0;
        private bool isFinished = false;

        /// <summary>
        /// Gets the number of moves remaining.
        /// </summary>
        public int MovesRemaining
        {
            get
            {
                return movesRemaining;
            }
        }

        /// <summary>
        /// Gets the gem type required by the current level goal.
        /// </summary>
        public GemType TargetGemType
        {
            get
            {
                return targetGemType;
            }
        }

        /// <summary>
        /// Gets the number of gems required to complete the goal.
        /// </summary>
        public int TargetAmount
        {
            get
            {
                return targetAmount;
            }
        }

        /// <summary>
        /// Gets the number of collected target gems.
        /// </summary>
        public int CollectedAmount
        {
            get
            {
                return collectedAmount;
            }
        }

        /// <summary>
        /// Gets whether the game session has ended.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return isFinished;
            }
        }

        /// <summary>
        /// Occurs when progress towards the level goal changes.
        /// </summary>
        public event Action<int, int> GoalChanged = null;

        /// <summary>
        /// Occurs when the remaining move count changes.
        /// </summary>
        public event Action<int> MovesChanged = null;

        /// <summary>
        /// Occurs when the game session ends.
        /// </summary>
        public event Action<GameResult> GameEnded = null;

        /// <summary>
        /// Initializes a new game session.
        /// </summary>
        /// <param name="moves">Number of available moves.</param>
        /// <param name="targetGemType">Gem type required by the level goal.</param>
        /// <param name="targetAmount">Number of target gems required to win.</param>
        public GameSession(
            int moves,
            GemType targetGemType,
            int targetAmount)
        {
            if (moves <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moves));
            }

            if (targetAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetAmount));
            }

            movesRemaining = moves;
            this.targetGemType = targetGemType;
            this.targetAmount = targetAmount;
        }

        /// <summary>
        /// Registers a successfully completed player move.
        /// </summary>
        public void RegisterMove()
        {
            if (isFinished || movesRemaining <= 0)
            {
                return;
            }

            --movesRemaining;

            Action<int> movesChanged = MovesChanged;

            if (movesChanged != null)
            {
                movesChanged(movesRemaining);
            }
        }

        /// <summary>
        /// Registers collected gems that belong to the current level goal.
        /// </summary>
        /// <param name="gemCount">Number of collected target gems.</param>
        public void RegisterCollectedGems(int gemCount)
        {
            if (isFinished || gemCount <= 0)
            {
                return;
            }

            collectedAmount =
                Math.Min(collectedAmount + gemCount, targetAmount);

            Action<int, int> goalChanged = GoalChanged;

            if (goalChanged != null)
            {
                goalChanged(collectedAmount, targetAmount);
            }
        }

        /// <summary>
        /// Evaluates win and lose conditions after the board has been resolved.
        /// </summary>
        public void CompleteTurn()
        {
            if (isFinished)
            {
                return;
            }

            GameResult? result = null;

            if (collectedAmount >= targetAmount)
            {
                result = GameResult.Win;
            }
            else if (movesRemaining <= 0)
            {
                result = GameResult.Lose;
            }

            if (!result.HasValue)
            {
                return;
            }

            isFinished = true;

            Action<GameResult> gameEnded = GameEnded;

            if (gameEnded != null)
            {
                gameEnded(result.Value);
            }
        }
    }
}