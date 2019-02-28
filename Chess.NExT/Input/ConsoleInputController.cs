using Chess.Game;
using Chess.Utility;
using static Chess.Utility.Util;

namespace Chess.Input
{
	public class ConsoleInputController : InputController
	{
		public ConsoleInputController(Player player)
		{
			this.Player = player;
		}
		
		public TextIOInterface IOInterface { private get; set; } = new ConsoleTextIOInterface();
		public Player Player { private get; set; }

		public Move NextMove
		{
			get
			{
				if (Player.MovesMade == 0)
				{
					IOInterface.WriteLine(@"Enter your next move by first giving the current rank and file of the piece you wish to move, followed by the rank and file of the square to which you wish to move that piece, e.g. ""C2 C4""");
				}
				else
				{
					IOInterface.WriteLine("Enter your next move");
				}

				string playerInputText = IOInterface.ReadLine();
				playerInputText = playerInputText.CreateCleanedCopy();
				ushort firstFilePosition = (ushort) playerInputText.IndexOfAny(CharsAThroughZ);
				ushort firstRankPosition = (ushort) playerInputText.IndexOfAny(Numbers0Through9);
				ushort firstRankFileLength = (ushort)((firstRankPosition - firstFilePosition) + 1);
				ushort secondFilePosition = (ushort) playerInputText.LastIndexOfAny(CharsAThroughZ);
				ushort secondRankPosition = (ushort) playerInputText.LastIndexOfAny(Numbers0Through9);
				ushort secondRankFileLength = (ushort)((secondRankPosition - secondFilePosition) + 1);

				string currentPieceRankFileString = playerInputText.Substring(firstFilePosition, firstRankFileLength);
				string destinationRankFileString = playerInputText.Substring(secondFilePosition, secondRankFileLength);

				RankFile currentPieceRankFile = RankFile.CreateRankFileFromString(currentPieceRankFileString);
				RankFile destinationRankFile = RankFile.CreateRankFileFromString(destinationRankFileString); 

				Move move = Move.CreateFromPieceCurrentPositionAndDestination(Player, currentPieceRankFile, destinationRankFile);
				return move;
			}
		}
	}
}