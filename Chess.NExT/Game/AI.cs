using System;
using System.Collections.Generic;
using System.Security.Cryptography;

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
            MoveAction move = decideNextMove();

            move.commit();
        }

        public override MoveAction decideNextMove()
        {
            return chooseBestMove();
        }

		protected MoveAction chooseBestMove() 
		{
			List<MoveAction> highValueMoveOptions = findBestMoves();

			highValueMoveOptions = highValueMoveOptions.extractHighestValueSubset();
	
			MoveAction move = highValueMoveOptions.selectElementAtRandom();

			return move;
		}
	
    }
}