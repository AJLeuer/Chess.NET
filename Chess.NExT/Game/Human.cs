using System;
using Chess.Input;

namespace Chess.Game
{
    namespace Real
    {
        public class Human : Player
        {
            private InputController inputController;

            public InputController InputController
            {
                private get { return inputController; }
                set
                {
                    inputController = value;
                    InputController.Player = this;
                }
            }

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
                return InputController.NextMove;
            }
        }
    }
}