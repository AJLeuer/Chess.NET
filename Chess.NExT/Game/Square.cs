using System;
using Chess.Util;
using Chess.View;
using SFML.Graphics;

using File = System.Char;
using Rank = System.UInt16;
using Position = Chess.Util.Vec2<uint>;

namespace Chess.Game
{
    public class Square : ICloneable
    {
        public Position BoardPosition { get; protected set; }
        
        public RankFile RankAndFile
        {
            get { return BoardPosition; }
        }

        private Optional<Color> color = Optional<Color>.Empty;

        public Color Color
        {
            get
            {
                if (color.HasValue == false)
                {
                    color = determineColor();
                }

                return color.Object;
            }
        }

        public Board Board { get; set; }

        private Piece piece = null;
        
        public virtual Optional<Piece> Piece 
        {
            get { return piece; }
            set
            {
                if (value.HasValue == false)
                {
                    if (piece != null)
                    {
                        piece.Square = null;
                    }
                    this.piece = null;
                }
                else
                {
                    this.piece = value.Object;
                    piece.Square = this;
                    handleNewPiece();
                }
            }
        }

        public bool isEmpty
        {
            get { return (isOccupied == false); }
        }

        public bool isOccupied
        {
            get { return Piece.HasValue; }
        }

        public Square(File file, Rank rank):
            this(null, file, rank)
        {
            
        }

        public Square(RankFile rankAndFile) :
            this(null, rankAndFile)
        {
            
        }
        
        public Square(Piece piece, File file, Rank rank)
        {
            BoardPosition = new RankFile(file, rank);
            Piece = new Optional<Piece>(piece);
        }
        
        public Square(Piece piece, RankFile rankAndFile):
            this(piece, rankAndFile.File, rankAndFile.Rank)
        {
            
        }
        
        public Square(char pieceSymbol, File file, Rank rank) :
            this(Game.Piece.Create(pieceSymbol), file, rank)
        {
            
        }
        
        public Square(char pieceSymbol, RankFile rankAndFile) :
            this(Game.Piece.Create(pieceSymbol), rankAndFile)
        {
            
        }
        
        public Square(Square other):
            this ((other.isEmpty) ? null : Game.Piece.Create(other.piece),
                   new RankFile(other.RankAndFile)) /* Don't copy other's board pointer */
        {
            
        }

        ~Square()
        {
            clearCurrentPiece();
        }
        
        object ICloneable.Clone()
        {
            return Clone();
        }

        public virtual Square Clone()
        {
            return new Square(this);
        }

        protected ref Optional<Color> determineColor()
        {
            uint coordinateSum = BoardPosition.X + BoardPosition.Y;
            
            color = (coordinateSum % 2) == 0 ? Color.black : Color.white;

            return ref color;
        }

        protected void handleLeavingPiece()
        {
            // ReSharper disable once DelegateSubtraction
            Piece.Object.PieceMovingNotifier -= this.handleLeavingPiece;
            clearCurrentPiece();
        }

        protected void handleNewPiece()
        {
            this.Piece.Object.PieceMovingNotifier += this.handleLeavingPiece;
        }

        public void receiveArrivingPiece(Piece arrivingPiece)
        {
            if (this.isOccupied)
            {
                captureCurrentPiece();
            }

            this.Piece = arrivingPiece; //calls handleNewPiece()
        }

        protected void captureCurrentPiece()
        {
            piece.PostCapturedActions?.Invoke();
            clearCurrentPiece();
        }

        protected void clearCurrentPiece()
        {
            Piece = Optional<Piece>.Empty;
        }

        protected static Vec2<double> calculateScalingFromBoardResolution(Size unscaledSizeOfSquare)
        {
            ushort numberOfSquaresHorizontal = (ushort) Game.Board.DefaultStartingSquares.GetLength(0);
            ushort numberOfSquaresVertical = (ushort) Game.Board.DefaultStartingSquares.GetLength(1);
            
            uint targetWidthForSquare = Chess.Util.Config.BoardResolution.Width / numberOfSquaresHorizontal;
            uint targetHeightForSquare = Chess.Util.Config.BoardResolution.Height / numberOfSquaresVertical;

            Size targetSizeForSquare = new Size {Width = targetWidthForSquare, Height = targetHeightForSquare};

            Vec2<double> scaleFactor = targetSizeForSquare / unscaledSizeOfSquare;

            return scaleFactor;
        }

    }
    
    namespace Graphical
    {
        public class Square : Chess.Game.Square, ChessDrawable
        {
            
            public override Optional<Chess.Game.Piece> Piece 
            {
                get { return base.Piece; }
                set
                {
                    if ((value.HasValue) && ((value.Object is Graphical.Piece) == false))
                    {
                        throw new ArgumentException("A Piece belonging to a Graphical Square must be an instance of the subtype Graphical.Piece");   
                    }
                    else
                    {
                        base.Piece = value;
                    }
                }
            }
            
            public Optional<Graphical.Piece> Piece2D
            {
                get
                {
                    if (base.Piece.HasValue)
                    {
                        return (Graphical.Piece) Piece.Object;
                    }
                    else
                    {
                        return Optional<Graphical.Piece>.Empty;
                    }
                }
            }

            public Sprite Sprite { get; set; }

            public Size Size
            {
                get { return Sprite.Texture.Size; }
            }

            public Vec2<uint> Coordinates2D
            {
                get
                {
                    if (Sprite != null)
                    {
                        return Sprite.Position;
                    }

                    return (0, 0);
                }
            
                set { Sprite.Position = value; }
            }
            
            public Square(char file, ushort rank) : 
                base(file, rank)
            {
            }

            public Square(RankFile rankAndFile) : 
                base(rankAndFile)
            {
            }

            public Square(Piece piece, char file, ushort rank) : 
                base(piece, file, rank)
            {
            }

            public Square(Piece piece, RankFile rankAndFile) : 
                base(piece, rankAndFile)
            {
            }

            public Square(char pieceSymbol, char file, ushort rank) : 
                this(Graphical.Piece.Create(pieceSymbol), file, rank)
            {
            }

            public Square(char pieceSymbol, RankFile rankAndFile) : 
                this(Graphical.Piece.Create(pieceSymbol), rankAndFile)
            {
            }

            public Square(Graphical.Square other) : 
                this (piece: (other.isEmpty) ? null : Graphical.Piece.Create(other.Piece2D.Object),
                      new RankFile(other.RankAndFile)) 
            {
            }
            
            public override Chess.Game.Square Clone()
            {
                return new Square(this);
            }
            
            public void InitializeGraphicalElements()
            {
                var spriteTexture = new Texture(Config.BoardSpriteFilePath);
                Sprite       = new Sprite(spriteTexture);
                Sprite.Scale = calculateScalingFromBoardResolution(Size);
            
                if (Piece2D.HasValue)
                {
                    Piece2D.Object.InitializeGraphicalElements();
                }
            }

            public void Initialize2DCoordinates(Vec2<uint> coordinates)
            {
                this.Coordinates2D = coordinates;

                if (base.Piece.HasValue)
                {
                    Piece2D.Object.Initialize2DCoordinates();
                }
            }
        }
    }

}