using System;

namespace Chess.Game
{
    namespace Real
    {
        public class Human : Player
        {
            public Human(Color color) : 
                base(color)
            {
            }

            public Human(Player other) : 
                base(other)
            {
            }

            public override Chess.Game.Player Clone()
            {
                return new Human(this);
            }

            protected override Move decideNextMove()
            {
                //todo: implement
                throw new NotImplementedException();
            }
        }
    }
}