using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Game.Simulation;
using Chess.Utility;
using static Chess.Utility.Util;
using Pawn = Chess.Game.Graphical.Pawn;

namespace Chess.Game
{
    public class Move : GameEntity, IComparable, IComparable<Move> 
    {
        public enum MoveType
        {
            Generic,
            Capture,
            EnPassant,
            Castle,
            PawnPromotion,
            Check,
            Checkmate
        }
        
        protected static ulong IDs = 0;
            
        public ulong ID { get; } = IDs++;

        public ISet<MoveType> Type
        {
            get { return DetermineTypes(this); }
        }

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
            Move simulatedMove = createSimulatedEquivalentOfMove(this);
            simulatedMove.Commit();
            return simulatedMove;
        }
        
        private static Move createSimulatedEquivalentOfMove(Move originalMove)
        {
            Simulation.Game simulatedEquivalentGame = new Simulation.Game(originalMove.Game);

            Move simulatedEquivalentMove = CreateMatchingMoveForGame(originalMove: originalMove, game: simulatedEquivalentGame);

            return simulatedEquivalentMove;
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

        private ISet<MoveType> DetermineTypes(Move move)
        {
            var types = new HashSet<MoveType>();
            
            if (move.IsOfCaptureType())
            {
                types.Add(MoveType.Capture);
            }
            
            if (move.IsAPawnPromotion())
            {
                types.Add(MoveType.PawnPromotion);
            }

            if (move.CausesCheck())
            {
                types.Add(MoveType.Check);
            }

            if (types.Any() == false)
            {
                types.Add(MoveType.Generic);
            }

            return types;
        }
        
        public bool IsOfCaptureType()
        {
            return this.Destination.IsOccupied;
        }
        
        public bool IsAPawnPromotion()
        {
            if (this.Piece.IsOfType<IPawn>())
            {
                Pawn pawn = this.Piece as Pawn;

                switch (pawn?.Color) 
                {
                    case Color.White _:
                        if (this.Destination.IsOnBlackBackRank())
                        {
                            return true;
                        }

                        break;
                    case Color.Black _:
                        if (this.Destination.IsOnBlackBackRank())
                        {
                            return true;
                        }

                        break;
                }
            }
            return false;
        }
        
        public bool CausesCheck()
        {
            Move simulation = CommitInSimulation();
            List<Square> possibleMoveDestinationsAfterCommittingThisMove = simulation.Piece.FindAllPossibleLegalMoveDestinations();

            foreach (Square square in possibleMoveDestinationsAfterCommittingThisMove)
            {
                if (square.IsOccupied)
                {
                    if (square.Piece.Object.IsOfType<IKing>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}