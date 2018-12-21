using System;
using System.Collections.Generic;
using Chess.Util;
using Chess.View;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using File = System.Char;
using Rank = System.UInt16;
using Position = Chess.Util.Vec2<uint>;
using static Chess.Game.Color;

namespace Chess.Game
{
    public abstract class Square : GameEntity, ICloneable 
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
        
        public BasicGame Game { get { return Board.Game; }}
        public Board Board { get; set; }

        private IPiece piece = null;
        
        public virtual Optional<IPiece> Piece 
        {
            get { return new Optional<IPiece>(piece); }
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

        public bool IsEmpty 
        {
            get { return (IsOccupied == false); }
        }

        public bool IsOccupied 
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
        
        public Square(IPiece piece, File file, Rank rank)
        {
            BoardPosition = new RankFile(file, rank);
            Piece = new Optional<IPiece>(piece);
        }
        
        public Square(IPiece piece, RankFile rankAndFile):
            this(piece, rankAndFile.File, rankAndFile.Rank)
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

        public abstract Square Clone();

        protected ref Optional<Color> determineColor() 
        {
            uint coordinateSum = BoardPosition.X + BoardPosition.Y;
            
            color = (coordinateSum % 2) == 0 ? black : white;

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

        public void ReceiveArrivingPiece(IPiece arrivingPiece) 
        {
            if (this.IsOccupied)
            {
                captureCurrentPiece();
            }

            this.Piece = new Optional<IPiece>(arrivingPiece); //calls handleNewPiece()
        }

        protected void captureCurrentPiece()
        {
            piece.PostCapturedActions?.Invoke();
            clearCurrentPiece();
        }

        protected void clearCurrentPiece()
        {
            Piece = Optional<IPiece>.Empty;
        }

        public static double CalculateDistanceBetween(Square firstSquare, Square secondSquare)
        {
            return Position.Distance(firstSquare.BoardPosition, secondSquare.BoardPosition);
        }
    }

    namespace Simulation
    {
        public class Square : Chess.Game.Square 
        {
            public Square(char file, ushort rank) : 
                base(file, rank)
            {
                
            }

            public Square(RankFile rankAndFile) : 
                base(rankAndFile)
            {
                
            }

            public Square(Chess.Game.IPiece piece, char file, ushort rank) : 
                base(piece, file, rank)
            {
                
            }

            public Square(Chess.Game.IPiece piece, RankFile rankAndFile) : 
                base(piece, rankAndFile)
            {
                
            }

            public Square(char pieceSymbol, char file, ushort rank) : 
                this(Simulation.Piece.Create(pieceSymbol), file, rank)
            {
                
            }

            public Square(char pieceSymbol, RankFile rankAndFile) : 
                this(Simulation.Piece.Create(pieceSymbol), rankAndFile)
            {
            }

            public Square(Chess.Game.Square other) : 
                this (piece: (other.IsEmpty) ? null : Simulation.Piece.Create(other.Piece.Object),
                      new RankFile(other.RankAndFile)) 
            {
            }
            
            public override Chess.Game.Square Clone()
            {
                return new Square(this);
            }
        }
    }
    
    namespace Graphical 
    {
        public class Square : Chess.Game.Square, ChessDrawable 
        {
            public static readonly Dictionary<Color, String> DefaultSpriteImageFiles = new Dictionary<Color, String> 
            {
                {black, "./Assets/Bitmaps/BlackSquare.png"},
                {white, "./Assets/Bitmaps/WhiteSquare.png"}
            };
            
            public override Optional<Chess.Game.IPiece> Piece 
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
            
            public Graphical.Piece Piece2D 
            {
                get
                {
                    if (base.Piece.HasValue)
                    {
                        return (Graphical.Piece) Piece.Object;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public Sprite Sprite { get; set; }

            public Size Size 
            {
                get { return Sprite.GetActualSize(); }
            }

            public Vec2<uint> OriginCoordinates 
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
            
            public Vec2<uint> CenterCoordinates 
            {
                get
                {
                    if (Sprite != null)
                    {
                        uint x = (uint)(Sprite.Position.X + (Sprite.GetActualSize().Width / 2f));
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
            
            public Square(char file, ushort rank) : 
                base(file, rank)
            {
                
            }

            public Square(RankFile rankAndFile) : 
                base(rankAndFile)
            {
                
            }

            public Square(Chess.Game.IPiece piece, char file, ushort rank) : 
                base(piece, file, rank)
            {
                
            }

            public Square(Chess.Game.IPiece piece, RankFile rankAndFile) : 
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

            public Square(Chess.Game.Square other) : 
                this (piece: (other.IsEmpty) ? null : Graphical.Piece.Create(other.Piece.Object),
                      new RankFile(other.RankAndFile)) 
            {
                
            }
            
            public override Chess.Game.Square Clone()
            {
                return new Square(this);
            }
            
            public void InitializeGraphicalElements()
            {
                var spriteTexture = new Texture(DefaultSpriteImageFiles[Color]);
                Sprite       = new Sprite(spriteTexture);
                Sprite.Scale = calculateScalingFromBoardResolution();
                Piece2D?.InitializeGraphicalElements();
            }

            public void Initialize2DCoordinates(Vec2<uint> coordinates)
            {
                this.OriginCoordinates = coordinates;
                Piece2D?.Initialize2DCoordinates();
            }

            public void Draw(RenderTarget renderer)
            {
                renderer.Draw(Sprite);
                Piece2D?.Draw(renderer);
            }
            
            private Vec2<double> calculateScalingFromBoardResolution()
            {
                Size unscaledSizeOfObject = Sprite.Texture.Size;
                
                ushort numberOfSquaresHorizontal = (ushort) Graphical.Board.DefaultStartingSquares().GetLength(0);
                ushort numberOfSquaresVertical   = (ushort) Graphical.Board.DefaultStartingSquares().GetLength(1);
            
                uint targetWidthForSquare  = Chess.Util.Config.BoardResolution.Width  / numberOfSquaresHorizontal;
                uint targetHeightForSquare = Chess.Util.Config.BoardResolution.Height / numberOfSquaresVertical;

                Size targetSizeForSquare = new Size {Width = targetWidthForSquare, Height = targetHeightForSquare};

                Vec2<double> scaleFactor = targetSizeForSquare / unscaledSizeOfObject;

                return scaleFactor;
            }
        }
    }

}