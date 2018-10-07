using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public class King : Piece
    {
        
        protected static readonly List<Direction> defaultLegalMovementDirections =  new List<Direction>
        {
            up,
            down,
            left,
            right,
            upLeft,
            upRight,
            downLeft,
            downRight
        };

        public new static readonly Dictionary<Color, Char> defaultSymbols = new Dictionary<Color, Char>
        {
            {black, '♚'}, 
            {white, '♔'}
        };

        public new static readonly Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
        {
            {black, "./Assets/Bitmaps/BlackKing.png"},
            {white, "./Assets/Bitmaps/WhiteKing.png"}
        };
        
        public override char ASCIISymbol
        {
            get { return 'K'; }
        }
        
        public override ushort Value
        {
            get { return 40; }
        }
        
        public override List<Direction> LegalMovementDirections
        {
            get { return defaultLegalMovementDirections; }
        }
        
        public King(King other) :
            base(other)
        {
		    
        }
	    
        public King(Color color) :
            base(defaultSymbols[color], defaultImageFiles[color], color)
        {
	
        }
	
        public King(char symbol) :
            this((symbol == defaultSymbols[black]) ? black : white)
        {
            if (defaultSymbols.ContainsValue(symbol) == false)
            {
                throw new ArgumentException($"{symbol} is not a valid chess piece");
            }
        }
        
        public override Piece Clone()
        {
            return new King(this);
        }
	
        public override void Move(RankFile destination)  
        {
            //todo add move legality checking
            base.Move(destination);
        }
    }
}