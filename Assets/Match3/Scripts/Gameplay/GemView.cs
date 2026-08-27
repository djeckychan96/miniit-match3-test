using System;
using UnityEngine.EventSystems;
using UnityEngine;

namespace MiniIT.MATCH3
{
    /// <summary>
    /// Displays a gem model in the Unity scene.
    /// </summary>
    public class GemView : MonoBehaviour, IPointerClickHandler
    {
        [Header("References")]

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        [Header("Visual Settings")]

        [SerializeField]
        private Sprite[] gemSprites = null;

        /// <summary>
        /// Gem model represented by this view.
        /// </summary>
        private Gem gem = null;

        /// <summary>
        /// Gets the current board position of the displayed gem.
        /// </summary>
        public BoardPosition Position
        {
            get
            {
                return gem.Position;
            }
        }

        /// <summary>
        /// Initial scale of the gem visual.
        /// </summary>
        private Vector3 defaultVisualScale;

        /// <summary>
        /// Invoked when the player selects this gem view.
        /// </summary>
        public event Action<GemView> Clicked = null;

        /// <summary>
        /// Validates required scene references.
        /// </summary>
        private void Awake()
        {
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer reference is not assigned.", this);
            }

            if (spriteRenderer != null)
            {
                defaultVisualScale = spriteRenderer.transform.localScale;
            }
        }

        /// <summary>
        /// Initializes the view with the specified gem model.
        /// </summary>
        /// <param name="gem">Gem model to display.</param>
        public void Initialize(Gem gem)
        {
            if (gem == null)
            {
                Debug.LogError("Gem model cannot be null.", this);

                return;
            }

            int spriteIndex = (int)gem.GemType;

            if (gemSprites == null ||
                spriteIndex < 0 ||
                spriteIndex >= gemSprites.Length)
            {
                Debug.LogError("Sprite for the specified gem type is not configured.", this);

                return;
            }

            this.gem = gem;
            spriteRenderer.sprite = gemSprites[spriteIndex];
        }

        /// <summary>
        /// Changes the visual state of the gem selection.
        /// </summary>
        /// <param name="selected">True if the gem should be displayed as selected.</param>
        public void SetSelected(bool selected)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (selected)
            {
                spriteRenderer.transform.localScale = defaultVisualScale * 1.12f;
            }
            else
            {
                spriteRenderer.transform.localScale = defaultVisualScale;
            }
        }

        /// <summary>
        /// Handles pointer input on this gem view.
        /// </summary>
        /// <param name="eventData">Pointer event data provided by Unity.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Action<GemView> clicked = Clicked;

            if (clicked != null)
            {
                clicked(this);
            }
        }
    }
}