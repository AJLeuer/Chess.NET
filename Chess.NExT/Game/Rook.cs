using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;


namespace Chess.Game
{
    public class Rook : Piece
    {
        
        protected static readonly List<Direction> defaultLegalMovementDirections = new List<Direction> {up, down, left, right};
        
        public static readonly new Dictionary<Color, char> defaultSymbols = new Dictionary<Color, char>
        {
            {black, '♜'}, 
            {white, '♖'}
        };

        public static readonly new Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
        {
            {black, "./Assets/Bitmaps/BlackRook.png"},
            {white, "./Assets/Bitmaps/WhiteRook.png"}
        };
        
        public override ushort value
        {
            get { return 5; }
        }
        
        public override List<Direction> legalMovementDirections
        {
            get { return defaultLegalMovementDirections; }
        }
        
        public Rook(Rook other) :
            base(other)
        {
		    
        }
	    
        public Rook(Color color, Square square) :
            base((color == Color.black) ? defaultSymbols[black] : defaultSymbols[white],
                 (color == Color.black) ? defaultImageFiles[black] : defaultImageFiles[white], color, square)
        {
	
        }
	
        public Rook(char symbol, Square square) :
            base((symbol == defaultSymbols[black]) ? defaultSymbols[black] : defaultSymbols[white],
                 (symbol == defaultSymbols[black]) ? defaultImageFiles[black] : defaultImageFiles[white],
                 (symbol == defaultSymbols[black]) ? black : white, square) 
        {
	
        }

        public override Piece Clone()
        {
            return new Rook(this);
        }

        public override void move(RankAndFile destination) 
        {
            //todo add move legality checking
            base.move(destination);
        }
    }
}