using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

using Chess.Utility;

namespace Chess.Game
{
    public abstract class Player : GameEntity, ICloneable 
    {
        protected static ulong uniqueIDs = 0;

        protected ulong ID { get; } = uniqueIDs++;
        
        public string Name { get; }
    
        public virtual Color Color { get; }

        public uint MovesMade { get; protected set; } = 0;
        
        public BasicGame Game { get { return Board.Game; }}

        private Board board;
        
        public Board Board {
            get { return board; }

            set
            {
                this.board = value;
                this.Pieces = findOwnPiecesOnBoard(board);
                initializePieces();
            }
        }

        public HashSet<IPiece> Pieces { get; private set; }

        /* Any other constructors should call this as a delegating constructor */
        public Player(Color color) 
        {
            this.Name = "Player " + ID;
            this.Color = color;
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
        
        internal HashSet<IPiece> findOwnPiecesOnBoard(Board anotherBoard)
        {
            var matchingColorPieces = new HashSet<IPiece>();

            if (anotherBoard != null)
            {
                foreach (var square in anotherBoard.Squares)
                {    
                    if (square.IsOccupied)
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
            foreach (var piece in Pieces)
            {
                CallBack removePiece = () => Pieces.Remove(piece);
                piece.PostCapturedActions += removePiece;
                piece.Player = this;
            }
        }

        public abstract Move DecideNextMove();

        protected abstract Move decideNextMove();
        
        public List<Move> FindPossibleMoves() 
        {
            var moves = new List<Move>();
            
            foreach (var piece in Pieces)
            {
                var movesForPiece = FindAllPossibleMovesForPiece(piece);
                moves.AddRange(movesForPiece);
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
        
        protected Optional<Move> findBestMoveForPiece(IPiece piece) 
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

    }
    
    namespace Real 
    {
        public abstract class Player : Chess.Game.Player
        {
            protected readonly Timer playerMoveTimer = new Timer();
            public List<Duration> TimeSpentDecidingEachMove { get; private set; } = new List<Duration>();
            
            public Duration AverageTimeToDecideMove
            {
                get { return TimeSpentDecidingEachMove.ComputeAverage(); }
            }
            
            protected Player(Color color) : 
                base(color)
            {
            }

            protected Player(Chess.Game.Player other) : 
                base(other)
            {
            }
            
            public override Move DecideNextMove()
            {
                MovesMade++;
                playerMoveTimer.Start();
                Move nextMove = decideNextMove();
                TimeSpentDecidingEachMove.Add(playerMoveTimer.Stop());
                return nextMove;
            }

        }
	}

    namespace Simulation 
    {
        public abstract class Player : Chess.Game.Player
        {
            protected Player(Color color) : 
                base(color)
            {
            }

            protected Player(Chess.Game.Player other) : 
                base(other)
            {
            }
            
            public override Move DecideNextMove() 
            {
                MovesMade++;
                Move nextMove = decideNextMove();
                return nextMove;
            }

        }
    }
    
    public class NoRemainingMovesException : Exception
    {
        
    }
}