using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Connects the logical game board with its Unity scene representation.
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField]
        private int width = 8;

        [SerializeField]
        private int height = 8;

        [SerializeField]
        private float cellSize = 1f;

        [Header("Game Settings")]
        [SerializeField]
        private int startMoves = 20;

        [SerializeField]
        private GemType targetGemType = GemType.Purple;

        [SerializeField]
        private int targetAmount = 20;

        [Header("References")]
        [SerializeField]
        private GemView gemPrefab = null;

        [SerializeField]
        private GameUI gameUI = null;

        [SerializeField]
        private ParticleSystem matchParticlesPrefab = null;

        [Header("Animation Settings")]
        [SerializeField]
        private float swapDuration = 0.2f;

        [SerializeField]
        private float destroyDuration = 0.18f;

        [SerializeField]
        private float fallDuration = 0.25f;

        /// <summary>
        /// Logical state of the game board.
        /// </summary>
        private Board board = null;

        /// <summary>
        /// Finds matching gem sequences on the logical board.
        /// </summary>
        private MatchFinder matchFinder = null;

        /// <summary>
        /// Stores the current game session state.
        /// </summary>
        private GameSession gameSession = null;

        /// <summary>
        /// Stores visual representations associated with gem models.
        /// </summary>
        private Dictionary<Gem, GemView> gemViews = null;

        /// <summary>
        /// Gets the gem view collection and creates it on first access.
        /// </summary>
        private Dictionary<Gem, GemView> GemViews
        {
            get
            {
                if (gemViews == null)
                {
                    gemViews = new Dictionary<Gem, GemView>();
                }

                return gemViews;
            }
        }

        /// <summary>
        /// Calculates the local position of a board cell.
        /// </summary>
        /// <param name="position">Logical board position.</param>
        /// <returns>Local position of the corresponding cell.</returns>
        private Vector3 GetCellLocalPosition(BoardPosition position)
        {
            float offsetX = (board.Width - 1) * cellSize * 0.5f;
            float offsetY = (board.Height - 1) * cellSize * 0.5f;

            float x = position.Column * cellSize - offsetX;
            float y = position.Row * cellSize - offsetY;

            return new Vector3(x, y, 0f);
        }

        /// <summary>
        /// Moves gems down to fill empty board cells.
        /// </summary>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator CollapseBoard()
        {
            bool hasMovement = false;

            for (int column = 0; column < board.Width; ++column)
            {
                int targetRow = 0;

                for (int row = 0; row < board.Height; ++row)
                {
                    BoardPosition sourcePosition =
                        new BoardPosition(column, row);

                    Gem gem = board.GetGem(sourcePosition);

                    if (gem == null)
                    {
                        continue;
                    }

                    if (row != targetRow)
                    {
                        BoardPosition targetPosition =
                            new BoardPosition(column, targetRow);

                        GemView gemView = GemViews[gem];

                        board.SetGem(targetPosition, gem);
                        board.SetGem(sourcePosition, null);

                        Vector3 targetLocalPosition =
                            GetCellLocalPosition(targetPosition);

                        StartCoroutine(
                            AnimateGemView(gemView, targetLocalPosition));

                        hasMovement = true;
                    }

                    ++targetRow;
                }
            }

            if (hasMovement)
            {
                yield return new WaitForSeconds(fallDuration);
            }
        }

        /// <summary>
        /// Counts target gems included in the specified matches.
        /// </summary>
        /// <param name="matches">Matched board positions.</param>
        /// <returns>Number of gems belonging to the current level goal.</returns>
        private int CountTargetGems(List<BoardPosition> matches)
        {
            int targetGemCount = 0;

            for (int i = 0; i < matches.Count; ++i)
            {
                Gem gem = board.GetGem(matches[i]);

                if (gem != null &&
                    gem.GemType == gameSession.TargetGemType)
                {
                    ++targetGemCount;
                }
            }

            return targetGemCount;
        }

        /// <summary>
        /// Resolves board matches, falling gems and automatic cascades.
        /// </summary>
        /// <param name="matches">Initial matched positions.</param>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator ResolveBoard(List<BoardPosition> matches)
        {
            List<BoardPosition> currentMatches = matches;

            while (currentMatches.Count > 0)
            {
                int targetGemCount =
                    CountTargetGems(currentMatches);

                gameSession.RegisterCollectedGems(targetGemCount);

                yield return StartCoroutine(
                    DestroyMatches(currentMatches));

                yield return StartCoroutine(
                    CollapseBoard());

                yield return StartCoroutine(
                    RefillBoard());

                currentMatches = matchFinder.FindMatches(board);
            }
        }

        /// <summary>
        /// Creates new gems for empty board cells and animates them from above.
        /// </summary>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator RefillBoard()
        {
            bool hasNewGems = false;

            for (int column = 0; column < board.Width; ++column)
            {
                int emptyCount = 0;

                for (int row = 0; row < board.Height; ++row)
                {
                    BoardPosition position =
                        new BoardPosition(column, row);

                    if (board.GetGem(position) == null)
                    {
                        ++emptyCount;
                    }
                }

                if (emptyCount == 0)
                {
                    continue;
                }

                for (int row = board.Height - emptyCount;
                    row < board.Height;
                    ++row)
                {
                    BoardPosition position =
                        new BoardPosition(column, row);

                    GemType gemType =
                        (GemType)Random.Range(0, 5);

                    Gem gem = new Gem(gemType, position);

                    board.SetGem(position, gem);

                    GemView gemView =
                        Instantiate<GemView>(gemPrefab, transform);

                    Vector3 targetPosition =
                        GetCellLocalPosition(position);

                    Vector3 spawnPosition =
                        targetPosition +
                        Vector3.up * emptyCount * cellSize;

                    gemView.transform.localPosition = spawnPosition;
                    gemView.Initialize(gem);
                    GemViews.Add(gem, gemView);
                    gemView.Clicked += OnGemClicked;
                    gemView.SwipeRequested += OnGemSwipeRequested;

                    StartCoroutine(
                        AnimateGemView(gemView, targetPosition));

                    hasNewGems = true;
                }
            }

            if (hasNewGems)
            {
                yield return new WaitForSeconds(fallDuration);
            }
        }

        /// <summary>
        /// Currently selected gem view.
        /// </summary>
        private GemView selectedGemView = null;

        /// <summary>
        /// Indicates whether board input is temporarily blocked.
        /// </summary>
        private bool inputLocked = false;

        /// <summary>
        /// Initializes the logical game board when the scene starts.
        /// </summary>
        private void Start()
        {
            BoardGenerator boardGenerator = new BoardGenerator();
            board = boardGenerator.Generate(width, height);
            matchFinder = new MatchFinder();
            gameSession = new GameSession(startMoves,targetGemType,targetAmount);
            
            if (gameUI != null)
            {
                gameUI.Initialize(gameSession);
            }

            CreateBoardView();
        }

        /// <summary>
        /// Creates the visual representation of the game board.
        /// </summary>
        private void CreateBoardView()
        {
            if (gemPrefab == null)
            {
                Debug.LogError("Gem prefab reference is not assigned.", this);

                return;
            }

            float offsetX = (board.Width - 1) * cellSize * 0.5f;
            float offsetY = (board.Height - 1) * cellSize * 0.5f;

            for (int row = 0; row < board.Height; ++row)
            {
                for (int column = 0; column < board.Width; ++column)
                {
                    float x = column * cellSize - offsetX;
                    float y = row * cellSize - offsetY;
                    Vector3 localPosition = new Vector3(x, y, 0f);

                    BoardPosition position = new BoardPosition(column, row);

                    Gem gem = board.GetGem(position);
                    
                    GemView gemView = Instantiate<GemView>(gemPrefab, transform);
                    gemView.transform.localPosition = localPosition;
                    gemView.Initialize(gem);
                    GemViews.Add(gem, gemView);
                    gemView.Clicked += OnGemClicked;
                    gemView.SwipeRequested += OnGemSwipeRequested;
                }
            }
        }

        /// <summary>
        /// Handles a swipe request between adjacent gems.
        /// </summary>
        /// <param name="gemView">Gem view where the swipe started.</param>
        /// <param name="targetPosition">Adjacent position selected by the swipe.</param>
        private void OnGemSwipeRequested(
            GemView gemView,
            BoardPosition targetPosition)
        {
            if (inputLocked ||
                gemView == null ||
                gameSession == null ||
                gameSession.IsFinished)
            {
                return;
            }

            if (!board.IsInside(targetPosition))
            {
                return;
            }

            if (!gemView.Position.IsAdjacentTo(targetPosition))
            {
                return;
            }

            Gem targetGem = board.GetGem(targetPosition);

            if (targetGem == null)
            {
                return;
            }

            GemView targetGemView = null;

            if (!GemViews.TryGetValue(targetGem, out targetGemView))
            {
                return;
            }

            if (selectedGemView != null)
            {
                selectedGemView.SetSelected(false);
                selectedGemView = null;
            }

            StartCoroutine(
                SwapGemViews(gemView, targetGemView));
        }

        /// <summary>
        /// Handles selection and swapping of gem views.
        /// </summary>
        /// <param name="gemView">Gem view selected by the player.</param>
        private void OnGemClicked(GemView gemView)
        {
           if (inputLocked ||
                gemView == null ||
                gameSession == null ||
                gameSession.IsFinished)
            {
                return;
            }

            if (selectedGemView == null)
            {
                selectedGemView = gemView;
                selectedGemView.SetSelected(true);

                return;
            }

            if (selectedGemView == gemView)
            {
                selectedGemView.SetSelected(false);
                selectedGemView = null;

                return;
            }

            if (!selectedGemView.Position.IsAdjacentTo(gemView.Position))
            {
                selectedGemView.SetSelected(false);
                selectedGemView = gemView;
                selectedGemView.SetSelected(true);

                return;
            }

            GemView firstGemView = selectedGemView;
            GemView secondGemView = gemView;

            selectedGemView.SetSelected(false);
            selectedGemView = null;

            StartCoroutine(SwapGemViews(firstGemView, secondGemView));
        }

        /// <summary>
        /// Swaps two gem views and restores them if the move does not create a match.
        /// </summary>
        /// <param name="firstGemView">First gem view.</param>
        /// <param name="secondGemView">Second gem view.</param>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator SwapGemViews(GemView firstGemView, GemView secondGemView)
        {
            inputLocked = true;

            BoardPosition firstPosition = firstGemView.Position;
            BoardPosition secondPosition = secondGemView.Position;

            Vector3 firstStartPosition = firstGemView.transform.localPosition;
            Vector3 secondStartPosition = secondGemView.transform.localPosition;

            board.SwapGems(firstPosition, secondPosition);

            yield return StartCoroutine(
                AnimateGemViews(
                    firstGemView,
                    secondGemView,
                    secondStartPosition,
                    firstStartPosition));

            List<BoardPosition> matches = matchFinder.FindMatches(board);

            bool createsMatch =
                matches.Contains(firstPosition) ||
                matches.Contains(secondPosition);

            if (!createsMatch)
            {
                board.SwapGems(firstPosition, secondPosition);

                yield return StartCoroutine(
                    AnimateGemViews(
                        firstGemView,
                        secondGemView,
                        firstStartPosition,
                        secondStartPosition));
            }

            if (createsMatch)
            {
                gameSession.RegisterMove();

                yield return StartCoroutine(
                    ResolveBoard(matches));

                gameSession.CompleteTurn();
            }

            inputLocked = false;
        }

        /// <summary>
        /// Animates two gem views between the specified local positions.
        /// </summary>
        /// <param name="firstGemView">First gem view to animate.</param>
        /// <param name="secondGemView">Second gem view to animate.</param>
        /// <param name="firstTargetPosition">Target position of the first gem view.</param>
        /// <param name="secondTargetPosition">Target position of the second gem view.</param>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator AnimateGemViews(
            GemView firstGemView,
            GemView secondGemView,
            Vector3 firstTargetPosition,
            Vector3 secondTargetPosition)
        {
            Vector3 firstStartPosition = firstGemView.transform.localPosition;
            Vector3 secondStartPosition = secondGemView.transform.localPosition;
            float elapsedTime = 0f;

            while (elapsedTime < swapDuration)
            {
                elapsedTime += Time.deltaTime;

                float progress = elapsedTime / swapDuration;
                progress = Mathf.Clamp01(progress);

                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

                firstGemView.transform.localPosition =
                    Vector3.Lerp(
                        firstStartPosition,
                        firstTargetPosition,
                        easedProgress);

                secondGemView.transform.localPosition =
                    Vector3.Lerp(
                        secondStartPosition,
                        secondTargetPosition,
                        easedProgress);

                yield return null;
            }

            firstGemView.transform.localPosition = firstTargetPosition;
            secondGemView.transform.localPosition = secondTargetPosition;
        }

        /// <summary>
        /// Animates a gem view to the specified local position.
        /// </summary>
        /// <param name="gemView">Gem view to animate.</param>
        /// <param name="targetPosition">Target local position.</param>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator AnimateGemView(
            GemView gemView,
            Vector3 targetPosition)
        {
            Vector3 startPosition = gemView.transform.localPosition;
            float elapsedTime = 0f;

            while (elapsedTime < fallDuration)
            {
                elapsedTime += Time.deltaTime;

                float progress = elapsedTime / fallDuration;
                progress = Mathf.Clamp01(progress);

                float easedProgress = GetFallProgress(progress);

                gemView.transform.localPosition =
                    Vector3.Lerp(
                        startPosition,
                        targetPosition,
                        easedProgress);

                yield return null;
            }

            gemView.transform.localPosition = targetPosition;
        }

        /// <summary>
        /// Animates and removes the specified gem view.
        /// </summary>
        /// <param name="gemView">Gem view to remove.</param>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator DestroyGemView(GemView gemView)
        {
            Vector3 startScale = gemView.transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < destroyDuration)
            {
                elapsedTime += Time.deltaTime;

                float progress = elapsedTime / destroyDuration;
                progress = Mathf.Clamp01(progress);

                gemView.transform.localScale =
                    Vector3.Lerp(startScale, Vector3.zero, progress);

                yield return null;
            }

            Destroy(gemView.gameObject);
        }

        /// <summary>
        /// Creates a particle burst at the specified gem view position.
        /// </summary>
        /// <param name="gemView">Gem view used as the particle origin.</param>
        private void CreateMatchParticles(GemView gemView)
        {
            if (matchParticlesPrefab == null ||
                gemView == null)
            {
                return;
            }

            ParticleSystem particles =
                Instantiate<ParticleSystem>(
                    matchParticlesPrefab,
                    gemView.transform.position,
                    Quaternion.identity);

            particles.Play();

            Destroy(
                particles.gameObject,
                particles.main.duration + 1f);
        }

        /// <summary>
        /// Removes all gems included in the specified matches.
        /// </summary>
        /// <param name="matches">Matched board positions.</param>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator DestroyMatches(List<BoardPosition> matches)
        {
            for (int i = 0; i < matches.Count; ++i)
            {
                BoardPosition position = matches[i];
                Gem gem = board.GetGem(position);

                if (gem == null)
                {
                    continue;
                }

                GemView gemView = GemViews[gem];

                CreateMatchParticles(gemView);

                GemViews.Remove(gem);
                board.SetGem(position, null);

                StartCoroutine(DestroyGemView(gemView));
            }

            yield return new WaitForSeconds(destroyDuration);
        }

        /// <summary>
        /// Calculates a subtle overshoot easing value.
        /// </summary>
        /// <param name="progress">Normalized animation progress.</param>
        /// <returns>Eased animation progress.</returns>
        private float GetFallProgress(float progress)
        {
            const float overshoot = 0.8f;

            float shiftedProgress = progress - 1f;

            return 1f +
                (overshoot + 1f) *
                shiftedProgress *
                shiftedProgress *
                shiftedProgress +
                overshoot *
                shiftedProgress *
                shiftedProgress;
        }
    }
}