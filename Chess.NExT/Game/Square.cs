using System;
using System.Collections.Generic;
using Chess.Util;
using Chess.View;
using SFML.Graphics;

using File = System.Char;
using Rank = System.UInt16;
using Position = Chess.Util.Vec2<uint>;
using static Chess.Game.Color;

namespace Chess.Game
{
    public abstract class Square : ICloneable 
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

        public void receiveArrivingPiece(IPiece arrivingPiece)
        {
            if (this.isOccupied)
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
                this (piece: (other.isEmpty) ? null : Simulation.Piece.Create(other.Piece.Object),
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
                this (piece: (other.isEmpty) ? null : Graphical.Piece.Create(other.Piece.Object),
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
                Sprite.Scale = CalculateScalingFromBoardResolution(Size);
                Piece2D?.InitializeGraphicalElements();
            }

            public void Initialize2DCoordinates(Vec2<uint> coordinates)
            {
                this.Coordinates2D = coordinates;
                Piece2D?.Initialize2DCoordinates();
            }

            public void Draw(RenderTarget renderer)
            {
                renderer.Draw(Sprite);
                Piece2D?.Draw(renderer);
            }
            
            public static Vec2<double> CalculateScalingFromBoardResolution(Size unscaledSizeOfObject)
            {
                ushort numberOfSquaresHorizontal = (ushort) Graphical.Board.DefaultStartingSquares.GetLength(0);
                ushort numberOfSquaresVertical   = (ushort) Graphical.Board.DefaultStartingSquares.GetLength(1);
            
                uint targetWidthForSquare  = Chess.Util.Config.BoardResolution.Width  / numberOfSquaresHorizontal;
                uint targetHeightForSquare = Chess.Util.Config.BoardResolution.Height / numberOfSquaresVertical;

                Size targetSizeForSquare = new Size {Width = targetWidthForSquare, Height = targetHeightForSquare};

                Vec2<double> scaleFactor = targetSizeForSquare / unscaledSizeOfObject;

                return scaleFactor;
            }
        }
    }

}