using System.Collections.Generic;
using Chess.Game;
using Chess.Game.Simulation;
using NUnit.Framework;
using static Chess.Test.Util.AdditionalCollectionAssertions;

using Board = Chess.Game.Simulation.Board;
using File = System.Char;
using Rank = System.UInt16;
using Piece = Chess.Game.Simulation.Piece;
using King = Chess.Game.Simulation.King;
using Square = Chess.Game.Simulation.Square;

namespace Chess.Test.Tests
{
    public static class KingTest
    {
        [Test]
        public static void ShouldFindAllValidMoveDestinations()
        {
            //Unlike all other pieces, King requires an existing game in order to compute its available moves,
            //since it will potentially need to eliminate some moves as possibilities based on the state of the game
            //and the positions of the opponent's pieces
            Piece king = new King(Color.white);
            var board = new Board(squares: Board.DefaultEmptySquares());
            board['e', 4].Piece = king;
            var whitePlayer = new SimpleAI(Color.white);
            var blackPlayer = new SimpleAI(Color.black);
            var unused = new Game.Simulation.Game(board, whitePlayer, blackPlayer);

            List<Chess.Game.Square> possibleMoves = king.FindAllPossibleLegalMoveDestinations();
            
            AssertContains(actual: possibleMoves,  board['d', 5],
                                                                    board['e', 5],
                                                                    board['f', 5],
                                                                    board['f', 4],
                                                                    board['f', 3],
                                                                    board['e', 3],
                                                                    board['d', 3],
                                                                    board['d', 4]);
        }

        [Test]
        public static void ShouldFindAllValidMoveDestinationsWithOccupiedSquaresNearby()
        {
            
        }
        
    }
}