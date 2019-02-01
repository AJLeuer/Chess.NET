using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chess.Utility;
using System.Linq;
using System.Threading;
using static Chess.Utility.Util;

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
	        Move bestMove = findBestMove();
			return bestMove;
        }

        private Move findBestMove()
        {
	        Simulation.Game simulatedGame = new Simulation.Game(this.Game);
	        AI simPlayer = (AI) simulatedGame.FindMatchingPlayer(this);
	        TreeNode<Move> movePossibilityTree = simPlayer.buildMovePossibilityTree(currentDepth: 0, maximumDepth: 3);
	        Move bestMove = searchMovePossibilityTreeForBestMove(movePossibilityTree);
	        return bestMove;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		private TreeNode<Move> buildMovePossibilityTree(uint currentDepth, uint maximumDepth, TreeNode<Move> moveTree = null)
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

					SimpleAI simulatedOpponent = (SimpleAI) simulatedMove.Game.FindOpponentPlayer(simulatedAIPlayer);

					try
					{
						Move opponentMove = simulatedOpponent.DecideNextMove();
						opponentMove.Commit();
						possibleMoves = simulatedAIPlayer.FindPossibleMoves();
					}
					catch (NoRemainingMovesException)
					{
						possibleMoves = new List<Move>{};
					}
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

		private Move searchMovePossibilityTreeForBestMove(TreeNode<Move> moveTree)
		{
			TreeNode<Move> bestMoveSequence = searchMovePossibilityTreeForBestMoveSequence(moveTree);
			Move startingMove = retrieveFirstMoveFromSequence(bestMoveSequence);

			return startingMove;
		}
		
		private TreeNode<Move> searchMovePossibilityTreeForBestMoveSequence(TreeNode<Move> moveTree)
		{
			TreeNode<Move> overallHighestValueMove = moveTree; //if this node has no children, then it itself is considered the highest-value move

			for (int i = 0; i < moveTree.Children.Count; i++)
			{
				TreeNode<Move> childMoveNode = moveTree.Children[i];
			
				TreeNode<Move> currentMove = searchMovePossibilityTreeForBestMoveSequence(childMoveNode);

				if (currentMove != null)
				{
					if (i == 0)
					{
						overallHighestValueMove = currentMove;
					}
					else if (currentMove == overallHighestValueMove)
					{
						overallHighestValueMove = SelectAtRandom(currentMove, overallHighestValueMove);
					}
					else if (currentMove > overallHighestValueMove)
					{
						overallHighestValueMove = currentMove;
					}	
				}
			}

			return overallHighestValueMove;
		}
		
		private Move retrieveFirstMoveFromSequence(TreeNode<Move> sequence)
		{
			Move simulatedStartingMove;

			do
			{
				simulatedStartingMove = sequence.Datum;
				sequence = sequence.Parent;
			} while (sequence.Parent != null);


			Move startingMove = Move.CreateMatchingMoveForGame(simulatedStartingMove, this.Game);
			return startingMove;
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

	/// <summary>
	/// Can serve the same role of computer-controlled opponent as AI, but has none of the sophisticated game-tree
	/// searching used in the AI class. Move decisions are made via a basic greedy algorithm: SimpleAI conducts a brute-force
	/// search of possible moves to make in the next turn and chooses the most valuable. It does not look ahead. This
	/// makes it useful for applications like testing, and serving as a straw-man opponent for an AI player (which
	/// *does* look ahead) to test potential moves against.
	/// </summary>
	public class SimpleAI : AI
	{
		public SimpleAI(Color color) :
			base(color)
		{
            
		}

		public SimpleAI(Player other) : 
			base(other)
		{
            
		}

		public override Player Clone()
		{
			return new SimpleAI(this);
		}
		
		protected override Move decideNextMove()
		{
			return chooseBestMoveGreedily();
		}
		
		protected Move chooseBestMoveGreedily() 
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

			if (moves.Count == 0)
			{
				throw new NoRemainingMovesException();
			}
			
			return moves;
		}
	}
}