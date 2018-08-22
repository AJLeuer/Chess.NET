using NUnit.Framework;

using Chess.Game;

using File = System.Char;
using Rank = System.UInt16;

namespace Chess.NExT.Test
{
    [TestFixture]
    public static class KnightTest
    {

        [Test]
        public static void ShouldFindAllValidMoveDestinations()
        {
            Board board = new Board();

            Square square = board['e', 4];
            
            Piece knight = new Knight(Color.white);
        }
        
    }
}