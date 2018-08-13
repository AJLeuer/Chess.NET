using System;
using System.Collections.Generic;
using static Chess.Game.Color;

namespace Chess.Game
{
    public class Knight : Piece
    {
        
        public static readonly new Dictionary<Color, char> defaultSymbols = new Dictionary<Color, char>
        {
            {black, '♞'}, 
            {white, '♘'}
        };

        public static readonly new Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
        {
            {black, "./Assets/Bitmaps/BlackKnight.png"},
            {white, "./Assets/Bitmaps/WhiteKnight.png"}
        };
        
        public override ushort value
        {
            get { return 3; }
        }
        
        public override List<Direction> legalMovementDirections
        {
            get
            {
                List<Direction> directions = new List<Direction>();

                for (short h = -1; h <= 1; h += 2)
                {
                    for (short v = -2; v <= 2; v += 4)
                    {
                        directions.Add(new Direction(h, v));
                    }
                }

                for (short h = -2; h <= 2; h += 4)
                {
                    for (short v = -1; v <= 1; v += 2)
                    {
                        directions.Add(new Direction(h, v));
                    }
                }

                return directions;
            }
        }

        public Knight(Knight other) :
            base(other)
        {
		    
        }

        public Knight(Color color, Square square) :
            base((color == Color.black) ? defaultSymbols[black] : defaultSymbols[white],
                 (color == Color.black) ? defaultImageFiles[black] : defaultImageFiles[white], color, square)
        {
	
        }
	
        public Knight(char symbol, Square square) :
            base((symbol == defaultSymbols[black]) ? defaultSymbols[black] : defaultSymbols[white],
                 (symbol == defaultSymbols[black]) ? defaultImageFiles[black] : defaultImageFiles[white],
                 (symbol == defaultSymbols[black]) ? black : white, square) 
        {
	
        }
        
        public override Piece Clone()
        {
            return new Knight(this);
        }
	
        public override List<Square> findAllPossibleLegalMoveDestinations()
        {
            var legalMoves = board.getSpecifiedSquares(1, true, color.getOpposite(), position,
                legalMovementDirections.ToArray());
	
            return legalMoves;
        }
        
        public override void move(RankAndFile destination) 
        {
            //todo add move legality checking
            base.move(destination);
        }
        
    }
}