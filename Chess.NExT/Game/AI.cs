﻿using System;
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
			TreeNode<Move> movePossibilityTree = simPlayer.buildMovePossibilityTree(toDepth: 3);
            return searchMovePossibilityTreeForBestMove(movePossibilityTree, toDepth: 3);
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

		private Move searchMovePossibilityTreeForBestMove(TreeNode<Move> moveTree, uint toDepth)
		{
			TreeNode<Move> bestMoveSequence = searchMovePossibilityTreeForBestMove(moveTree, currentDepth: 0, maximumDepth: toDepth);

			Move startingMove;
			
			do
			{
				startingMove = bestMoveSequence.Datum;
				bestMoveSequence = bestMoveSequence.Parent;
			}
			while (bestMoveSequence.Parent != null);

			return startingMove;
		}
		
		private TreeNode<Move> searchMovePossibilityTreeForBestMove(TreeNode<Move> moveTree, uint currentDepth, uint maximumDepth)
		{
			if (currentDepth == (maximumDepth - 1))
			{
				TreeNode<Move> localHighestValue = moveTree.Children.RetrieveHighestValueItem();
				return localHighestValue;
			}
			else
			{
				TreeNode<Move> overallHighestValueMove = null;

				for (int i = 0; i < moveTree.Children.Count; i++)
				{
					TreeNode<Move> childMoveNode = moveTree.Children[i];
				
					TreeNode<Move> currentMove = searchMovePossibilityTreeForBestMove(childMoveNode, (currentDepth + 1), maximumDepth);
				
					if (i == 0)
					{
						overallHighestValueMove = currentMove;
					}
					else if (currentMove > overallHighestValueMove)
					{
						overallHighestValueMove = currentMove;
					}
				}

				return overallHighestValueMove;
			}
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

			return moves;
		}
	}
    
}