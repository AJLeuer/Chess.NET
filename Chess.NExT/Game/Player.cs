﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Chess.Util;
using Tree;

namespace Chess.Game
{
    public abstract class Player : ICloneable
    {
        
        protected static ulong uniqueIDs = 0;

        protected ulong ID { get; } = uniqueIDs++;

        public string name { get; }
    
        public virtual Color color { get; }

        private Board board;
        
        public Board Board {
            get { return board; }

            set
            {
                this.board = value;
                this.pieces = findOwnPiecesOnBoard(board);
                initializePieces();
            }
        }

        public List<Piece> pieces { get; private set; }

        /* Any other constructors should call this as a delegating constructor */
        public Player(Color color, Board board = null)
        {
            this.name = "Player " + ID;
            this.color = color;
            this.Board = board;
        }

        public Player(Player other) :
            this(other.color)
        {
            //can't clone our own board since we don't control it
            //the game itself is responsible for cloning the board and then assigning it to us
            this.Board = null;
        }

        public static Player CreateByCopy(Player player)
        {
            switch (player)
            {
                case Human human:
                    return new Human(human);
                case AI ai:
                    return new AI(ai);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
        
        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract Player Clone();

        public abstract void onTurn();
        
        internal List<Piece> findOwnPiecesOnBoard(Board boardSearched)
        {
            var matchingColorPieces = new List<Piece>();

            if (boardSearched != null)
            {
                foreach (var file in boardSearched)
                {
                    foreach (Square square in file)
                    {
                        if (square.isOccupied)
                        {
                            Piece piece = square.Piece.Value;

                            if (piece.color == this.color)
                            {
                                matchingColorPieces.Add(piece);
                            }
                        }
                    }
                }
            }

            return matchingColorPieces;
        }

        protected void initializePieces()
        {
            foreach (var piece in pieces)
            {
                CallBack removePiece = () => pieces.Remove(piece);
                piece.onCaptured += removePiece;
            }
        }

        public abstract MoveAction decideNextMove();

        public List<MoveAction> findPossibleMoves()
        {
            var moves = new List<MoveAction>();
            
            foreach (var piece in pieces)
            {
                var movesForPiece = findPossibleMovesForPiece(piece);
                moves.AddRange(movesForPiece);
            }

            return moves;
        }
        
        public virtual List<MoveAction> findBestMoves()
        {
            var moves = new List<MoveAction>();
            
            foreach (var piece in pieces)
            {
                Optional<MoveAction> bestMoveForPiece = findBestMoveForPiece(piece);

                if (bestMoveForPiece.HasValue)
                {
                    moves.Add(bestMoveForPiece.Value);
                }
            }

            return moves;
        }

        public List<MoveAction> findPossibleMovesForPiece(Piece piece)
        {
            var moves = new List<MoveAction>();
            
            List<Square> moveDestinations = piece.findAllPossibleLegalMoveDestinations();
            
            foreach (var moveDestination in moveDestinations)
            {
                var move = new MoveAction(this, piece, moveDestination);
                
                moves.Add(move);
            }
            
            return moves;
        }

        public virtual Optional<MoveAction> findBestMoveForPiece(Piece piece)
        {
            List<MoveAction> moves = findPossibleMovesForPiece(piece);

            moves = moves.extractHighestValueSubset();

            if (moves.Any())
            {
                return moves.selectElementAtRandom();
            }
            else
            {
                return new Optional<MoveAction>(null);
            }
        }
        
        public Tree<MoveAction> computeMoveDecisionTree()
        {
            throw new NotImplementedException();
        }
    }
}