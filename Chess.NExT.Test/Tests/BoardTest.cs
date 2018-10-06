using Chess.Game;
using Chess.Util;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
    public static class BoardTest
    {
        
        [Test]
        public static void ShouldIdentifyPositionsInsideBoard()
        {
            var board = new Board(squares: Board.DefaultStartingSquares);
            var onBoard1 = new Vec2<int>(1, 7);
            var onBoard2 = new Vec2<int>(7, 0);
            

            board.IsInsideBounds(onBoard1).Should().Be(true);
            board.IsInsideBounds(onBoard2).Should().Be(true);
        }
        
        [Test]
        public static void ShouldIdentifyPositionsOutsideBoard()
        {
            var board = new Board(squares: Board.DefaultStartingSquares);
            var offBoard1 = new Vec2<int>(-1, 7);
            var offBoard2 = new Vec2<int>(0, 8);

            board.IsInsideBounds(offBoard1).Should().Be(false);
            board.IsInsideBounds(offBoard2).Should().Be(false);
        }

        [Test]
        public static void ShouldCalculateTotalValueOfPiecesOfGivenColorOnBoard()
        {
            var squares = new Square[,]
            {
                { new Square('♜'), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
                
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
                
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
                
                { new Square('♚'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
                
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♔') },
                
                { new Square('♝'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♙'), new Square(' ') },
                
                { new Square(' '), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♙'), new Square('♘') },
                
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') }
            };
            
            var board = new Board(squares);
            var blackPlayerMock = new Mock<Player>(Color.black, null);
            var whitePlayerMock = new Mock<Player>(Color.white, null);

            blackPlayerMock.Setup(
                    (Player self) => self.Color)
                .Returns(
                    () => Color.black);
            
            whitePlayerMock.Setup(
                    (Player self) => self.Color)
                .Returns(
                    () => Color.white);


            Player blackPlayer = blackPlayerMock.Object;
            Player whitePlayer = whitePlayerMock.Object;
            
            board.CalculateRelativeValue(blackPlayer).Should().Be(5);
            board.CalculateRelativeValue(whitePlayer).Should().Be(-5);
        }

        [Test]
        public static void ShouldIdentifyMatchingPieceOnAnotherBoard()
        {
            Board board = new Board();

            Board boardCopy = new Board(board);

            Queen queen = (Queen) board['d', 8].Piece.Object;
            Queen duplicateQueen = (Queen) boardCopy['d', 8].Piece.Object;

            board.FindMatchingPiece(duplicateQueen).Should().Be(queen);
        }

        [Test]
        public static void SquarePositionShouldMatchPositionOnGrid()
        {
            var board = new Board(Board.EmptySquares);
            
            for (uint i = 0; i < board.Squares.GetLength(0); i++)
            {
                for (uint j = 0; j < board.Squares.GetLength(1); j++)
                {
                    Square square = board.Squares[i, j];
                    
                    Assert.AreEqual((i, j), square.BoardPosition);
                }
            }
        }
    }
}