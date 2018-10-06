﻿using System.Collections.Generic;
using Chess.Game;
using FluentAssertions;
using NUnit.Framework;
using File = System.Char;
using Rank = System.UInt16;

namespace Chess.NExT.Test.Tests
{
    public static class KnightTest
    {
        [Test]
        public static void ShouldFindAllValidMoveDestinations()
        {
            Board board = new Board(squares: Board.EmptySquares);

            Square square = board['e', 4];
            
            Piece knight = new Knight(Color.white);

            square.Piece = knight;

            List<Square> possibleMoves = knight.FindAllPossibleLegalMoveDestinations();
            
            possibleMoves.Should().BeEquivalentTo(
                board['f', 2], 
                board['d', 2], 
                board['c', 3],
                board['c', 5], 
                board['d', 6], 
                board['f', 6], 
                board['g', 5] , 
                board['g', 3]);
        }
        
    }
}