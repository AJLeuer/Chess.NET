﻿using System.Collections.Generic;
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

        public override Move ComputeNextMove()
        {
            return chooseBestMove();
        }

		protected Move chooseBestMove() 
		{
			List<Move> highValueMoveOptions = FindBestMoves();

			highValueMoveOptions = highValueMoveOptions.ExtractHighestValueSubset();
	
			Move move = highValueMoveOptions.SelectElementAtRandom();

			return move;
		}
	
    }
}