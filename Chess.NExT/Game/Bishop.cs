using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public class Bishop : Piece
    {
        protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction> {upLeft, upRight, downLeft, downRight};
        
        public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
        {
            {black, '♝'}, 
            {white, '♗'}
        };

        public override char ASCIISymbol
        {
            get { return 'B'; }
        }

        public override ushort Value
        {
            get { return 3; }
        }
        
        public override List<Direction> LegalMovementDirections
        {
            get { return DefaultLegalMovementDirections; }
        }

        public Bishop(Bishop other) :
            base(other)
        {
		    
        }
	    
        public Bishop(Color color) :
            base(DefaultSymbols[color], color)
        {
	
        }
	
        public Bishop(char symbol) :
            this((symbol == DefaultSymbols[black]) ? black : white)
        {
            if (DefaultSymbols.ContainsValue(symbol) == false)
            {
                throw new ArgumentException($"{symbol} is not a valid chess piece");
            }
        }
        
        public override Piece Clone()
        {
            return new Bishop(this);
        }

        public override void Move(RankFile destination) 
        {
            //todo add move legality checking
            base.Move(destination);
            Console.WriteLine("Warning: add move legality checking");
        }

    }

    namespace Graphical
    {
        public class Bishop : Piece
        {
            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction> {upLeft, upRight, downLeft, downRight};
        
            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♝'}, 
                {white, '♗'}
            };

            public static readonly Dictionary<Color, String> DefaultImageFiles = new Dictionary<Color, String> 
            {
                {black, "./Assets/Bitmaps/BlackBishop.png"},
                {white, "./Assets/Bitmaps/WhiteBishop.png"}
            };

            public override char ASCIISymbol
            {
                get { return 'B'; }
            }

            public override ushort Value
            {
                get { return 3; }
            }
        
            public override List<Direction> LegalMovementDirections
            {
                get { return DefaultLegalMovementDirections; }
            }

            public Bishop(Bishop other) :
                base(other)
            {
		    
            }
	    
            public Bishop(Color color) :
                base(DefaultSymbols[color], color, DefaultImageFiles[color])
            {
	
            }
	
            public Bishop(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }
        
            public override Game.Piece Clone()
            {
                return new Bishop(this);
            }

            public override void Move(RankFile destination) 
            {
                //todo add move legality checking
                base.Move(destination);
                Console.WriteLine("Warning: add move legality checking");
            }
        }
    }
}