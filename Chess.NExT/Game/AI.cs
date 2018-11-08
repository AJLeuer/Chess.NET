using System.Collections.Generic;
using Chess.Util;
using System.Linq;

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

        protected override Move decideNextMove()
        {
            return chooseBestMove();
        }

		protected Move chooseBestMove() 
		{
			List<Move> highValueMoveOptions = findBestMoves();

			highValueMoveOptions = highValueMoveOptions.ExtractHighestValueSubset();
	
			Move move = highValueMoveOptions.SelectElementAtRandom();

			return move;
		}
		
		protected virtual List<Move> findBestMoves()
		{
			var moves = new List<Move>();
            
			foreach (var piece in pieces)
			{
				Optional<Move> bestMoveForPiece = findBestMoveForPiece(piece);

				if (bestMoveForPiece.HasValue)
				{
					moves.Add(bestMoveForPiece.Object);
				}
			}

			return moves;
		}
		
		protected virtual Optional<Move> findBestMoveForPiece(IPiece piece)
		{
			List<Move> moves = FindAllPossibleMovesForPiece(piece);

			moves = moves.ExtractHighestValueSubset();

			if (moves.Any())
			{
				return moves.SelectElementAtRandom();
			}
			else
			{
				return new Optional<Move>(null);
			}
		}
	
    }
    
}