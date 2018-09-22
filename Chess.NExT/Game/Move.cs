using System;

namespace Chess.Game
{
    public class Move : IComparable, IComparable<Move>
    {
        public Player Player { get; }
		
        public Piece Piece { get; }
		
        public Square Destination { get; }
		
        public Board Board { get { return Piece.Board; } }
		
        public BasicGame Game { get { return Board.Game; } }

        private short? value = null;

        public short Value
        {
            get
            {
                if (value.HasValue == false)
                {
                    value = calculateValue();
                }

                // ReSharper disable once PossibleInvalidOperationException
                return value.Value;
            }
        }

        public Move(Player player, Piece piece, Square destination)
        {
            this.Player = player;
            this.Piece = piece;
            this.Destination = destination;
        }

        /// <param name="originalMove"></param>
        /// <param name="game"></param>
        /// <returns>A new <see cref="Chess.Game.Move"/> with Player, Piece, and Destination corresponding to those of <paramref name="originalMove"/>, but
        /// originating within <paramref name="game"/></returns>
        public static Move CreateMatchingMoveForGame(Move originalMove, BasicGame game)
        {
            var board = game.Board;
			
            Player player = game.FindMatchingPlayer(originalMove.Player);
            Piece piece = board.FindMatchingPiece(originalMove.Piece);
            Square destination = board.FindMatchingSquare(originalMove.Destination);
            
            return new Move(player, piece, destination);
        }
		
        public static bool operator > (Move move0, Move move1)
        {
            return move0.Value > move1.Value;
        }

        public static bool operator < (Move move0, Move move1)
        {
            return move0.Value < move1.Value;
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

        protected ref short? calculateValue()
        {
            short startingValue = Board.CalculateRelativeValue(Player);

            short valueAfterMove = calculateRelativeValueAfterMove(this);

            value = (short)(valueAfterMove - startingValue);

            return ref value;
        }

        protected static short calculateRelativeValueAfterMove(Move move)
        {
            var testGame = new TemporaryGame(move.Game);
			
            Move translatedMove = CreateMatchingMoveForGame(move, testGame);

            translatedMove.Commit();
			
            var testBoard = testGame.Board;
            return testBoard.CalculateRelativeValue(translatedMove.Player);
        }

        /// <summary>
        /// Executes this move
        /// </summary>
        public void Commit()
        {
            Piece.move(Destination);
        }
		
        /// <summary>
        /// <seealso cref="Move.Commit()"/>
        /// </summary>
        public void Invoke()
        {
            Commit();
        }

    }
}