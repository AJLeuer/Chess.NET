using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Util;
using Chess.View;
using SFML.Graphics;
using static Chess.Game.Color;
using static Chess.Game.BasicGame;
using static Chess.Util.Config;

using Position = Chess.Util.Vec2<uint>;

namespace Chess.Game
{
    public interface IPiece : ICloneable 
    {
        ulong ID { get; }
        
        char Symbol { get; }
        
        char ASCIISymbol { get; }

        Color Color { get; }

        ushort Value { get; }
        
        List<Direction> LegalMovementDirections { get; }
        
        ushort MaximumMoveDistance { get; }

        uint MovesMade { get; set; }

        Square Square { get; set; }

        Board Board { get; }

        Position StartingPosition { get; }

        List<RankFile> PositionHistory { get; }

        RankFile RankAndFile { get; }

        CallBack PostCapturedActions { get; set; }

        CallBack PieceMovingNotifier { get; set; }

        new IPiece Clone();
        
        /**
        * Returns true if there exists at least one Square that this Piece can legally move to,
        * false otherwise
        */
        bool CanMove();

        void Move(Square destination);

        void Move(RankFile destination);

        List<Square> FindAllPossibleLegalMoveDestinations();

        void UpdateStateToHandleAssignmentToNewSquare();
    }

    public abstract class Piece : IPiece
    {
        protected static ulong IDs = 0;
            
        public ulong ID { get; } = IDs++;
    
        public char Symbol { get; }
            
        public abstract char ASCIISymbol { get; }
    
        public Color Color { get; }
    
        public abstract ushort Value { get; }
            
        public abstract List<Direction> LegalMovementDirections { get; }
            
        public virtual ushort MaximumMoveDistance { get { return MaximumPossibleMoveDistance; } }
    
        public uint MovesMade { get; set; } = 0;

        public abstract Square Square { get; set; }
        
