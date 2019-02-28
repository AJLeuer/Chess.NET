using System;
using Chess.Game;

namespace Chess.Input
{
	public class ConsoleInputController : InputController
	{
		public Player Player { private get; set; }

		public Move NextMove
		{
			get
			{
				if (Player.MovesMade == 0)
				{
					Console.WriteLine(@"Enter your next move by first giving the current rank and file of the piece you wish to move, followed by the rank and file of the square to which you wish to move that piece, e.g. ""C2 C4""");
				}
				else
				{
					Console.WriteLine("Enter your next move");
				}

				string playerInputText = Console.ReadLine();
				
				throw new NotImplementedException();
			}
		}
	}
}