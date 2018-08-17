using System;
using Chess.Util;

namespace Chess.Game
{
    public class Square : ICloneable
    {
        public Vec2<uint> position { get; }

        public Board board { get; set; }

        private Piece currentPiece = null;

        public bool isEmpty
        {
            get { return (isOccupied == false); }
        }

        public bool isOccupied
        {
            get { return piece.HasValue; }
        }

        public RankAndFile rankAndFile
        {
            get { return position; }
        }

        public Optional<Piece> piece
        {
            get { return currentPiece; }
            set
            {
                if (value.HasValue == false)
                {
                    if (currentPiece != null)
                    {
                        currentPiece.square = null;
                    }
                    this.currentPiece = null;
                }
                else
                {
                    this.currentPiece = value.Value;
                    currentPiece.square = this;
                }
            }
        }

        public Square(Square other)
        {
            position = new Vec2<uint>(other.position);
            /* Don't copy other's board pointer */
            piece = (other.isEmpty) ? null : Piece.createByCopy(other.currentPiece);
        }

        public Square(char file, ushort rank, Board board)
        {
            position = new RankAndFile(file, rank);
            this.board = board;
            currentPiece = null;
        }

        public Square(Piece piece, char file, ushort rank, Board board) :
            this(file, rank, board)
        {
            this.piece = piece;
        }

        public Square(char pieceSymbol, char file, ushort rank, Board board) :
            this(file, rank, board)
        {
            this.piece = Piece.create(pieceSymbol, this);
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

            this.piece = piece;
        }

        protected void captureCurrentPiece()
        {
            currentPiece?.onCapture();
            clearCurrentPiece();
        }

        protected void clearCurrentPiece()
        {
            piece = null;
        }

    }

}