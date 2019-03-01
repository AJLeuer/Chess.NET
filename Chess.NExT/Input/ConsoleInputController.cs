using Chess.Game;
using Chess.Utility;
using static Chess.Utility.Util;

namespace Chess.Input
{
	public class ConsoleInputController : InputController
	{
		public ConsoleInputController()
		{
			
		}
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
				Move move = null;

				while (move == null)
				{
					try
					{
						if (Player.MovesMade == 0)
						{
							IOInterface.WriteLine(@"Enter your next move by first giving the current rank and file of the piece you wish to move, followed by the rank and file of the square to which you wish to move that piece, e.g. ""C2 C4"":");
						}
						else
						{
							IOInterface.WriteLine("Enter your next move:");
						}

						string playerInputText = IOInterface.ReadLine();
						playerInputText = playerInputText.CreateCleanedCopy();
						ushort firstFilePosition    = (ushort) playerInputText.IndexOfAny(CharsAThroughZ);
						ushort firstRankPosition    = (ushort) playerInputText.IndexOfAny(Numbers0Through9);
						ushort firstRankFileLength  = (ushort)((firstRankPosition - firstFilePosition) + 1);
						ushort secondFilePosition   = (ushort) playerInputText.LastIndexOfAny(CharsAThroughZ);
						ushort secondRankPosition   = (ushort) playerInputText.LastIndexOfAny(Numbers0Through9);
						ushort secondRankFileLength = (ushort)((secondRankPosition - secondFilePosition) + 1);

						string currentPieceRankFileString = playerInputText.Substring(firstFilePosition, firstRankFileLength);
						string destinationRankFileString  = playerInputText.Substring(secondFilePosition, secondRankFileLength);

						RankFile currentPieceRankFile = RankFile.CreateRankFileFromString(currentPieceRankFileString);
						RankFile destinationRankFile  = RankFile.CreateRankFileFromString(destinationRankFileString); 

						move = CreateMoveFromPlayerInput(currentPieceRankFile, destinationRankFile);
					}
					catch (InvalidMoveException badMove)
					{
						IOInterface.WriteLine($"Invalid move. {badMove.Message}. Please try again.");
					}
				}

				return move;
			}
		}
		
		public Move CreateMoveFromPlayerInput(RankFile pieceCurrentPosition, RankFile pieceDesiredDestination)
		{
			validate(pieceCurrentPosition, pieceDesiredDestination);
			
			BasicGame        game  = Player.Game;
			Chess.Game.Board board = game.Board;

			IPiece piece       = board[pieceCurrentPosition].Piece.Object;
			Square destination = board[pieceDesiredDestination];
            
			return new Move(Player, piece, destination);
		}

		private void validate(RankFile pieceCurrentPosition, RankFile pieceDesiredDestination)
		{
			BasicGame        game  = Player.Game;
			Chess.Game.Board board = game.Board;
			IPiece piece = board[pieceCurrentPosition].Piece.Object;
			
			if (piece == null)
			{
				throw new InvalidMoveException($"There is no piece at the position {pieceCurrentPosition.ToString()}");
			}

			if (piece.CanMoveTo(board[pieceDesiredDestination]) == false)
			{
				throw new InvalidMoveException($"{piece.GetType().Name} at {pieceCurrentPosition.ToString()} cannot move to {pieceDesiredDestination.ToString()}");
			}

			if (Player.Pieces.Contains(piece) == false)
			{
				throw new InvalidMoveException($"{piece.GetType().Name} at {pieceCurrentPosition.ToString()} does not belong to {Player.Name}");
			}
		}
	}
}