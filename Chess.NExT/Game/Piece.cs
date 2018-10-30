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

        RankFile BoardPosition { get; }

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

    public static class PieceDefaults
    {
        public static bool canMove(this IPiece piece) 
        {
            var moves = piece.FindAllPossibleLegalMoveDestinations();
    
            bool canMove = (moves.Count > 0);
    
            return canMove;
        }
        
        public static void move(this IPiece piece, Chess.Game.Square destination) 
        {
            piece.PieceMovingNotifier?.Invoke();
    
            destination.receiveArrivingPiece(piece);
    
            piece.MovesMade++;
        }
        
        public static List<Chess.Game.Square> findAllPossibleLegalMoveDestinations(this IPiece piece) 
        {
            Predicate<Chess.Game.Square> squareChecker = (Chess.Game.Square squareToCheck) =>
            {
                if (squareToCheck.isEmpty)
                {
                    return true;
                }
                else /* if (squareToCheck.isOccupied) */ 
                {
                    return piece.Color.getOpposite() == squareToCheck.Piece.Object.Color;
                }
            };
                
            // ReSharper disable once PossibleInvalidOperationException
            List<Chess.Game.Square> squaresLegalToMove = piece.Board.SearchForSquares(
                squareMatcher: squareChecker, 
                startingSquarePosition: piece.BoardPosition, 
                directions: piece.LegalMovementDirections.ToArray(), 
                distance: piece.MaximumMoveDistance);
    
            return squaresLegalToMove;
        }
        
        public static void updateStateToHandleAssignmentToNewSquare(this IPiece piece) 
        {
            piece.recordCurrentPosition();
        }
        
        public static void recordCurrentPosition(this IPiece piece)
        {
            var currentPosition = new RankFile(piece.BoardPosition);
            piece.PositionHistory.Add(currentPosition);
        }
    }
    
    namespace Simulation
    {
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
    
            protected Optional<Simulation.Square> square;
            
            public virtual Chess.Game.Square Square 
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
            
            public Chess.Game.Board Board
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
    
            public CallBack PostCapturedActions { get; set; }
    
            public CallBack PieceMovingNotifier { get; set; }
            
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
                    case IKnight knight: {
                        return new Knight(knight);
                    }
                    case IBishop bishop: {
                        return new Bishop(bishop);
                    }
                    case IRook rook: {
                        return new Rook(rook);
                    }
                    case IQueen queen: {
                        return new Queen(queen);
                    }
                    case IKing king: {
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
            
            protected Piece(IPiece other):
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
    
            public abstract IPiece Clone();
    
            /// <summary>
            /// 
            /// </summary>
            /// <returns>true if there exists at least one Square that this Piece can legally move to, false otherwise</returns>
            public bool CanMove()
            {
                return this.canMove();
            }
            
            public virtual void Move(Chess.Game.Square destination)
            {
                this.move(destination);
            }
            
            public virtual void Move(RankFile destination) 
            {
                Chess.Game.Square destinationSquare = Board[destination];
                Move(destinationSquare);
            }
    
            public virtual List<Chess.Game.Square> FindAllPossibleLegalMoveDestinations()
            {
                return this.findAllPossibleLegalMoveDestinations();
            }

            public void UpdateStateToHandleAssignmentToNewSquare()
            {
                this.updateStateToHandleAssignmentToNewSquare();
            }
        }
    }

    namespace Graphical 
    {
        public abstract class Piece : IPiece, ChessDrawable 
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
    
            protected Optional<Graphical.Square> square;
            
            public Chess.Game.Square Square 
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
            
            public Chess.Game.Board Board
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
    
            public CallBack PostCapturedActions { get; set; }
    
            public CallBack PieceMovingNotifier { get; set; }
            public string SpriteImageFilePath { get; set; }

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

            protected Piece(char symbol, Color color, string spriteImageFilePath) 
            {
                this.Symbol = symbol;
                this.Color  = color;
                this.SpriteImageFilePath = spriteImageFilePath;
            }
            
            protected Piece(IPiece other) : 
                this(other.Symbol, other.Color, "")
            {
                
            }
            
            protected Piece(Graphical.Piece other) : 
                this(other.Symbol, other.Color, other.SpriteImageFilePath)
            {
                
            }
            
                        
            object ICloneable.Clone()
            {
                return Clone();
            }
    
            public abstract IPiece Clone();
            
            /// <summary>
            /// 
            /// </summary>
            /// <returns>true if there exists at least one Square that this Piece can legally move to, false otherwise</returns>
            public bool CanMove()
            {
                return this.canMove();
            }
            
            public virtual void Move(Chess.Game.Square destination)
            {
                this.move(destination);
            }
            
            public virtual void Move(RankFile destination) 
            {
                Chess.Game.Square destinationSquare = Board[destination];
                Move(destinationSquare);
            }
            
            public void InitializeGraphicalElements() 
            {
                var spriteTexture = new Texture(SpriteImageFilePath);
                Sprite = new Sprite(spriteTexture);
                Sprite.Scale = Graphical.Square.CalculateScalingFromBoardResolution(this.Size);
            }

            public void Initialize2DCoordinates(Vec2<uint> coordinates = default)
            {
                update2DPosition();
            }

            public void Draw(RenderTarget renderer)
            {
                renderer.Draw(Sprite);
            }
            
            public virtual List<Chess.Game.Square> FindAllPossibleLegalMoveDestinations()
            {
                return this.findAllPossibleLegalMoveDestinations();
            }

            public void UpdateStateToHandleAssignmentToNewSquare()
            {
                this.updateStateToHandleAssignmentToNewSquare();
                update2DPosition();
            }
        
            protected void update2DPosition()
            {
                this.Coordinates2D = (Square2D.Coordinates2D / 2) / 2;
            }
        }
    }
}