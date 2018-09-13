using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;


namespace Chess.Game
{
    public class Rook : Piece
    {
        
        protected static readonly List<Direction> defaultLegalMovementDirections = new List<Direction> {up, down, left, right};
        
        public new static readonly Dictionary<Color, Char> defaultSymbols = new Dictionary<Color, Char>
        {
            {black, '♜'}, 
            {white, '♖'}
        };

        public new static readonly Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
        {
            {black, "./Assets/Bitmaps/BlackRook.png"},
            {white, "./Assets/Bitmaps/WhiteRook.png"}
        };
        
        public override char ASCIISymbol
        {
            get { return 'R'; }
        }
        
        public override ushort Value
        {
            get { return 5; }
        }
        
        public override List<Direction> LegalMovementDirections
        {
            get { return defaultLegalMovementDirections; }
        }
        
        public Rook(Rook other) :
            base(other)
        {
		    
        }
        
        public Rook(Color color) :
            base(defaultSymbols[color], defaultImageFiles[color], color)
        {
	
        }
	
        public Rook(char symbol) :
            this((symbol == defaultSymbols[black]) ? black : white)
        {
            if (defaultSymbols.ContainsValue(symbol) == false)
            {
                throw new ArgumentException($"{symbol} is not a valid chess piece");
            }
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