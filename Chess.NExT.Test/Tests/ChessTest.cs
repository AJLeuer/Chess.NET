using Chess.Game;
using Chess.Util;
using FluentAssertions;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
    public static class ChessTest
    {
        [Test]
        public static void ShouldConvertCoordinatePositionToRankAndFile()
        {
            Vec2<uint> position = new Vec2<uint>(3, 3);
            
            RankAndFile boardPosition = position; //invokes conversion operator

            boardPosition.Should().BeEquivalentTo(new RankAndFile('d', 5));
        }
        
        [Test]
        public static void ShouldConvertRankAndFileToPosition()
        {
            RankAndFile g7 = new RankAndFile('g', 7);
            
            Vec2<uint> position = g7; //invokes conversion operator

            position.Should().BeEquivalentTo(new Vec2<uint>(6, 1));
        }

        [Test]
        public static void RankAndFileConversionShouldBeReversible()
        {
            RankAndFile a8 = new RankAndFile('a', 8);
            
            Vec2<uint> position = a8; //invokes conversion operator

            ((RankAndFile) position).Should().BeEquivalentTo(a8);
        }
    }
}