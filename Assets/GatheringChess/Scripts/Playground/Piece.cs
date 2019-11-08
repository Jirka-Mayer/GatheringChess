using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GatheringChess.Playground
{
    /// <summary>
    /// Represents one piece physically on the board
    /// </summary>
    public class Piece : MonoBehaviour
    {
        /// <summary>
        /// ID of the piece
        /// </summary>
        public PieceId pieceId = PieceId.Default;

        /// <summary>
        /// Position of the piece on board
        /// </summary>
        public Vector2Int Position { get; private set; }

        // movement animation
        private Vector3 targetPosition;
        private bool animatePosition;
        private TaskCompletionSource<bool> movementPromise;
        
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
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition,
                    targetPosition,
                    0.1f
                );

                if ((transform.localPosition - targetPosition).sqrMagnitude < 0.01f)
                {
                    transform.localPosition = targetPosition;
                    animatePosition = false;
                    
                    movementPromise?.SetResult(true);
                    movementPromise = null;
                }
            }
        }

        private void OnDestroy()
        {
            movementPromise?.SetCanceled();
            movementPromise = null;
        }

        /// <summary>
        /// Set the proper sprite to display
        /// </summary>
        private void UpdateAppearance()
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.sprite = pieceId.LoadSprite();
        }

        /// <summary>
        /// Move piece to a position instantaneously
        /// </summary>
        public void SetPosition(Vector2Int position)
        {
            movementPromise?.SetCanceled();
            movementPromise = null;
            
            // update board position
            Position = position;
            
            // update real position
            targetPosition = new Vector3(
                position.x - Board.BoardSize / 2 + 0.5f,
                position.y - Board.BoardSize / 2 + 0.5f,
                0
            );
            
            // no animation
            animatePosition = false;
            transform.localPosition = targetPosition;
        }

        public void SetPosition(int x, int y)
            => SetPosition(new Vector2Int(x, y));

        /// <summary>
        /// Move a piece to a position on the board
        /// </summary>
        public Task MovePieceToAsync(Vector2Int position)
        {
            movementPromise?.SetCanceled();
            
            movementPromise = new TaskCompletionSource<bool>();
            
            // update board position
            Position = position;

            // animate real position
            animatePosition = true;
            targetPosition = new Vector3(
                position.x - Board.BoardSize / 2 + 0.5f,
                position.y - Board.BoardSize / 2 + 0.5f,
                0
            );
            
            return movementPromise.Task;
        }
        
        public Task MovePieceToAsync(int x, int y)
            => MovePieceToAsync(new Vector2Int(x, y));

        public async void MovePieceTo(Vector2Int position)
            => await MovePieceToAsync(position);
        
        public void MovePieceTo(int x, int y)
            => MovePieceTo(new Vector2Int(x, y));
    }
}
