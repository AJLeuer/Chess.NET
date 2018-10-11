using System;
using Chess.Util;
using Chess.View;
using SFML.Graphics;

namespace Chess.Game
{
    public class Square : ICloneable, ChessDrawable
    {
        public Vec2<uint> BoardPosition { get; set; }
        
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

        public Sprite Sprite { get; set; }

        public Size Size
        {
            get { return Sprite.Texture.Size; }
        }

        public Vec2<uint> Position2D
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

        public Board Board { get; set; }

        private Piece piece = null;
        
        public Optional<Piece> Piece 
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

        public Square(Piece piece = null)
        {
            Piece = new Optional<Piece>(piece);
        }

        public Square(char pieceSymbol) :
            this(Game.Piece.create(pieceSymbol))
        {
            
        }
        
        public Square(Square other):
            this ((other.isEmpty) ? null : Game.Piece.create(other.piece)) /* Don't copy other's board pointer */
        {
            BoardPosition = new Vec2<uint>(other.BoardPosition);
        }

        ~Square()
        {
            clearCurrentPiece();
        }

        public object Clone()
        {
            return new Square(this);
        }

        public void InitializeGraphicalElements()
        {
            var spriteTexture = new Texture(Config.BoardSpriteFilePath);
            Sprite = new Sprite(spriteTexture);
            Sprite.Scale = calculateScalingFromBoardResolution(Size);
            
            if (Piece.HasValue)
            {
                Piece.Object.InitializeGraphicalElements();
            }
        }

        public void Initialize2DPosition(Vec2<uint> position)
        {
            this.Position2D = position;

            if (Piece.HasValue)
            {
                Piece.Object.Initialize2DPosition();
            }
        }

        protected Color determineColor()
        {
            Color color;
            
            uint coordinateSum = BoardPosition.X + BoardPosition.Y;
            
            if ((coordinateSum % 2) == 0) //is even
            {
                color = Color.black;
            }
            else
            {
                color = Color.white;
            }

            return color;
        }

        public void handleLeavingPiece()
        {
            clearCurrentPiece();
        }

        public void receiveArrivingPiece(Piece arrivingPiece)
        {
            if (this.isOccupied)
            {
                captureCurrentPiece();
            }

            this.Piece = arrivingPiece;
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

}