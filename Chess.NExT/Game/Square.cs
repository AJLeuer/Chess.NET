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

        public void InitializeSprite()
        {
            var spriteTexture = new Texture(Config.BoardSpriteFilePath);
            Sprite = new Sprite(spriteTexture);
            
            if (Piece.HasValue)
            {
                Piece.Object.InitializeSprite();
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
            piece?.onCapture();
            clearCurrentPiece();
        }

        protected void clearCurrentPiece()
        {
            Piece = Optional<Piece>.Empty;
        }

    }

}