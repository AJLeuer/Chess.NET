using System;

namespace Chess.Game
{
    public class Move : GameEntity, IComparable, IComparable<Move> 
    {
        protected static ulong IDs = 0;
            
        public ulong ID { get; } = IDs++;
        
        public Player Player { get; }
		
        public IPiece Piece { get; }
		
        public Square Destination { get; }
		
        public Board Board { get { return Piece.Board; } }
		
        public BasicGame Game { get { return Board.Game; } }

        private short? outcomeValue = null;

        public short OutcomeValue 
        {
            get
            {
                if (outcomeValue.HasValue == false)
                {
                    outcomeValue = calculateResultingBoardValue();
                }
                // ReSharper disable once PossibleInvalidOperationException
                return outcomeValue.Value;
            }
        }

        public Move(Player player, IPiece piece, Square destination)
        {
            this.Player = player;
            this.Piece = piece;
            this.Destination = destination;
        }
        
        
        /// <summary>
        /// Executes this move
        /// </summary>
        public void Commit()
        {
            Piece.Move(Destination);
        }
        
        /// <summary>
        /// Creates a copy of the Game associated with this Move (including the board, all players, and all pieces,
        /// then commits this move inside the simulation and updates the resulting state of the simulated game
        /// </summary>
        /// <returns>A Move object with the associated (Simulated) Game updated to reflect the result of applying this move</returns>
        public Move CommitInSimulation()
        {
            Simulation.Game simGame = new Simulation.Game(this.Game);
            Move simulatedMove = CreateMatchingMoveForGame(this, simGame);
            simulatedMove.Commit();
            return simulatedMove;
        }

        /// <param name="originalMove"></param>
        /// <param name="game"></param>
        /// <returns>A new <see cref="Chess.Game.Move"/> with Player, Piece, and Destination corresponding to those of <paramref name="originalMove"/>, but
        /// originating within <paramref name="game"/></returns>
        public static Move CreateMatchingMoveForGame(Move originalMove, BasicGame game)
        {
            var board = game.Board;
			
            Player player = game.FindMatchingPlayer(originalMove.Player);
            IPiece piece = board.FindMatchingPiece(originalMove.Piece);
            Square destination = board.FindMatchingSquare(originalMove.Destination);
            
            return new Move(player, piece, destination);
        }

        private static Move createSimulatedEquivalentOfMove(Move originalMove)
        {
            Simulation.Game simulatedEquivalentGame = new Simulation.Game(originalMove.Game);

            Move simulatedEquivalentMove = CreateMatchingMoveForGame(originalMove: originalMove, game: simulatedEquivalentGame);

            return simulatedEquivalentMove;
        }
		
        public static bool operator > (Move move0, Move move1)
        {
            return move0.OutcomeValue > move1.OutcomeValue;
        }

        public static bool operator < (Move move0, Move move1)
        {
            return move0.OutcomeValue < move1.OutcomeValue;
        }

        public int CompareTo(object @object) 
        {
            if (@object.GetType() == typeof(Move))
            {
                return CompareTo((Move) @object);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
		
        public int CompareTo(Move move) 
        {
            if (this > move)
            {
                return 1;
            }
            else if (this < move)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        protected ref short? calculateResultingBoardValue()
        {
            var testGame = new Simulation.TemporaryGame(this.Game);
			
            Move translatedMove = CreateMatchingMoveForGame(this, testGame);

            translatedMove.Commit();
			
            var testBoard = testGame.Board;

            (short valueToBlack, short valueToWhite) valueToPlayers = testBoard.CalculateRelativeValueToPlayers();

            short valueToPlayer = (translatedMove.Player.Color == Color.black)
                ? valueToPlayers.valueToBlack
                : valueToPlayers.valueToWhite;
            
            outcomeValue = valueToPlayer;

            return ref outcomeValue;
        }
    }
}