using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;


namespace Chess.Game
{
    public class Rook : Piece
    {
        
        protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction> {up, down, left, right};
        
        public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
        {
            {black, '♜'}, 
            {white, '♖'}
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
            get { return DefaultLegalMovementDirections; }
        }
        
        public Rook(Rook other) :
            base(other)
        {
		    
        }
        
        public Rook(Color color) :
            base(DefaultSymbols[color], color)
        {
	
        }
	
        public Rook(char symbol) :
            this((symbol == DefaultSymbols[black]) ? black : white)
        {
            if (DefaultSymbols.ContainsValue(symbol) == false)
            {
                throw new ArgumentException($"{symbol} is not a valid chess piece");
            }
        }

        public override Piece Clone()
        {
            return new Rook(this);
        }

        public override void Move(RankFile destination) 
        {
            //todo add move legality checking
            base.Move(destination);
        }
    }

    namespace Graphical
    {
        public class Rook : Piece
        {
            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction> {up, down, left, right};
        
            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♜'}, 
                {white, '♖'}
            };

            public static readonly Dictionary<Color, String> DefaultImageFiles = new Dictionary<Color, String> 
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
                get { return DefaultLegalMovementDirections; }
            }
        
            public Rook(Rook other) :
                base(other)
            {
		    
            }
        
            public Rook(Color color) :
                base(DefaultSymbols[color], color, DefaultImageFiles[color])
            {
	
            }
	
            public Rook(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }

            public override Game.Piece Clone()
            {
                return new Rook(this);
            }

            public override void Move(RankFile destination) 
            {
                //todo add move legality checking
                base.Move(destination);
            }
        }
    }
}