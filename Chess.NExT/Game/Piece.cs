using System;
using System.Collections.Generic;
using Chess.Util;
using Chess.View;
using SFML.Graphics;
using static Chess.Game.Color;
using static Chess.Game.BasicGame;

using Position = Chess.Util.Vec2<uint>;

namespace Chess.Game
{
    public abstract class Piece : ICloneable, ChessDrawable
    {
        public static readonly Dictionary<Color, Char> defaultSymbols = new Dictionary<Color, Char>();

        public static readonly Dictionary<Color, string> defaultImageFiles = new Dictionary<Color, string>();
   
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
        
        public Square Square {
            set
            {
                this.square = value;
                
                if (square.HasValue)
                {
                    
                    PostAssignedToSquareActions?.Invoke();
                    
                    if (MovesMade == 0)
                    {
                        //this.startingPosition = new Position(Square.BoardPosition);
                    } 
                    
                }
            }
            get { return square.Object; } 
        }
        
        public Board Board
        {
            get { return Square.Board; }
        }

        protected Position startingPosition { get; set; }

        public RankFile BoardPosition
        {
            get { return Square.BoardPosition; }
        }
        
        public CallBack PostCapturedActions { get; set; }
        
        public CallBack PostAssignedToSquareActions { get; set; }
        
        protected string spriteImageFilePath;

        public Sprite Sprite { get; set; }
        
        public Size Size
        {
            get { return Sprite.Texture.Size; }
        }
        
        public Position Position2D
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
        
        public static Piece create(char symbol) {
            Piece piece;
            if (symbol == Pawn.defaultSymbols[white]) {
                piece = new Pawn(white);
            }
            else if (symbol == Pawn.defaultSymbols[black]) {
                piece = new Pawn(black);
            }
            else if (symbol == Knight.defaultSymbols[white]) {
                piece = new Knight(white);
            }
            else if (symbol == Knight.defaultSymbols[black]) {
                piece = new Knight(black);
            }
            else if (symbol == Bishop.defaultSymbols[white]) {
                piece = new Bishop(white);
            }
            else if (symbol == Bishop.defaultSymbols[black]) {
                piece = new Bishop(black);
            }
            else if (symbol == Rook.defaultSymbols[white]) {
                piece = new Rook(white);
            }
            else if (symbol == Rook.defaultSymbols[black]) {
                piece = new Rook(black);
            }
            else if (symbol == Queen.defaultSymbols[white]) {
                piece = new Queen(white);
            }
            else if (symbol == Queen.defaultSymbols[black]) {
                piece = new Queen(black);
            }
            else if (symbol == King.defaultSymbols[white]) {
                piece = new King(white);
            }
            else if (symbol == King.defaultSymbols[black]) {
                piece = new King(black);
            }
            else {
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
        public static Piece create(Piece piece)
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
        
        protected Piece(char symbol, Color color, string spriteImageFilePath)
        {
            this.Symbol = symbol;
            this.Color  = color;
            this.spriteImageFilePath = spriteImageFilePath;

            PostAssignedToSquareActions += this.calculate2DPosition;
        }
        
        protected Piece(Piece other):
            this(other.Symbol, other.Color, other.spriteImageFilePath)
        {
            /* Don't initialize the actual texture/sprite data. Too
             expensive - we only init it when we need it (lazily) */

            /* Don't copy other's square references: we don't know if we're owned
             by a new board or still held by the same, and if we are on the new square/board,
             they'll have to update our references */
        }
        
        ~Piece() {
            Square = null;
        }
        
        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract Piece Clone();

        public void InitializeGraphicalElements() 
        {
            var spriteTexture = new Texture(spriteImageFilePath);
            Sprite = new Sprite(spriteTexture);
        }

        public void Initialize2DPosition(Position position = default)
        {
            calculate2DPosition();
        }
        
        protected void calculate2DPosition()
        {
            this.Position2D = (Square.Position2D / 2) / 2;
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
            Square.handleLeavingPiece();

            destination.receiveArrivingPiece(this);

            MovesMade++;
        }
        
        public virtual void Move(RankFile destination) 
        {
            Square destinationSquare = Board[destination];
            Move(destinationSquare);
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
            
            List<Square> squaresLegalToMove = Board.SearchForSquares(
                squareMatcher: squareChecker, 
                startingSquarePosition: this.BoardPosition, 
                directions: LegalMovementDirections.ToArray(), 
                distance: MaximumMoveDistance);

            return squaresLegalToMove;
        }
    }
}