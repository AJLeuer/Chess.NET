using System;
using Chess.Input;

namespace Chess.Game
{
    namespace Real
    {
        public class Human : Player
        {
            public InputController InputController { get; set; }
            
            public Human(Color color, InputController inputController) : 
                base(color)
            {
                this.InputController = inputController;
            }

            public Human(Player other) : 
                base(other)
            {
                if (other is Human otherHuman)
                {
                    this.InputController = otherHuman.InputController;
                }
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