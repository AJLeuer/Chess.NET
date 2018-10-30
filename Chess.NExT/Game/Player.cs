using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Util;
using Tree;

namespace Chess.Game
{
    public abstract class Player : ICloneable
    {
        
        protected static ulong uniqueIDs = 0;

        protected ulong ID { get; } = uniqueIDs++;
        
        public string Name { get; }
    
        public virtual Color Color { get; }

        public uint MovesMade { get; private set; } = 0;

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

        public List<IPiece> pieces { get; private set; }

        /* Any other constructors should call this as a delegating constructor */
        public Player(Color color, Board board = null)
        {
            this.Name = "Player " + ID;
            this.Color = color;
            this.Board = board;
        }

        public Player(Player other) :
            this(other.Color)
        {
            //can't clone our own board since we don't control it
            //the game itself is responsible for cloning the board and then assigning it to us
            this.Board = null;
        }
        
        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract Player Clone();
        
        internal List<IPiece> findOwnPiecesOnBoard(Board anotherBoard)
        {
            var matchingColorPieces = new List<IPiece>();

            if (anotherBoard != null)
            {
                foreach (var square in anotherBoard.Squares)
                {    
                    if (square.isOccupied)
                    {
                        IPiece piece = square.Piece.Object;

                        if (piece.Color == this.Color)
                        {
                            matchingColorPieces.Add(piece);
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
                piece.PostCapturedActions += removePiece;
            }
        }

        public Move DecideNextMove()
        {
            MovesMade++;
            return ComputeNextMove();
        }

        public abstract Move ComputeNextMove();
        
        public List<Move> FindPossibleMoves()
        {
            var moves = new List<Move>();
            
            foreach (var piece in pieces)
            {
                var movesForPiece = FindAllPossibleMovesForPiece(piece);
                moves.AddRange(movesForPiece);
            }

            return moves;
        }
        
        public virtual List<Move> FindBestMoves()
        {
            var moves = new List<Move>();
            
            foreach (var piece in pieces)
            {
                Optional<Move> bestMoveForPiece = FindBestMoveForPiece(piece);

                if (bestMoveForPiece.HasValue)
                {
                    moves.Add(bestMoveForPiece.Object);
                }
            }

            return moves;
        }

        public List<Move> FindAllPossibleMovesForPiece(IPiece piece)
        {
            var moves = new List<Move>();
            
            List<Square> moveDestinations = piece.FindAllPossibleLegalMoveDestinations();
            
            foreach (var moveDestination in moveDestinations)
            {
                var move = new Move(this, piece, moveDestination);
                
                moves.Add(move);
            }
            
            return moves;
        }

        public virtual Optional<Move> FindBestMoveForPiece(IPiece piece)
        {
            List<Move> moves = FindAllPossibleMovesForPiece(piece);

            moves = moves.ExtractHighestValueSubset();

            if (moves.Any())
            {
                return moves.SelectElementAtRandom();
            }
            else
            {
                return new Optional<Move>(null);
            }
        }
        
        public Tree<Move> ComputeMoveDecisionTree()
        {
            throw new NotImplementedException();
        }
    }
}