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
            throw new System.NotImplementedException();
        }

        public override MoveAction decideNextMove()
        {
            //todo: implement
            throw new System.NotImplementedException();
        }
    }
}