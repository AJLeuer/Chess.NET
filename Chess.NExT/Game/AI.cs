using System.Collections.Generic;
using Chess.Util;

namespace Chess.Game
{
    public class AI : Player
    {
        public AI(Color color, Board board) :
            base(color, board)
        {
            
        }

        public AI(Player other) : 
            base(other)
        {
            
        }

        public override Player Clone()
        {
            return new AI(this);
        }
        
        public override void onTurn()
        {
            Move move = decideNextMove();

            move.Commit();
        }

        public override Move decideNextMove()
        {
            return chooseBestMove();
        }

		protected Move chooseBestMove() 
		{
			List<Move> highValueMoveOptions = findBestMoves();

			highValueMoveOptions = highValueMoveOptions.extractHighestValueSubset();
	
			Move move = highValueMoveOptions.selectElementAtRandom();

			return move;
		}
	
    }
}