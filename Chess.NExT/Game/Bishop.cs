using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public class Bishop : Piece
    {
        protected static readonly List<Direction> defaultLegalMovementDirections = new List<Direction> {upLeft, upRight, downLeft, downRight};
        
        public new static readonly Dictionary<Color, Char> defaultSymbols = new Dictionary<Color, Char>
        {
            {black, '♝'}, 
            {white, '♗'}
        };

        public new static readonly Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
        {
            {black, "./Assets/Bitmaps/BlackBishop.png"},
            {white, "./Assets/Bitmaps/WhiteBishop.png"}
        };

        public override char asciiSymbol
        {
            get { return 'B'; }
        }

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
	    
        public Bishop(Color color) :
            base(defaultSymbols[color], defaultImageFiles[color], color)
        {
	
        }
	
        public Bishop(char symbol) :
            this((symbol == defaultSymbols[black]) ? black : white)
        {
            if (defaultSymbols.ContainsValue(symbol) == false)
            {
                throw new ArgumentException($"{symbol} is not a valid chess piece");
            }
        }
        
        public override Piece Clone()
        {
            return new Bishop(this);
        }

        public override void move(RankAndFile destination) 
        {
            //todo add move legality checking
            base.move(destination);
            Console.WriteLine("Warning: add move legality checking");
        }

    }
}