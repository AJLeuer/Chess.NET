using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chess.Util;
using System.Linq;
using System.Net;

namespace Chess.Game
{
    public class AI : Player
    {
        public AI(Color color) :
            base(color)
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
			Simulation.Game simulatedGame = new Simulation.Game(this.Board.Game);
			AI simPlayer = (AI) simulatedGame.FindMatchingPlayer(this);
			TreeNode<Move> movePossibilityTree = simPlayer.buildMovePossibilityTree(4);
            return chooseBestMove();
        }

		TreeNode<Move> buildMovePossibilityTree(uint toDepth)
		{
			return buildMovePossibilityTree(currentDepth: 0, maximumDepth: toDepth);
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		TreeNode<Move> buildMovePossibilityTree(uint currentDepth, uint maximumDepth, TreeNode<Move> moveTree = null)
		{
			if (currentDepth < maximumDepth)
			{
				List<Move> possibleMoves = null;
				
				if (currentDepth == 0)
				{
					moveTree = new TreeNode<Move>(null);
					possibleMoves = FindPossibleMoves();
				}
				else if (currentDepth > 0)
				{
					Move simulatedMove = moveTree.Datum;
					simulatedMove = simulatedMove.CommitInSimulation();
					
					AI simulatedAIPlayer = (AI) simulatedMove.Game.FindMatchingPlayer(this);

					MockAIPlayer simulatedOpponent = (MockAIPlayer) simulatedMove.Game.FindOpponentPlayer(simulatedAIPlayer);
					Move opponentMove = simulatedOpponent.DecideNextMove();
					opponentMove.Commit();

					moveTree.Datum = simulatedMove;
					possibleMoves = simulatedAIPlayer.FindPossibleMoves();
				}
				
				moveTree.AddChildren(possibleMoves.ToArray());
				currentDepth++;
				
				foreach (var childMoveNode in moveTree.Children)
				{
					buildMovePossibilityTree(currentDepth, maximumDepth, childMoveNode);
				}
			}
			
			return moveTree;
		}

		protected virtual Move chooseBestMove() 
		{
			List<Move> highValueMoveOptions = findBestMoves();

			highValueMoveOptions = highValueMoveOptions.ExtractHighestValueSubset();
	
			Move move = highValueMoveOptions.SelectElementAtRandom();

			return move;
		}
		
		protected List<Move> findBestMoves()
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
		
		protected Optional<Move> findBestMoveForPiece(IPiece piece)
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

	public class MockAIPlayer : AI
	{
		public MockAIPlayer(Color color) :
			base(color)
		{
            
		}

		public MockAIPlayer(Player other) : 
			base(other)
		{
            
		}

		public override Player Clone()
		{
			return new MockAIPlayer(this);
		}
		
		protected override Move decideNextMove()
		{
			return chooseBestMove();
		}
		
		protected override Move chooseBestMove() 
		{
			List<Move> highValueMoveOptions = findBestMoves();

			highValueMoveOptions = highValueMoveOptions.ExtractHighestValueSubset();
	
			Move move = highValueMoveOptions.SelectElementAtRandom();

			return move;
		}
	}
    
}