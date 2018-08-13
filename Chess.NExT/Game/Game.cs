using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using Window = Chess.View.Window;

using Chess.Util;
using static Chess.Util.Config;
using static Chess.Game.Color;


namespace Chess.Game
{
    public abstract class BasicGame : ICloneable
    {
        protected ulong gameLoops = 0;

        /* Note: Must be initialized first */
        public Board board { get; protected set; }
        
        /* holds references to pieces at index (0, 0) through (1, 15) */
        protected Player player0;
        
        /* holds references to pieces at index (6, 0) through (7, 7) */
        protected Player player1;

        protected Player currentPlayer = null;

        public abstract void advanceGame(bool overrideMoveDecision = false, MoveAction overridingMove = null);

        protected List<GameRecordEntry> gameRecord;

        protected BasicGame() :
            this(new Board(), new AI(white, null), new AI(black, null))
        {
            
        }

        protected BasicGame(BasicGame other) :
            this(new Board(other.board), other.player0.Clone(), other.player1.Clone())
        {
            
        }

        protected BasicGame(Board board, Player player0, Player player1)
        {
            this.board = board;
            this.player0 = player0;
            this.player1 = player1;

            player0.board = board;
            player1.board = board; 
            
            board.game = this;

            foreach (var file in (Square[][]) board)
            {
                foreach (Square square in file)
                {
                    var piece = square.piece;
                    
                    if (piece.HasValue)
                    {
                        piece.Value.initializeSpriteTexture();
                    }
                }
            }
        }

        ~BasicGame(){}

        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract BasicGame Clone();

        public abstract void playGame();

        /**
         * @return The Player with the same color as the Player passed in as an argument
         */
        Player findMatchingPlayer(Player player) {
            return (player.color == player0.color) ? player0 : player1;
        }

        /**
         * @return The Player with the color opposite the Player passed in as an argument
         */
        Player findOpponentPlayer(Player player) {
            return (player.color != player0.color) ? player0 : player1;
        }

        /**
         * @return A MoveIntent object with the matching Piece found by calling this->board.findMatch(move.piece), such that the returned
         * MoveIntent could be used to make the same move but in this game
         */
        MoveAction tranlateMoveIntent(MoveAction move)
        {
            Player player = findMatchingPlayer(move.player);
            Piece piece = board.findMatchingPiece(move.piece);
            Square destination = board.findMatchingSquare(move.destination);
            
            return new MoveAction(player, piece, destination);
        }
    }
    
    public class ChessGame : BasicGame 
    {

        protected Window window = new Window();

        public ChessGame() :
            base()
        {
            
        }

        public ChessGame(BasicGame other) :
            base(other)
        {
            
        }

        public ChessGame(Board board, Player player0, Player player1) :
            base(board, player0, player1)
        {
            
        }

        ~ChessGame () {}

        public override BasicGame Clone()
        {
            return new ChessGame(board, player0, player1);
        }

        public override void playGame () {
            monitorMouse();

            while (gameActive) {
                advanceGame();
                display();
                gameLoops++;
                Thread.Sleep(TimeSpan.FromMilliseconds(4));
            }
        }
        
        public override void advanceGame(bool overrideMoveDecision = false, MoveAction overridingMove = null)
        {
            if ((gameLoops % 2) == 0) 
            {
                currentPlayer = this.player0;
            }
            else {
                currentPlayer = this.player1;
            }

            MoveAction nextMove = currentPlayer.decideNextMove();
            
            nextMove.commit();
            
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    
        public void display ()
        {
            var stringRepresentation = board.ToString();

            Vec2<uint> windowSize = window.Size;
            var middle = windowSize / 2;

            foreach (var file in (Square[][]) board)
            {
                foreach (Square square in file)
                {
                    var piece = square.piece;
                    
                    if (piece.HasValue)
                    {
                        window.Draw(piece.Value.sprite);
                    }
                }
            }

            window.displayText(stringRepresentation, windowForegroundColor, middle);

            window.display();

        }
    
        public void monitorMouse () {
            var mouseMonitor = new ThreadStart(() => 
            {
                while (gameActive) {
                    if (Mouse.IsButtonPressed(buttonMain)) {

                        Vec2<int> mousePosition = Mouse.GetPosition();
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(1));
                }
            });
            
            var mouseMonitoringThread = new Thread(mouseMonitor);
            
            mouseMonitoringThread.Start();
        }
    }
    
    public class SimulatedGame : BasicGame {

        public SimulatedGame() :
            base()
        {
            
        }

        public SimulatedGame(BasicGame other) :
            base(other)
        {
            
        }

        public SimulatedGame(Board board, Player player0, Player player1) :
            base(board, player0, player1)
        {
            
        }

        ~SimulatedGame() {}

        public override BasicGame Clone()
        {
            return new SimulatedGame(board, player0, player1);
        }
    
        public override void playGame() {
            while (gameActive) {
                advanceGame();
                gameLoops++;
            }
        }    
        
        public override void advanceGame(bool overrideMoveDecision = false, MoveAction overridingMove = null)
        {
            
            if (overrideMoveDecision) {
                overridingMove.commit();
            }
            else {

                MoveAction nextMove = currentPlayer.decideNextMove();
                nextMove.commit();
            }
            //and definitely don't sleep
        }
    }

}