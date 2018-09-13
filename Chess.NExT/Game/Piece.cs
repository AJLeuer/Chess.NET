﻿using System;
using System.Collections.Generic;
using Chess.Util;
using SFML.Graphics;
using static Chess.Game.Color;

namespace Chess.Game
{
    public abstract class Piece : ICloneable
    {
        public static readonly Dictionary<Color, Char> defaultSymbols = new Dictionary<Color, Char>();

        public static readonly Dictionary<Color, string> defaultImageFiles = new Dictionary<Color, string>();
   
        protected static ulong IDs = 0;
        
        public ulong ID { get; } = IDs++;

        public char symbol { get; }
        
        public abstract char ASCIISymbol { get; }

        public Color color { get; }

        public abstract ushort Value { get; }
        
        public abstract List<Direction> LegalMovementDirections { get; }

        public uint movesMade { get; protected set; } = 0;

        protected string spriteImageFilePath;

        protected Texture spriteTexture;

        public Sprite sprite { get; private set; }

        protected Optional<Square> square;
        
        public Square Square {
            set
            {
                this.square = value;
                
                if (square.HasValue)
                {
                    updateSpritePosition();
                    
                    if (movesMade == 0)
                    {
                        this.startingPosition = new Vec2<uint>(square.Value.position);
                    } 
                }
            }
            get { return square.Value; } 
        }
        
        public Board board
        {
            get { return Square.board; }
        }

        protected Vec2<uint> startingPosition;

        public RankAndFile position
        {
            get { return Square.position; }
        }
        
        public CallBack onCaptured { get; set; }
        
        public static Piece create(char symbol) {
            Piece piece;
            if (symbol == Pawn.defaultSymbols[white]) {
                piece = new Pawn(white);
            }
            else if (symbol == Pawn.defaultSymbols[black]) {
                piece = new Pawn(black);
            }
            else if (symbol == Knight.defaultSymbols[white]) {
                piece = new Knight(white);
            }
            else if (symbol == Knight.defaultSymbols[black]) {
                piece = new Knight(black);
            }
            else if (symbol == Bishop.defaultSymbols[white]) {
                piece = new Bishop(white);
            }
            else if (symbol == Bishop.defaultSymbols[black]) {
                piece = new Bishop(black);
            }
            else if (symbol == Rook.defaultSymbols[white]) {
                piece = new Rook(white);
            }
            else if (symbol == Rook.defaultSymbols[black]) {
                piece = new Rook(black);
            }
            else if (symbol == Queen.defaultSymbols[white]) {
                piece = new Queen(white);
            }
            else if (symbol == Queen.defaultSymbols[black]) {
                piece = new Queen(black);
            }
            else if (symbol == King.defaultSymbols[white]) {
                piece = new King(white);
            }
            else if (symbol == King.defaultSymbols[black]) {
                piece = new King(black);
            }
            else {
                piece = null;
            }
            return piece;
        }
        

        /// <summary>
        ///    Creates a new piece by copying the piece passed
        /// </summary>
        /// <param name="piece">
        ///     The piece to copy
        /// </param>
        /// <returns>A copy of piece</returns>
        /// <exception cref="TypeInitializationException"></exception>
        public static Piece create(Piece piece)
        {
            switch (piece) 
            {
                case Pawn pawn:
                {
                    return new Pawn(pawn);
                }
                case Knight knight: {
                    return new Knight(knight);
                }
                case Bishop bishop: {
                    return new Bishop(bishop);
                }
                case Rook rook: {
                    return new Rook(rook);
                }
                case Queen queen: {
                    return new Queen(queen);
                }
                case King king: {
                    return new King(king);
                }
                default: {
                    throw new TypeInitializationException(piece.GetType().Name, new Exception("Unknown subclass of Piece"));
                }
            }
        }
        
        protected Piece(char symbol, string spriteImageFilePath, Color color)
        {
            /* ID init from IDs */
            this.symbol = symbol;
            this.color = color;
            /* movesMade init to 0 */
            this.spriteImageFilePath = spriteImageFilePath;
            /* Don't initialize the actual texture/sprite data. Too
             expensive - we only init it when we need it */
        }
        
        protected Piece(Piece other)
        {
            //Pieces resulting from copies have their own, unique IDs
            symbol = other.symbol;
            color = other.color;
            /* movesMade is already init to 0 */
            spriteImageFilePath = other.spriteImageFilePath;
            startingPosition = other.startingPosition;
            
            /* Don't initialize the actual texture/sprite data. Too
             expensive - we only init it when we need it (lazily) */

            /* Don't copy other's square references: we don't know if we're owned
             by a new board or still held by the same, and if we are on the new square/board,
             they'll have to update our references */
        }
        
        ~Piece() {
            Square = null;
        }
        
        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract Piece Clone();

        public void onCapture()
        {
            onCaptured.Invoke();
        }
	    
        /**
        * Returns true if there exists at least one Square that this Piece can legally move to,
        * false otherwise
        */
        public bool canMove()
        {
            var moves = findAllPossibleLegalMoveDestinations();

            bool canMove = (moves.Count > 0);

            return canMove;
        }
        
        public virtual void move(Square destination) {
            Square.handleLeavingPiece();

            destination.receiveArrivingPiece(this);

            movesMade++;
        }
        
        public virtual void move(RankAndFile destination) {
            Square destinationSquare = board.getSquare(destination);
            move(destinationSquare);
        }
        
        public virtual List<Square> findAllPossibleLegalMoveDestinations()
        {
            Predicate<Square> squareChecker = (Square squareToCheck) =>
            {
                if (squareToCheck.isEmpty)
                {
                    return true;
                }
                else /* if (squareToCheck.isOccupied) */ 
                {
                    return this.color.getOpposite() == squareToCheck.Piece.Value.color;
                }
            };
            
            List<Square> squaresLegalToMove = board.SearchForSquares(
                squareChecker, this.position, directions: this.LegalMovementDirections.ToArray());
            

            return squaresLegalToMove;
        }
        
        public void initializeSpriteTexture () {
			spriteTexture = new Texture(spriteImageFilePath);
			sprite = new Sprite(this.spriteTexture);
			updateSpritePosition();
		}
        
        public void updateSpritePosition () {
            if (sprite != null)
            {
                sprite.Position = position.convertToPosition();
            }
        }

    }
}