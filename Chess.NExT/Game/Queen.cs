using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;


namespace Chess.Game
{
    public interface IQueen : IPiece {}

    namespace Simulation
    {
        public class Queen : Piece, IQueen 
        {
            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction>
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
        
            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♛'}, 
                {white, '♕'}
            };
        
            public override char ASCIISymbol
            {
                get { return 'Q'; }
            }
        
            public override ushort Value
            {
                get { return 9; }
            }
        
            public override List<Direction> LegalMovementDirections
            {
                get { return DefaultLegalMovementDirections; }
            }
        
            public Queen(IQueen other) :
                base(other)
            {
		    
            }
	
            public Queen(Color color) :
                base(DefaultSymbols[color], color)
            {
	
            }
	
            public Queen(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }
        
            public override IPiece Clone()
            {
                return new Queen(this);
            }

            public override void Move(RankFile destination) 
            {
                //todo add move legality checking
                base.Move(destination);
            }
        }
    }
    
    namespace Graphical
    {
        public class Queen : Piece, IQueen 
        {
            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction>
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
        
            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♛'}, 
                {white, '♕'}
            };
            
            public static readonly Dictionary<Color, String> DefaultSpriteImageFiles = new Dictionary<Color, String> 
            {
                {black, "./Assets/Bitmaps/BlackQueen.png"},
                {white, "./Assets/Bitmaps/WhiteQueen.png"}
            };
        
            public override char ASCIISymbol
            {
                get { return 'Q'; }
            }
        
            public override ushort Value
            {
                get { return 9; }
            }
        
            public override List<Direction> LegalMovementDirections
            {
                get { return DefaultLegalMovementDirections; }
            }
        
            public Queen(IQueen other) :
                base(other)
            {
                SpriteImageFilePath = DefaultSpriteImageFiles[this.Color];
            }
	
            public Queen(Color color) :
                base(DefaultSymbols[color], color, DefaultSpriteImageFiles[color])
            {
	
            }
	
            public Queen(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }
        
            public override IPiece Clone()
            {
                return new Queen(this);
            }

            public override void Move(RankFile destination) 
            {
                //todo add move legality checking
                base.Move(destination);
            }
        }
    }
}