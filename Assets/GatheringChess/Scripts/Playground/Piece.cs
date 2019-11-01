using System;
using System.Collections.Generic;
using System.Linq;
using RSG;
using UnityEngine;

namespace GatheringChess.Playground
{
    public class Piece : MonoBehaviour
    {
        /// <summary>
        /// ID of the piece
        /// </summary>
        public PieceId Id
        {
            get => id;

            set
            {
                id = value;
                UpdateAppearance();
            }
        }
        [SerializeField] private PieceId id = PieceId.Default;

        /// <summary>
        /// Coordinates of the piece (in board space)
        /// </summary>
        /// <value></value>
        public Vector2Int Coordinates { get; private set; }

        /// <summary>
        /// Where the piece should be located (in continuous 3D space)
        /// </summary>
        public Vector3 TargetPosition
        {
            get => targetPosition;
        
            set
            {
                targetPosition = value;
                animatePosition = true;
            }
        }
        private Vector3 targetPosition;
        
        private bool animatePosition;
        private Action onPositionAnimationFinish;
        
        /// <summary>
        /// Reference to the sprite renderer
        /// </summary>
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            UpdateAppearance();
        }

        void OnValidate()
        {
            UpdateAppearance();
        }

        void Update()
        {
            if (animatePosition)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.1f);

                if ((transform.localPosition - targetPosition).sqrMagnitude < 0.01f)
                {
                    transform.localPosition = targetPosition;
                    animatePosition = false;

                    onPositionAnimationFinish?.Invoke();
                    onPositionAnimationFinish = null;
                }
            }
        }

        private void UpdateAppearance()
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.sprite = Id.LoadSprite();
        }

        /// <summary>
        /// Move a piece to a position on the board
        /// </summary>
        public IPromise MovePieceTo(int x, int y, bool animate)
        {
            Coordinates = new Vector2Int(x, y);

            targetPosition = new Vector3(
                x - Board.BoardSize / 2 + 0.5f,
                y - Board.BoardSize / 2 + 0.5f,
                0
            );

            if (!animate)
            {
                transform.localPosition = targetPosition;
                animatePosition = false;

                onPositionAnimationFinish?.Invoke();
                onPositionAnimationFinish = null;

                return Promise.Resolved();
            }

            animatePosition = true;

            Promise promise = new Promise();
            onPositionAnimationFinish += () => {
                promise.Resolve();
            };
            return promise;
        }

        public List<Tile> GetPossibleTargets(Tile[,] tiles, List<Piece> pieces)
        {
            var targets = new List<Tile>();

            for (int x = 0; x < Board.BoardSize; x++)
            for (int y = 0; y < Board.BoardSize; y++)
            {
                if (x == Coordinates.x && y == Coordinates.y)
                    continue;

                if ((new Vector2Int(x, y) - Coordinates).magnitude > 2f)
                    continue;

                var piece = pieces.FirstOrDefault(p => p.Coordinates == new Vector2Int(x, y));
                if (piece != null && piece.Id.color == Id.color)
                    continue;

                targets.Add(tiles[x, y]);
            }

            return targets;
        }
    }
}
