using System;
using Chess.Util;
using Chess.View;
using SFML.Graphics;

namespace Chess.Game
{
    public class Square : ICloneable, ChessDrawable
    {
        public Vec2<uint> Position { get; }
        
        public RankAndFile BoardPosition
        {
            get { return Position; }
        }

        private Color? color = null;

        public Color Color
        {
            get
            {
                if (color.HasValue == false)
                {
                    color = determineColor();
                }

                return color.Value;
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
        
        
        public Square(Vec2<uint> position, Board board, Piece piece = null)
        {
            Position = position;
            Board = board;
            Piece = new Optional<Piece>(piece);
        }

        public Square(Square other):
            this(new Vec2<uint>(other.Position),
                 null, /* Don't copy other's board pointer */
                 (other.isEmpty) ? null : Game.Piece.create(other.piece))
        {
            
        }

        public Square(char file, ushort rank, Board board = null) :
            this(new RankAndFile(file, rank), board)
        {
            
        }

        public Square(Piece piece, char file, ushort rank, Board board = null) :
            this(file, rank, board)
        {
            this.Piece = piece;
        }

        public Square(char pieceSymbol, char file, ushort rank, Board board = null) :
            this(file, rank, board)
        {
            this.Piece = Game.Piece.create(pieceSymbol);
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
            
            uint coordinateSum = Position.X + Position.Y;
            
            if ((coordinateSum % 2) == 0) //is even
            {
                color = Color.white;
            }
            else
            {
                color = Color.black;
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