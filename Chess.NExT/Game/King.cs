using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public interface IKing : IPiece {}

    public static class KingExtensions
    {
        public static bool DetermineIfCheckMateExists(this IKing king)
        {
            List<Move> movesAvailableToKing = king.FindAllPossibleLegalMoves();

            bool atLeastOneSafeMoveExists = false;
            
            foreach (var move in movesAvailableToKing)
            {
                var gameState = move.CommitInSimulation();
                IKing kingAfterMove = (IKing) gameState.Piece;
                var opponentPieces = gameState.Game.FindOpponentPlayer(gameState.Piece.Player).Pieces;

                bool atLeastOneOpponentPieceAbleToMoveToKingsPosition = false;
                
                foreach (var opponentPiece in opponentPieces)
                {
                    if (opponentPiece.CanMoveTo(kingAfterMove.Square))
                    {
                        atLeastOneOpponentPieceAbleToMoveToKingsPosition = true;
                        break;
                    }
                }

                if (atLeastOneOpponentPieceAbleToMoveToKingsPosition == false)
                {
                    atLeastOneSafeMoveExists = true;
                    break;
                }
            }

            return (atLeastOneSafeMoveExists == false);
        }
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

            public override void Move(RankFile destination)
            {
                //todo: add move legality checking
                base.Move(destination);
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

            public override char ASCIISymbol { get { return 'K'; } }

            public override ushort Value { get { return 40; } }

            public override List<Direction> LegalMovementDirections { get { return DefaultLegalMovementDirections; } }

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

            public override void Move(RankFile destination)
            {
                //todo: add move legality checking
                base.Move(destination);
            }
        }
    }
}