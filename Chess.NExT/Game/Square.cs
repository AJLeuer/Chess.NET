using System;
using Chess.Util;

namespace Chess.Game
{
    public class Square : ICloneable
    {
        public Vec2<uint> position { get; }

        public Board board { get; set; }

        private Piece piece = null;

        public bool isEmpty
        {
            get { return (isOccupied == false); }
        }

        public bool isOccupied
        {
            get { return Piece.HasValue; }
        }

        public RankAndFile rankAndFile
        {
            get { return position; }
        }

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
                    this.piece = value.Value;
                    piece.Square = this;
                }
            }
        }

        public Square(Square other)
        {
            position = new Vec2<uint>(other.position);
            /* Don't copy other's board pointer */
            Piece = (other.isEmpty) ? null : Game.Piece.create(other.piece);
        }

        public Square(char file, ushort rank, Board board = null)
        {
            position = new RankAndFile(file, rank);
            this.board = board;
            piece = null;
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

        public void handleLeavingPiece()
        {
            clearCurrentPiece();
        }

        public void receiveArrivingPiece(Piece piece)
        {
            if (this.isOccupied)
            {
                captureCurrentPiece();
            }

            this.Piece = piece;
        }

        protected void captureCurrentPiece()
        {
            piece?.onCapture();
            clearCurrentPiece();
        }

        protected void clearCurrentPiece()
        {
            Piece = Optional<Piece>.CreateEmpty();
        }

    }

}