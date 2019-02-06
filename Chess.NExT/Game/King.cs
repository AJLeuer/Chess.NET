using System;
using System.Collections.Generic;
using System.Linq;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public interface IKing : IPiece {}

    public static class KingDefaults
    {
        public const ushort MaximumMoveDistance = 1;
    }
    
    namespace Simulation
    {
        public class King : Piece, IKing 
        {

            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction>
            {
                up,
                down,
                left,
                right,
                upLeft,
                upRight,
                downLeft,
                downRight
            };

            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♚'},
                {white, '♔'}
            };

            public override char ASCIISymbol { get { return 'K'; } }

            public override ushort Value { get { return 40; } }

            public override List<Direction> LegalMovementDirections { get { return DefaultLegalMovementDirections; } }
            
            public override ushort MaximumMoveDistance { get { return KingDefaults.MaximumMoveDistance; } }

            public King(IKing other) :
                base(other)
            {

            }

            public King(Color color) :
                base(DefaultSymbols[color], color)
            {

            }

            public King(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }

            public override IPiece Clone()
            {
                return new King(this);
            }

            public override List<Move> FindAllPossibleLegalMoves()
            {
                List<Move> movesPotentiallyAvailable = base.FindAllPossibleLegalMoves();
                var safeMoves = new List<Move>();

                foreach (var move in movesPotentiallyAvailable)
                {
                    var gameState = move.CommitInSimulation();
                    IKing kingAfterMove = (IKing) gameState.Piece;
                    var opponentPieces = gameState.Game.FindOpponentPlayer(gameState.Piece.Player).Pieces;

                    bool destinationIsSafe = true;

                    foreach (var piece in opponentPieces)
                    {
                        var opponentPiece = (Piece) piece;
                        //In this extreme edge case and highly unusual scenario, we will need to call the base implementation
                        //of CanMoveTo() in order to avoid the infinite recursion that would result from a King needing to know if another King can
                        //potentially move to a certain square before *it* can know whether it itself can legally move there
                        if (opponentPiece.canMoveTo(kingAfterMove.Square))
                        {
                            destinationIsSafe = false;
                            break;
                        }
                    }

                    if (destinationIsSafe)
                    {
                        safeMoves.Add(move);
                    }
                }

                return safeMoves;
            }
            
            public override bool canMoveTo(Chess.Game.Square destination)
            {
                var moves = base.FindAllPossibleLegalMoves();

                IEnumerable<Move> matchingAllowedMove = moves.Where((Move move) => { return move.Destination == destination; });

                if (matchingAllowedMove.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    
    namespace Graphical
    {
        public class King : Piece, IKing
        {
            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction>
            {
                up,
                down,
                left,
                right,
                upLeft,
                upRight,
                downLeft,
                downRight
            };

            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♚'},
                {white, '♔'}
            };

            public static readonly Dictionary<Color, String> DefaultSpriteImageFiles = new Dictionary<Color, String>
            {
                {black, "./Assets/Bitmaps/BlackKing.png"},
                {white, "./Assets/Bitmaps/WhiteKing.png"}
            };

            public override char ASCIISymbol
            {
                get { return 'K'; }
            }

            public override ushort Value
            {
                get { return 40; }
            }

            public override List<Direction> LegalMovementDirections
            {
                get { return DefaultLegalMovementDirections; }
            }

            public override ushort MaximumMoveDistance
            {
                get { return KingDefaults.MaximumMoveDistance; }
            }

            public King(IKing other) :
                base(other)
            {
                SpriteImageFilePath = DefaultSpriteImageFiles[this.Color];
            }

            public King(Color color) :
                base(DefaultSymbols[color], color, DefaultSpriteImageFiles[color])
            {

            }

            public King(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }

            public override IPiece Clone()
            {
                return new King(this);
            }

            public override List<Move> FindAllPossibleLegalMoves()
            {
                List<Move> movesPotentiallyAvailable = base.FindAllPossibleLegalMoves();
                var safeMoves = new List<Move>();

                foreach (var move in movesPotentiallyAvailable)
                {
                    var gameState = move.CommitInSimulation();
                    IKing kingAfterMove = (IKing) gameState.Piece;
                    var opponentPieces = gameState.Game.FindOpponentPlayer(gameState.Piece.Player).Pieces;

                    bool destinationIsSafe = true;

                    foreach (var piece in opponentPieces)
                    {
                        var opponentPiece = (Chess.Game.Piece) piece;
                        //In this extreme edge case and highly unusual scenario, we will need to call the base implementation
                        //of CanMoveTo() in order to avoid the infinite recursion that would result from a King needing to know if another King can
                        //potentially move to a certain square before *it* can know whether it itself can legally move there
                        if (opponentPiece.canMoveTo(kingAfterMove.Square))
                        {
                            destinationIsSafe = false;
                            break;
                        }
                    }

                    if (destinationIsSafe)
                    {
                        safeMoves.Add(move);
                    }
                }

                return safeMoves;
            }
            
            public override bool canMoveTo(Chess.Game.Square destination)
            {
                var moves = base.FindAllPossibleLegalMoves();

                IEnumerable<Move> matchingAllowedMove = moves.Where((Move move) => { return move.Destination == destination; });

                if (matchingAllowedMove.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}