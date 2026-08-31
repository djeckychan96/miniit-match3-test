using Doozy.Runtime.UIManager.Containers;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Displays the current game session state.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField]
        private TMP_Text movesText = null;

        [SerializeField]
        private TMP_Text goalText = null;

        [SerializeField]
        private RectTransform goalRoot = null;

        [Header("Popups")]
        [SerializeField]
        private UIView winPopup = null;

        [SerializeField]
        private UIView losePopup = null;

        [SerializeField]
        private TMP_Text winGoalText = null;

        [SerializeField]
        private TMP_Text loseGoalText = null;

        [SerializeField]
        private Button winRestartButton = null;

        [SerializeField]
        private Button loseRestartButton = null;

        /// <summary>
        /// Current game session displayed by this UI.
        /// </summary>
        private GameSession gameSession = null;

        private int lastCollectedAmount = 0;

        private Coroutine goalPunchCoroutine = null;

        /// <summary>
        /// Initializes the UI with the specified game session.
        /// </summary>
        /// <param name="gameSession">Game session to display.</param>
        public void Initialize(GameSession gameSession)
        {
            if (gameSession == null)
            {
                return;
            }

            this.gameSession = gameSession;

            gameSession.GoalChanged += OnGoalChanged;
            gameSession.MovesChanged += OnMovesChanged;
            gameSession.GameEnded += OnGameEnded;

            OnGoalChanged(
                gameSession.CollectedAmount,
                gameSession.TargetAmount);

            OnMovesChanged(gameSession.MovesRemaining);

            if (winRestartButton != null)
            {
                winRestartButton.onClick.AddListener(RestartGame);
            }

            if (loseRestartButton != null)
            {
                loseRestartButton.onClick.AddListener(RestartGame);
            }
        }

        /// <summary>
        /// Updates the displayed level goal progress.
        /// </summary>
        /// <param name="collected">Number of collected target gems.</param>
        /// <param name="target">Number of target gems required to win.</param>
        private void OnGoalChanged(int collected, int target)
        {
            if (goalText != null)
            {
                goalText.text =
                    collected.ToString() +
                    " / " +
                    target.ToString();
            }

            if (collected > lastCollectedAmount &&
                goalRoot != null)
            {
                if (goalPunchCoroutine != null)
                {
                    StopCoroutine(goalPunchCoroutine);
                }

                goalPunchCoroutine =
                    StartCoroutine(AnimateGoalPunch());
            }

            lastCollectedAmount = collected;
        }

        /// <summary>
        /// Animates the goal indicator when target gems are collected.
        /// </summary>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator AnimateGoalPunch()
        {
            Vector3 startScale = Vector3.one;
            Vector3 punchScale = Vector3.one * 1.18f;

            float duration = 0.16f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress =
                    Mathf.Clamp01(elapsedTime / duration);

                float punchProgress =
                    Mathf.Sin(progress * Mathf.PI);

                goalRoot.localScale =
                    Vector3.Lerp(
                        startScale,
                        punchScale,
                        punchProgress);

                yield return null;
            }

            goalRoot.localScale = startScale;
            goalPunchCoroutine = null;
        }

        /// <summary>
        /// Updates the displayed remaining move count.
        /// </summary>
        /// <param name="moves">Remaining move count.</param>
        private void OnMovesChanged(int moves)
        {
            if (movesText != null)
            {
                movesText.text = moves.ToString();
            }
        }

        /// <summary>
        /// Updates a popup goal progress label.
        /// </summary>
        /// <param name="popupGoalText">Popup goal text to update.</param>
        private void UpdatePopupGoalText(TMP_Text popupGoalText)
        {
            if (popupGoalText == null ||
                gameSession == null)
            {
                return;
            }

            popupGoalText.text =
                gameSession.CollectedAmount.ToString() +
                " / " +
                gameSession.TargetAmount.ToString();
        }

        /// <summary>
        /// Displays the popup associated with the completed game result.
        /// </summary>
        /// <param name="result">Completed game result.</param>
        private void OnGameEnded(GameResult result)
        {
            if (result == GameResult.Win)
            {
                UpdatePopupGoalText(winGoalText);

                if (winPopup != null)
                {
                    winPopup.Show();
                }

                return;
            }

            UpdatePopupGoalText(loseGoalText);

            if (losePopup != null)
            {
                losePopup.Show();
            }
        }

        /// <summary>
        /// Restarts the current game scene.
        /// </summary>
        private void RestartGame()
        {
            Scene currentScene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(currentScene.buildIndex);
        }

        /// <summary>
        /// Releases game session event subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            if (gameSession == null)
            {
                return;
            }

            gameSession.GoalChanged -= OnGoalChanged;
            gameSession.MovesChanged -= OnMovesChanged;
            gameSession.GameEnded -= OnGameEnded;

            if (winRestartButton != null)
            {
                winRestartButton.onClick.RemoveListener(RestartGame);
            }

            if (loseRestartButton != null)
            {
                loseRestartButton.onClick.RemoveListener(RestartGame);
            }
        }
    }
}