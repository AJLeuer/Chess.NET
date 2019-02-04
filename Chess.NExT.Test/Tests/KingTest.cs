using System.Collections.Generic;
using NUnit.Framework;
using Chess.Game;
using static Chess.Test.Util.AdditionalCollectionAssertions;

using File = System.Char;
using Rank = System.UInt16;
using Piece = Chess.Game.Simulation.Piece;
using King = Chess.Game.Simulation.King;

namespace Chess.Test.Tests
{
    public static class KingTest
    {
        [Test]
        public static void ShouldFindAllValidMoveDestinations()
        {
            Board board = new Game.Simulation.Board(squares: Game.Simulation.Board.DefaultEmptySquares());

            Piece king = new King(Color.white);

            board['e', 4].Piece = king;

            List<Square> possibleMoves = king.FindAllPossibleLegalMoveDestinations();
        }

        [Test]
        public static void ShouldFindAllValidMoveDestinationsWithOccupiedSquaresNearby()
        {
            
        }
        
    }
}