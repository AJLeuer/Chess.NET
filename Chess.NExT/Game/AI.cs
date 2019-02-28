using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Utility;
using System.Threading.Tasks;
	
using static Chess.Utility.Util;

namespace Chess.Game
{
    namespace Real
    {
		public class AI : Player 
		{
	        public AI(Color color) :
	            base(color)
	        {
	            
	        }
	
	        public AI(Chess.Game.Player other) : 
	            base(other)
	        {
	            
	        }
	
	        public override Chess.Game.Player Clone()
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
		        TreeNode<Move> movePossibilityTree = simPlayer.buildMovePossibilityTreeInParallel(maximumDepth: 3);
		        Move bestMove = searchMovePossibilityTreeForBestMove(movePossibilityTree);
		        return bestMove;
	        }
	
			private void buildMovePossibilityTree(uint currentDepth, uint maximumDepth, TreeNode<Move> moveTree = null)
			{
				if (currentDepth < maximumDepth)
				{
					findPossibleMovesByDepth(currentDepth, ref moveTree);
					currentDepth++;
					
					foreach (var childMoveNode in moveTree.Children) 
					{
						buildMovePossibilityTree(currentDepth, maximumDepth, childMoveNode);	
					}
				}
			}
			
			private TreeNode<Move> buildMovePossibilityTreeInParallel(uint maximumDepth)
			{
				TreeNode<Move> moveTree = null;
				
				findPossibleMovesByDepth(currentDepth: 0, ref moveTree);

				var moveTreeBuilders = new List<Task>();
				
				foreach (var childMoveNode in moveTree.Children) 
				{
					Action moveTreeBuilder = () =>
					{
						this.buildMovePossibilityTree(currentDepth: 1, maximumDepth: maximumDepth, moveTree: childMoveNode);
					};
					
					moveTreeBuilders.Add(Task.Run(moveTreeBuilder));
				}

				Task.WaitAll(moveTreeBuilders.ToArray());

				return moveTree;
			}
			
			private void findPossibleMovesByDepth(uint currentDepth, ref TreeNode<Move> moveTree)
			{
				List<Move> possibleMoves;
				
				if (currentDepth == 0)
				{
					moveTree = new TreeNode<Move>(null);
					possibleMoves = FindPossibleMoves();
				}
				else /* if (currentDepth > 0) */
				{
					possibleMoves = findPossibleMovesInSimulation(moveTree);
				}

				moveTree.AddChildren(possibleMoves.ToArray());
			}

			private List<Move> findPossibleMovesInSimulation(TreeNode<Move> moveTree)
			{
				List<Move> possibleMoves;
				Move simulatedMove = moveTree.Datum;
				simulatedMove = simulatedMove.CommitInSimulation();

				AI simulatedAIPlayer = (AI) simulatedMove.Game.FindMatchingPlayer(this);

				var simulatedOpponent = (Simulation.SimpleAI) simulatedMove.Game.FindOpponentPlayer(simulatedAIPlayer);

				try
				{
					Move opponentMove = simulatedOpponent.DecideNextMove();
					opponentMove.Commit();
					possibleMoves = simulatedAIPlayer.FindPossibleMoves();
				}
				catch (NoRemainingMovesException)
				{
					possibleMoves = new List<Move>();
				}

				return possibleMoves;
			}

			private Move searchMovePossibilityTreeForBestMove(TreeNode<Move> moveTree)
			{
				TreeNode<Move> bestMoveSequence = searchMovePossibilityTreeForBestMoveSequenceInParallel(moveTree);
				Move startingMove = retrieveFirstMoveFromSequence(bestMoveSequence);
	
				return startingMove;
			}

			private static TreeNode<Move> searchMovePossibilityTreeForBestMoveSequenceInParallel(TreeNode<Move> moveTree)
			{
				var moveTreeSearchers = new List<Task<TreeNode<Move>>>();
				
				foreach (var node in moveTree.Children) 
				{
					Func<TreeNode<Move>> searchForBestMove = () =>
					{
						return searchMovePossibilityTreeForBestMoveSequence(node);
					};

					Task<TreeNode<Move>> moveTreeSearcher = Task.Run(searchForBestMove);
					
					moveTreeSearchers.Add(moveTreeSearcher);
				}

				Task<TreeNode<Move>[]> moveTreeSearchersTask = Task.WhenAll(moveTreeSearchers.ToArray());
				TreeNode<Move>[] bestMoveSequenceCandidates = moveTreeSearchersTask.Result; //blocks until all tasks are complete
				return bestMoveSequenceCandidates.ToList().RetrieveHighestValueItem();
			}
			
			private static TreeNode<Move> searchMovePossibilityTreeForBestMoveSequence(TreeNode<Move> moveTree)
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
	    }
	}

    namespace Simulation 
    {
	    /// <summary>
	    /// Can serve the same role of computer-controlled opponent as AI, but has none of the sophisticated game-tree
	    /// searching used in the AI class. Move decisions are made via a basic greedy algorithm: SimpleAI conducts a brute-force
	    /// search of possible moves to make in the next turn and chooses the most valuable. It does not look ahead. This
	    /// makes it useful for applications like testing, and serving as a straw-man opponent for an AI player (which
	    /// *does* look ahead) to test potential moves against.
	    /// </summary>
	    public class SimpleAI : Player
	    {
		    public SimpleAI(Color color) :
			    base(color)
		    {
            
		    }

		    public SimpleAI(Chess.Game.Player other) : 
			    base(other)
		    {
            
		    }

		    public override Chess.Game.Player Clone()
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
            
			    foreach (var piece in Pieces)
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
}