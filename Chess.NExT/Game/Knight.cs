using System;
using System.Collections.Generic;
using static Chess.Game.Color;

namespace Chess.Game
{
    public class Knight : Piece
    {
        
        public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
        {
            {black, '♞'}, 
            {white, '♘'}
        };
        
        public override char ASCIISymbol
        {
            get { return 'N'; }
        }
        
        public override ushort Value
        {
            get { return 3; }
        }
        
        public override List<Direction> LegalMovementDirections
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
        
        public override ushort MaximumMoveDistance { get { return 1; } }

        public Knight(Knight other) :
            base(other)
        {
		    
        }
        
        public Knight(Color color) :
            base(DefaultSymbols[color], color)
        {
	
        }
	
        public Knight(char symbol) :
            this((symbol == DefaultSymbols[black]) ? black : white)
        {
            if (DefaultSymbols.ContainsValue(symbol) == false)
            {
                throw new ArgumentException($"{symbol} is not a valid chess piece");
            }
        }
        
        public override Piece Clone()
        {
            return new Knight(this);
        }
        
        public override void Move(RankFile destination) 
        {
            //todo add move legality checking
            base.Move(destination);
        }
        
    }

    namespace Graphical
    {
        public class Knight : Piece
        {
            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♞'},
                {white, '♘'}
            };

            public static readonly Dictionary<Color, String> DefaultImageFiles = new Dictionary<Color, String>
            {
                {black, "./Assets/Bitmaps/BlackKnight.png"},
                {white, "./Assets/Bitmaps/WhiteKnight.png"}
            };

            public override char ASCIISymbol { get { return 'N'; } }

            public override ushort Value { get { return 3; } }

            public override List<Direction> LegalMovementDirections
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

            public override ushort MaximumMoveDistance { get { return 1; } }

            public Knight(Knight other) :
                base(other)
            {

            }

            public Knight(Color color) :
                base(DefaultSymbols[color], color, DefaultImageFiles[color])
            {

            }

            public Knight(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }

            public override Chess.Game.Piece Clone()
            {
                return new Knight(this);
            }

            public override void Move(RankFile destination)
            {
                //todo add move legality checking
                base.Move(destination);
            }
        }
    }
}