﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Chess.Util;
using SFML.Graphics;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public abstract class Piece : ICloneable
    {
        public static readonly Dictionary<Color, char> defaultSymbols;

        public static readonly Dictionary<Color, String> defaultImageFiles;
   
        protected static ulong IDs = 0;
        
        public ulong ID { get; } = IDs++;

        public char symbol { get; }

        public Color color { get; }

        public abstract ushort value { get; }
        
        public abstract List<Direction> legalMovementDirections { get; }

        public uint movesMade { get; protected set; } = 0;

        protected string spriteImageFilePath;

        protected Texture spriteTexture;

        public Sprite sprite { get; private set; }

        protected Square currentSquare;
        
        public Square square {
            set
            {
                this.currentSquare = value;
                if (currentSquare != null)
                {
                    updateSpritePosition();
                }
            }
            get { return currentSquare; } 
        }
        
        public Board board
        {
            get { return square.board; }
        }

        protected readonly Vec2<uint> startingPosition;

        public RankAndFile position
        {
            get { return square.position; }
        }
        
        public CallBack onCaptured { get; set; }
        
        public static Piece create(char symbol, Square square) {
            Piece piece;
            if (symbol == Pawn.defaultSymbols[white]) {
                piece = new Pawn(white, square);
            }
            else if (symbol == Pawn.defaultSymbols[black]) {
                piece = new Pawn(black, square);
            }
            else if (symbol == Knight.defaultSymbols[white]) {
                piece = new Knight(white, square);
            }
            else if (symbol == Knight.defaultSymbols[black]) {
                piece = new Knight(black, square);
            }
            else if (symbol == Bishop.defaultSymbols[white]) {
                piece = new Bishop(white, square);
            }
            else if (symbol == Bishop.defaultSymbols[black]) {
                piece = new Bishop(black, square);
            }
            else if (symbol == Rook.defaultSymbols[white]) {
                piece = new Rook(white, square);
            }
            else if (symbol == Rook.defaultSymbols[black]) {
                piece = new Rook(black, square);
            }
            else if (symbol == Queen.defaultSymbols[white]) {
                piece = new Queen(white, square);
            }
            else if (symbol == Queen.defaultSymbols[black]) {
                piece = new Queen(black, square);
            }
            else if (symbol == King.defaultSymbols[white]) {
                piece = new King(white, square);
            }
            else if (symbol == King.defaultSymbols[black]) {
                piece = new King(black, square);
            }
            else {
                piece = null;
            }
            return piece;
        }

        public static Piece createByCopy(Piece piece)
        {

//            Type pieceType = piece.GetType();
//
//            var constructor = pieceType.GetConstructor(new[]{pieceType});
//            return (Piece) constructor.Invoke(new[]{piece});

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
        
        protected Piece(char symbol, string spriteImageFilePath, Color color, Square square)
        {
            /* ID init from IDs */
            this.symbol = symbol;
            this.color = color;
            /* movesMade init to 0 */
            this.spriteImageFilePath = spriteImageFilePath;
            /* Don't initialize the actual texture/sprite data. Too
             expensive - we only init it when we need it */
            this.square = square;
            this.startingPosition = new Vec2<uint>(square.position);
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
             expensive - we only init it when we need it */

            /* Don't copy other's square references: we don't know if we're owned
             by a new board or still held by the same, and if we are on the new square/or board,
             they'll have to update our references */
        }
        
        ~Piece() {
            square = null;
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
            square.handleLeavingPiece();

            destination.receiveArrivingPiece(this);

            movesMade++;
        }
        
        public virtual void move(RankAndFile destination) {
            Square destinationSquare = board.getSquare(destination);
            move(destinationSquare);
        }
        
        public virtual List<Square> findAllPossibleLegalMoveDestinations()
        {
            List<Square> squaresLegalToMove = board.getSpecifiedSquares(true, this.color.getOpposite(), position, this.legalMovementDirections.ToArray());

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