using System;

namespace Chess.Game
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

        public override Player Clone()
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