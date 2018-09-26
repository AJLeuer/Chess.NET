using Chess.Game;
using FluentAssertions;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
	[TestFixture]
	public class SquareTest
	{
		[Test]
		public void WhiteSquaresShouldDetermineOwnColorBasedOnPositionOnBoard()
		{
			Square square = new Square('h', 7);

			square.Color.Should().Be(Color.white);
		}
		
		[Test]
		public void BlackSquaresShouldDetermineOwnColorBasedOnPositionOnBoard()
		{
			Square square = new Square('b', 2);

			square.Color.Should().Be(Color.black);
		}
	}
}