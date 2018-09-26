using System;

namespace Chess.Game
{
    public class Human : Player
    {
        public Human(Color color, Board board) : 
            base(color, board)
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

        public override void onTurn()
        {
            throw new NotImplementedException();
        }

        public override Move DecideNextMove()
        {
            //todo: implement
            throw new NotImplementedException();
        }
    }
}