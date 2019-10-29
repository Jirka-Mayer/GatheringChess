using System;
using UnityEngine;

namespace GatheringChess
{
    [ExecuteInEditMode]
    public class Tile : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        /// <summary>
        /// Is the tile white or black?
        /// </summary>
        public bool IsWhite
        {
            get => isWhite;

            set
            {
                isWhite = value;
                UpdateAppearance();
            }
        }
        [SerializeField] private bool isWhite = false;

        /// <summary>
        /// Is the tile active, meaning the chess piece on it is selected?
        /// </summary>
        public bool IsActive
        {
            get => isActive;

            set
            {
                isActive = value;
                UpdateAppearance();
            }
        }
        [SerializeField] private bool isActive = false;

        /// <summary>
        /// Is the tile a possible move target of a selected piece?
        /// </summary>
        public bool IsTarget
        {
            get => isTarget;

            set
            {
                isTarget = value;
                UpdateAppearance();
            }
        }
        [SerializeField] private bool isTarget = false;

        public Color BlackColor;
        public Color WhiteColor;
        public Color ActiveBlackColor;
        public Color ActiveWhiteColor;
        public Color TargetBlackColor;
        public Color TargetWhiteColor;

        public event Action<Tile> OnClick;

        public Vector2Int Coordinates { get; set; }

        void OnValidate()
        {
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            if (spriteRenderer == null)
                return;

            if (IsActive)
            {
                spriteRenderer.color = IsWhite ? ActiveWhiteColor : ActiveBlackColor;
                return;
            }

            if (IsTarget)
            {
                spriteRenderer.color = IsWhite ? TargetWhiteColor : TargetBlackColor;
                return;
            }

            spriteRenderer.color = IsWhite ? WhiteColor : BlackColor;
        }

        void OnMouseDown()
        {
            if (OnClick != null)
                OnClick.Invoke(this);
        }
    }
}
