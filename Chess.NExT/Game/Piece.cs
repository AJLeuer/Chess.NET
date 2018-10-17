using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Util;
using Chess.View;
using SFML.Graphics;
using static Chess.Game.Color;
using static Chess.Game.BasicGame;

using Position = Chess.Util.Vec2<uint>;

namespace Chess.Game
{
    public abstract class Piece : ICloneable
    {
        protected static ulong IDs = 0;
        
        public ulong ID { get; } = IDs++;

        public char Symbol { get; }
        
        public abstract char ASCIISymbol { get; }

        public Color Color { get; }

        public abstract ushort Value { get; }
        
        public abstract List<Direction> LegalMovementDirections { get; }
        
        public virtual ushort MaximumMoveDistance { get { return MaximumPossibleMoveDistance; } }

        public uint MovesMade { get; protected set; } = 0;

        protected Optional<Square> square;
        
        public virtual Square Square 
        {
            set
            {
                this.square = value;
                
                if (square.HasValue)
                {
                    updateStateToHandleAssignmentToNewSquare();
                }
            }
            get { return square.Object; } 
        }
        
        public Board Board
        {
            get { return Square.Board; }
        }

        public Position StartingPosition
        {
            get
            {
                if (PositionHistory.Any())
                {
                    return this.PositionHistory.First();
                }
                else
                {
                    return BoardPosition;
                }
            }
        }

        public List<RankFile> PositionHistory { get; } = new List<RankFile>();

        public RankFile BoardPosition
        {
            get { return Square.BoardPosition; }
        }

        public CallBack PostCapturedActions;

        public CallBack PieceMovingNotifier;
        
        public static Piece Create(char symbol) 
        {
            Piece piece;
            if (symbol == Pawn.DefaultSymbols[white]) 
            {
                piece = new Pawn(white);
            }
            else if (symbol == Pawn.DefaultSymbols[black]) 
            {
                piece = new Pawn(black);
            }
            else if (symbol == Knight.DefaultSymbols[white]) 
            {
                piece = new Knight(white);
            }
            else if (symbol == Knight.DefaultSymbols[black]) 
            {
                piece = new Knight(black);
            }
            else if (symbol == Bishop.DefaultSymbols[white]) 
            {
                piece = new Bishop(white);
            }
            else if (symbol == Bishop.DefaultSymbols[black]) 
            {
                piece = new Bishop(black);
            }
            else if (symbol == Rook.DefaultSymbols[white]) 
            {
                piece = new Rook(white);
            }
            else if (symbol == Rook.DefaultSymbols[black]) 
            {
                piece = new Rook(black);
            }
            else if (symbol == Queen.DefaultSymbols[white]) 
            {
                piece = new Queen(white);
            }
            else if (symbol == Queen.DefaultSymbols[black]) 
            {
                piece = new Queen(black);
            }
            else if (symbol == King.DefaultSymbols[white]) 
            {
                piece = new King(white);
            }
            else if (symbol == King.DefaultSymbols[black]) 
            {
                piece = new King(black);
            }
            else 
            {
                piece = null;
            }
            return piece;
        }
        
        /// <summary>
        ///    Creates a new piece by copying the piece passed
        /// </summary>
        /// <param name="piece">
        ///     The piece to copy
        /// </param>
        /// <returns>A copy of piece</returns>
        /// <exception cref="TypeInitializationException"></exception>
        public static Piece Create(Piece piece)
        {
            switch (piece) 
            {
                case Pawn pawn:
                {
                    return new Pawn(pawn);
                }
                case Knight knight: {
                    return new Knight(knight);
                }
                case Bishop bishop: {
                    return new Bishop(bishop);
                }
                case Rook rook: {
                    return new Rook(rook);
                }
                case Queen queen: {
                    return new Queen(queen);
                }
                case King king: {
                    return new King(king);
                }
                default: {
                    throw new TypeInitializationException(piece.GetType().Name, new Exception("Unknown subclass of Piece"));
                }
            }
        }
        
        protected Piece(char symbol, Color color)
        {
            this.Symbol = symbol;
            this.Color  = color;
        }
        
        protected Piece(Piece other):
            this(other.Symbol, other.Color)
        {
            /* Don't initialize the actual texture/sprite data. Too
             expensive - we only init it when we need it (lazily) */

            /* Don't copy other's square references: we don't know if we're owned
             by a new board or still held by the same, and if we are on the new square/board,
             they'll have to update our references */
        }
        
        ~Piece() 
        {
            Square = null;
        }
        
        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract Piece Clone();

        protected void recordCurrentPosition()
        {
            var currentPosition = new RankFile(this.BoardPosition);
            PositionHistory.Add(currentPosition);
        }
	    
        /**
        * Returns true if there exists at least one Square that this Piece can legally move to,
        * false otherwise
        */
        public bool CanMove()
        {
            var moves = FindAllPossibleLegalMoveDestinations();

            bool canMove = (moves.Count > 0);

            return canMove;
        }
        
        public virtual void Move(Square destination) 
        {
            PieceMovingNotifier?.Invoke();

            destination.receiveArrivingPiece(this);

            MovesMade++;
        }
        
        public virtual void Move(RankFile destination) 
        {
            Square destinationSquare = Board[destination];
            Move(destinationSquare);
        }

