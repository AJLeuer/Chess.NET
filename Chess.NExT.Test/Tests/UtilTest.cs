using System;
using Chess.Utility;
using NUnit.Framework;

namespace Chess.Test.Tests
{
	public static class UtilTest
	{
		[Test]
		public static void CleanShouldRemoveAnyNonNumberOrLetterCharacters()
		{
			String dirty = "%c&l#3🥳an%";

			String cleaned = dirty.CreateCleanedCopy();
			
			Assert.AreEqual("cl3an", cleaned);
		}
	}
}