        public Chess.Game.Board Board 
        {
            get { return this.Square.Board; }
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
                    return RankAndFile;
                }
            }
        }
    
        public List<RankFile> PositionHistory { get; } = new List<RankFile>();
        
        public RankFile RankAndFile 
        {
            get { return Square.BoardPosition; }
        }
            
        public CallBack PostCapturedActions { get; set; }
    
        public CallBack PieceMovingNotifier { get; set; }

        protected Piece(char symbol, Color color)
        {
            this.Symbol = symbol;
            this.Color = color;
        }

        protected Piece(IPiece other) :
            this(other.Symbol, other.Color)
        {
            
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public abstract IPiece Clone();
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if there exists at least one Square that this Piece can legally move to, false otherwise</returns>
        public bool CanMove()
        {
            var moves = FindAllPossibleLegalMoveDestinations();
    
            bool canMove = (moves.Count > 0);
    
            return canMove;
        }
        
        public virtual void Move(Chess.Game.Square destination)
        {
            PieceMovingNotifier?.Invoke();
    
            destination.ReceiveArrivingPiece(this);
    
            MovesMade++;
        }
        
        public virtual void Move(RankFile destination) 
        {
            Chess.Game.Square destinationSquare = Board[destination];
            Move(destinationSquare);
        }
        
        public virtual List<Chess.Game.Square> FindAllPossibleLegalMoveDestinations()
        {
            Predicate<Chess.Game.Square> squareChecker = (Chess.Game.Square squareToCheck) =>
            {
                if (squareToCheck.IsEmpty)
                {
                    return true;
                }
                else /* if (squareToCheck.isOccupied) */ 
                {
                    return this.Color.GetOpposite() == squareToCheck.Piece.Object.Color;
                }
            };
                
            // ReSharper disable once PossibleInvalidOperationException
            List<Chess.Game.Square> squaresLegalToMove = this.Board.SearchForSquares(
                squareMatcher: squareChecker, 
                startingSquarePosition: this.RankAndFile, 
                directions: this.LegalMovementDirections.ToArray(), 
                distance: this.MaximumMoveDistance);
    
            return squaresLegalToMove;
        }
        
        public virtual void UpdateStateToHandleAssignmentToNewSquare()
        {
            var currentPosition = new RankFile(this.RankAndFile);
            this.PositionHistory.Add(currentPosition);
        }
    }
    
    namespace Simulation 
    {
        public abstract class Piece : Chess.Game.Piece 
        {    
            protected Optional<Simulation.Square> square;
            
            public override Chess.Game.Square Square 
            {
                set
                {
                    if ((value != null) && ((value is Simulation.Square) == false))
                    {
                        throw new ArgumentException("A Simulation Piece can only be owned by a Simulation Square");
                    }
                    else
                    {
                        this.square = (Simulation.Square) value;
                    }
                    if (square.HasValue)
                    {
                        this.UpdateStateToHandleAssignmentToNewSquare();
                    }
                }
                get { return square.Object; } 
            }
            
            public static Simulation.Piece Create(char symbol) 
            {
                Piece piece;
                if (symbol == PawnDefaults.DefaultSymbols[white]) 
                {
                    piece = new Pawn(white);
                }
                else if (symbol == PawnDefaults.DefaultSymbols[black]) 
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
            public static Simulation.Piece Create(IPiece piece) 
            {
                switch (piece) 
                {
                    case IPawn pawn:
                    {
                        return new Pawn(pawn);
                    }
                    case IKnight knight: 
                    {
                        return new Knight(knight);
                    }
                    case IBishop bishop: 
                    {
                        return new Bishop(bishop);
                    }
                    case IRook rook: 
                    {
                        return new Rook(rook);
                    }
                    case IQueen queen: 
                    {
                        return new Queen(queen);
                    }
                    case IKing king: 
                    {
                        return new King(king);
                    }
                    default: 
                    {
                        throw new TypeInitializationException(piece.GetType().Name, new Exception("Unknown subclass of Piece"));
                    }
                }
            }
            
            protected Piece(char symbol, Color color):
                base(symbol, color)
            {
                
            }
            
            protected Piece(IPiece other):
                base(other.Symbol, other.Color)
            {
                
            }
            
            ~Piece() 
            {
                Square = null;
            }
    
            public abstract override IPiece Clone();
        }
    }

    namespace Graphical 
    {
        public abstract class Piece : Chess.Game.Piece, ChessDrawable 
        {
            protected Optional<Graphical.Square> square;
            
            public override Chess.Game.Square Square 
            {
                get { return square.Object; }
                
                set
                {
                    if ((value != null) && ((value is Graphical.Square) == false))
                    {
                        throw new ArgumentException("A Graphical Piece can only be owned by a Graphical Square");
                    }
                    else
                    {
                        this.square = (Graphical.Square) value;
                    }
                    if (square.HasValue)
                    {
                        UpdateStateToHandleAssignmentToNewSquare();
                    }
                }
            }

            public string SpriteImageFilePath { get; set; }

            public Graphical.Square Square2D { get { return (Graphical.Square) Square; } }
            
            public Sprite Sprite { get; set; }
        
            public Size Size 
            {
                get { return Sprite.GetActualSize(); }
            }
        
            public Position OriginCoordinates 
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
            
            public Vec2<uint> CenterCoordinates 
            {
                get
                {
                    if (Sprite != null)
                    {
                        uint x = (uint)(Sprite.Position.X + (Sprite.GetActualSize().Width  / 2f));
                        uint y = (uint)(Sprite.Position.Y + (Sprite.GetActualSize().Height / 2f));

                        return new Vec2<uint>(x, y);
                    }

                    return (0, 0);
                }
                set
                {
                    uint x = (uint)(value.X - (Sprite.GetActualSize().Width  / 2));
                    uint y = (uint)(value.Y - (Sprite.GetActualSize().Height / 2));
                    
                    Sprite.Position = new Vec2<uint>(x, y);
                }
            }

            public static Graphical.Piece Create(char symbol) 
            {
                Piece piece;
                if (symbol == PawnDefaults.DefaultSymbols[white]) 
                {
                    piece = new Pawn(white);
                }
                else if (symbol == PawnDefaults.DefaultSymbols[black]) 
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
            public static Graphical.Piece Create(IPiece piece) 
            {
                switch (piece) 
                {
                    case IPawn pawn:
                    {
                        return new Pawn(pawn);
                    }
                    case IKnight knight: 
                    {
                        return new Knight(knight);
                    }
                    case IBishop bishop: 
                    {
                        return new Bishop(bishop);
                    }
                    case IRook rook: 
                    {
                        return new Rook(rook);
                    }
                    case IQueen queen: 
                    {
                        return new Queen(queen);
                    }
                    case IKing king: 
                    {
                        return new King(king);
                    }
                    default: 
                    {
                        throw new TypeInitializationException(piece.GetType().Name, new Exception("Unknown subclass of Piece"));
                    }
                }
            }

            protected Piece(char symbol, Color color, string spriteImageFilePath):
                base(symbol, color)
            {
                this.SpriteImageFilePath = spriteImageFilePath;
            }
            
            protected Piece(IPiece other) : 
                base(other.Symbol, other.Color)
            {
                this.SpriteImageFilePath = "";
            }
            
            protected Piece(Graphical.Piece other) : 
                base(other.Symbol, other.Color)
            {
                this.SpriteImageFilePath = other.SpriteImageFilePath;
            }
    
            public abstract override IPiece Clone();
            
            public void InitializeGraphicalElements() 
            {
                var spriteTexture = new Texture(SpriteImageFilePath);
                Sprite = new Sprite(spriteTexture);
                Sprite.Texture.Smooth = true;
                Sprite.Scale = calculateScalingFromSquareResolution();
            }

            public void Initialize2DCoordinates(Vec2<uint> coordinates = default)
            {
                update2DPosition();
            }

            public void Draw(RenderTarget renderer)
            {
                renderer.Draw(Sprite);
            }

            public override void UpdateStateToHandleAssignmentToNewSquare()
            {
                base.UpdateStateToHandleAssignmentToNewSquare();
                update2DPosition();
            }
        
            protected void update2DPosition()
            {
                if (Square2D.Sprite != null)
                {
                    this.CenterCoordinates = Square2D.CenterCoordinates;
                }
            }
            
            private Vec2<double> calculateScalingFromSquareResolution()
            {
                Size unscaledSize = Sprite.Texture.Size;
                
                uint largestDimensionLength = (unscaledSize.Height > unscaledSize.Width) ?
                    unscaledSize.Height : unscaledSize.Width;
                
                float targetLengthForLargestDimension  = Square2D.Size.AverageSideLength * PieceScaleRelativeToSquare;

                float requisiteScalingValueForTargetSize = targetLengthForLargestDimension / largestDimensionLength;

                Vec2<double> scaleFactor = new Vec2<double>(requisiteScalingValueForTargetSize, requisiteScalingValueForTargetSize);

                return scaleFactor;
            }

        }
    }
}