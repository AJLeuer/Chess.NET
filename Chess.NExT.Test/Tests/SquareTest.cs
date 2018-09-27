using Chess.Game;
using FluentAssertions;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
	public static class SquareTest
	{
		[Test]
		public static void WhiteSquaresShouldDetermineOwnColorBasedOnPositionOnBoard()
		{
			Square square = new Square('h', 7);

			square.Color.Should().Be(Color.white);
		}
		
		[Test]
		public static void BlackSquaresShouldDetermineOwnColorBasedOnPositionOnBoard()
		{
			Square square = new Square('b', 2);

			square.Color.Should().Be(Color.black);
		}
	}
}