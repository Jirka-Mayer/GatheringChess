﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GatheringChess.Playground
{
    public class Board : MonoBehaviour
    {
        public const int BoardSize = 8;

        public GameObject tilePrefab;
        public GameObject piecePrefab;

        private Tile[,] tiles; // X: (a, b, ..., h), Y: (1, 2, ..., 8)      [X, Y]
        private List<Piece> pieces;

        /// <summary>
        /// What color does the player play for
        /// </summary>
        private bool isPlayerWhite;

        /// <summary>
        /// Is currently playing the local player?
        /// </summary>
        private bool playersTurn = false;

        private Piece activePiece = null;
        private List<Tile> possibleTargets;

        private Action<ChessMove> onPlayerMoveDone;

        public Piece this[int x, int y]
            => pieces.FirstOrDefault(p => p.Coordinates.x == x && p.Coordinates.y == y);

        /// <summary>
        /// Call this before using the chess board
        /// </summary>
        public void CreateBoard(bool isPlayerWhite, ChessHalfSet whiteSet, ChessHalfSet blackSet)
        {
            this.isPlayerWhite = isPlayerWhite;

            // rotate chessboard 180 deg
            if (!isPlayerWhite)
            {
                transform.Rotate(0, 0, 180, Space.Self);
            }

            // create tiles
            tiles = new Tile[BoardSize, BoardSize];

            for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++)
            {
                var go = Instantiate(
                    tilePrefab,
                    new Vector3(
                        x - BoardSize / 2 + 0.5f,
                        y - BoardSize / 2 + 0.5f,
                        0
                    ),
                    Quaternion.identity
                );
                go.transform.SetParent(this.transform, false);
                go.transform.rotation = Quaternion.identity;

                var tile = go.GetComponent<Tile>();
                tile.IsWhite = (x % 2 == 0) ^ (y % 2 == 0);
                tile.Coordinates = new Vector2Int(x, y);
                tile.OnClick += TileClicked;

                tiles[x, y] = tile;
            }

            // place pieces
            pieces = new List<Piece>();

            // white
            for (int i = 0; i < BoardSize; i++)
                CreatePiece(i, 1, whiteSet.pawns[i]);
        
            CreatePiece(0, 0, whiteSet.leftRook);
            CreatePiece(1, 0, whiteSet.leftKnight);
            CreatePiece(2, 0, whiteSet.leftBishop);
            CreatePiece(3, 0, whiteSet.queen);
            CreatePiece(4, 0, whiteSet.king);
            CreatePiece(5, 0, whiteSet.rightBishop);
            CreatePiece(6, 0, whiteSet.rightKnight);
            CreatePiece(7, 0, whiteSet.rightRook);

            // black
            for (int i = 0; i < BoardSize; i++)
                CreatePiece(i, 6, blackSet.pawns[i]);
        
            CreatePiece(0, 7, blackSet.leftRook);
            CreatePiece(1, 7, blackSet.leftKnight);
            CreatePiece(2, 7, blackSet.leftBishop);
            CreatePiece(3, 7, blackSet.queen);
            CreatePiece(4, 7, blackSet.king);
            CreatePiece(5, 7, blackSet.rightBishop);
            CreatePiece(6, 7, blackSet.rightKnight);
            CreatePiece(7, 7, blackSet.rightRook);
        }

        private void CreatePiece(int x, int y, PieceId pieceId)
        {
            var go = Instantiate(
                piecePrefab,
                Vector3.zero,
                Quaternion.identity
            );
            go.transform.SetParent(this.transform, false);
            go.transform.rotation = Quaternion.identity;

            var piece = go.GetComponent<Piece>();
            piece.Id = pieceId;
            piece.MovePieceTo(x, y, false);

            pieces.Add(piece);
        }

        private void TileClicked(Tile tile)
        {
            // clicking works only when the player can play
            if (!playersTurn)
                return;

            // activating a piece
            if (activePiece == null)
            {
                ActivatePiece(
                    GetPieceOn(tile.Coordinates.x, tile.Coordinates.y) // null is ok
                );
                return;
            }

            // deactivating a piece (clicking where no target exists)
            if (!possibleTargets.Contains(tile))
            {
                DeactivatePiece();
                return;
            }

            // killing the enemy
            Piece targetPiece = GetPieceOn(tile.Coordinates.x, tile.Coordinates.y);
            if (targetPiece != null)
            {
                KillPiece(targetPiece);
            }

            // moving the piece to a target
            var pieceToMove = activePiece;
            Vector2Int from = pieceToMove.Coordinates;
            DeactivatePiece();

            pieceToMove.MovePieceTo(tile.Coordinates.x, tile.Coordinates.y, true);

            // player move is done
            onPlayerMoveDone?.Invoke(new ChessMove {
                from = from,
                to = tile.Coordinates
            });
        }

        private void ActivatePiece(Piece piece)
        {
            if (piece == null)
                return;

            if (piece.Id.color.IsWhite() != isPlayerWhite)
                return;

            activePiece = piece;
            tiles[piece.Coordinates.x, piece.Coordinates.y].IsActive = true;
            possibleTargets = piece.GetPossibleTargets(tiles, pieces);
            foreach (var t in possibleTargets)
                t.IsTarget = true;
        }

        private void DeactivatePiece()
        {
            if (activePiece == null)
                return;

            tiles[activePiece.Coordinates.x, activePiece.Coordinates.y].IsActive = false;
            foreach (var t in possibleTargets)
                t.IsTarget = false;
        
            activePiece = null;
            possibleTargets = null;
        }

        private Piece GetPieceOn(int x, int y)
        {
            foreach (Piece p in pieces)
            {
                if (p.Coordinates.x == x && p.Coordinates.y == y)
                    return p;
            }

            return null;
        }

        public Task<ChessMove> LetPlayerHaveAMove()
        {
            playersTurn = true;

            var promise = new TaskCompletionSource<ChessMove>();

            onPlayerMoveDone += (move) => {
                playersTurn = false;
                onPlayerMoveDone = null;
                promise.SetResult(move);
            };

            return promise.Task;
        }

        public void PerformOpponentsMove(Vector2Int from, Vector2Int to)
        {
            Piece pieceToMove = GetPieceOn(from.x, from.y);

            if (pieceToMove == null)
                throw new Exception("Opponent played a move that is inconsistent with our state.");

            Piece targetPiece = GetPieceOn(to.x, to.y);
        
            if (targetPiece != null)
                KillPiece(targetPiece);

            pieceToMove.MovePieceTo(to.x, to.y, true);
        }

        public void KillPiece(Piece piece)
        {
            pieces.Remove(piece);
            piece.gameObject.SetActive(false);
        }
    }
}