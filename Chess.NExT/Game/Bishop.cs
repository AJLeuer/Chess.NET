using System;
using System.Collections.Generic;
using Chess.Util;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public class Bishop : Piece
    {
        protected static readonly List<Direction> defaultLegalMovementDirections = new List<Direction> {upLeft, upRight, downLeft, downRight};
        
        public static readonly new Dictionary<Color, char> defaultSymbols = new Dictionary<Color, char>
        {
            {black, '♝'}, 
            {white, '♗'}
        };

        public static readonly new Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
        {
            {black, "./Assets/Bitmaps/BlackBishop.png"},
            {white, "./Assets/Bitmaps/WhiteBishop.png"}
        };
        
        public override ushort value
        {
            get { return 3; }
        }
        
        public override List<Direction> legalMovementDirections
        {
            get { return defaultLegalMovementDirections; }
        }

        public Bishop(Bishop other) :
            base(other)
        {
		    
        }
	    
        public Bishop(Color color, Square square) :
            base((color == Color.black) ? defaultSymbols[black] : defaultSymbols[white],
                 (color == Color.black) ? defaultImageFiles[black] : defaultImageFiles[white], color, square)
        {
	
        }
	
        public Bishop(char symbol, Square square) :
            base((symbol == defaultSymbols[black]) ? defaultSymbols[black] : defaultSymbols[white],
                 (symbol == defaultSymbols[black]) ? defaultImageFiles[black] : defaultImageFiles[white],
                 (symbol == defaultSymbols[black]) ? black : white, square) 
        {
	
        }
        
        public override Piece Clone()
        {
            return new Bishop(this);
        }

        public override void move(RankAndFile destination) 
        {
            //todo add move legality checking
            base.move(destination);
        }

    }
}