        protected virtual void updateStateToHandleAssignmentToNewSquare()
        {
            recordCurrentPosition();
        }

        public virtual List<Square> FindAllPossibleLegalMoveDestinations()
        {
            Predicate<Square> squareChecker = (Square squareToCheck) =>
            {
                if (squareToCheck.isEmpty)
                {
                    return true;
                }
                else /* if (squareToCheck.isOccupied) */ 
                {
                    return this.Color.getOpposite() == squareToCheck.Piece.Object.Color;
                }
            };
            
            // ReSharper disable once PossibleInvalidOperationException
            List<Square> squaresLegalToMove = Board.SearchForSquares(
                squareMatcher: squareChecker, 
                startingSquarePosition: this.BoardPosition, 
                directions: LegalMovementDirections.ToArray(), 
                distance: MaximumMoveDistance);

            return squaresLegalToMove;
        }
    }

    namespace Graphical
    {
        public abstract class Piece : Game.Piece, ChessDrawable
        {
            protected string spriteImageFilePath { get; }

            public override Game.Square Square
            {
                get { return base.Square; }
                set
                {
                    if (value.GetType() != typeof(Graphical.Square))
                    {
                        throw new ArgumentException("A Graphical Piece can only be owned by a Graphical Square");
                    }
                    else
                    {
                        base.Square = value;
                    }
                }
            }

            public Graphical.Square Square2D { get { return (Graphical.Square) Square; } }
            
            public Sprite Sprite { get; set; }
        
            public Size Size
            {
                get { return Sprite.Texture.Size; }
            }
        
            public Position Coordinates2D
            {
                get { return Sprite.Position; }
            
                set
                {
                    if (Sprite != null)
                    {
                        Sprite.Position = value;
                    }
                }
            }
            
            public new static Graphical.Piece Create(char symbol) 
            {
                Piece piece;
                if (symbol == Pawn.DefaultSymbols[white]) 
                {
                    piece = new Pawn(white);
                }
                else if (symbol == Pawn.DefaultSymbols[black]) 
                {
                    piece = new Pawn(black);
                }
                else if (symbol == Knight.DefaultSymbols[white])
                {
                    piece = new Knight(white);
                }
                else if (symbol == Knight.DefaultSymbols[black])
                {
                    piece = new Knight(black);
                }
                else if (symbol == Bishop.DefaultSymbols[white])
                {
                    piece = new Bishop(white);
                }
                else if (symbol == Bishop.DefaultSymbols[black])
                {
                    piece = new Bishop(black);
                }
                else if (symbol == Rook.DefaultSymbols[white]) 
                {
                    piece = new Rook(white);
                }
                else if (symbol == Rook.DefaultSymbols[black]) 
                {
                    piece = new Rook(black);
                }
                else if (symbol == Queen.DefaultSymbols[white])
                {
                    piece = new Queen(white);
                }
                else if (symbol == Queen.DefaultSymbols[black])
                {
                    piece = new Queen(black);
                }
                else if (symbol == King.DefaultSymbols[white]) 
                {
                    piece = new King(white);
                }
                else if (symbol == King.DefaultSymbols[black]) 
                {
                    piece = new King(black);
                }
                else 
                {
                    piece = null;
                }
                return piece;
            }
        
            /// <summary>
            ///    Creates a new piece by copying the piece passed
            /// </summary>
            /// <param name="piece">
            ///     The piece to copy
            /// </param>
            /// <returns>A copy of piece</returns>
            /// <exception cref="TypeInitializationException"></exception>
            public static Graphical.Piece Create(Graphical.Piece piece)
            {
                switch (piece) 
                {
                    case Pawn pawn:
                    {
                        return new Pawn(pawn);
                    }
                    case Knight knight: 
                    {
                        return new Knight(knight);
                    }
                    case Bishop bishop: 
                    {
                        return new Bishop(bishop);
                    }
                    case Rook rook: 
                    {
                        return new Rook(rook);
                    }
                    case Queen queen: 
                    {
                        return new Queen(queen);
                    }
                    case King king: 
                    {
                        return new King(king);
                    }
                    default: 
                    {
                        throw new TypeInitializationException(piece.GetType().Name, new Exception("Unknown subclass of Piece"));
                    }
                }
            }

            protected Piece(char symbol, Color color, string spriteImageFilePath) : 
                base(symbol, color)
            {
                this.spriteImageFilePath = spriteImageFilePath;
            }

            protected Piece(Graphical.Piece other) : 
                this(other.Symbol, other.Color, other.spriteImageFilePath)
            {
            }

            public abstract override Game.Piece Clone();
            
            public void InitializeGraphicalElements() 
            {
                var spriteTexture = new Texture(spriteImageFilePath);
                Sprite = new Sprite(spriteTexture);
            }

            public void Initialize2DCoordinates(Vec2<uint> coordinates = default)
            {
                update2DPosition();
            }
            
            protected override void updateStateToHandleAssignmentToNewSquare()
            {
                base.updateStateToHandleAssignmentToNewSquare();
                update2DPosition();
            }
        
            protected void update2DPosition()
            {
                this.Coordinates2D = (Square2D.Coordinates2D / 2) / 2;
            }
        }
    }
}