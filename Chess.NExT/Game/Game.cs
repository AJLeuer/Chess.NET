using System;
using System.Collections.Generic;
using System.Threading;
using SFML.Window;

using Chess.Util;
using static Chess.Util.Config;
using static Chess.Game.Color;
using Window = Chess.View.Window;


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

        public void AdvanceGame()
        {
            AdvanceGame(Optional<MoveAction>.Empty);
        }
        
        public void AdvanceGame(Optional<MoveAction> overridingMove)
        {
            decideMove(overridingMove);

            onGameAdvanced.Invoke(this);
        }

        protected abstract void decideMove(Optional<MoveAction> overridingMove);

        protected List<GameRecordEntry> gameRecord;
        
        public GameStateAdvancedAction onGameAdvanced { get; private set; }

        protected BasicGame() :
            this(new Board())
        {
            player0 = new AI(white, this.board);
            player1 = new AI(black, this.board);
        }

        protected BasicGame(BasicGame other) :
            /* call copy constructor directly if class doesn't expect to have subclasses,
            call Clone() method where inheritance is in play and polymorphism is needed */
            this(new Board(other.board), other.player0.Clone(), other.player1.Clone())
        {
            
        }

        protected BasicGame(Board board, Player player0 = null, Player player1 = null)
        {
            this.board = board;
            this.player0 = player0;
            this.player1 = player1;
            board.game = this;

            initializePlayers();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract BasicGame Clone();
        
        protected void initializePlayers()
        {
            if ((player0 != null) && (player1 != null))
            {
                player0.Board = board;
                player1.Board = board;
            }
        }
        
        protected void initializeSpriteTextures()
        {
            foreach (var file in board)
            {
                foreach (Square square in file)
                {
                    var piece = square.Piece;

                    if (piece.HasValue)
                    {
                        piece.Value.initializeSpriteTexture();
                    }
                }
            }
        }

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
        MoveAction translate(MoveAction move)
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
            initializeSpriteTextures();
        }

        public ChessGame(BasicGame other) :
            base(other)
        {
            initializeSpriteTextures();
        }

        public ChessGame(Board board, Player player0, Player player1) :
            base(board, player0, player1)
        {
            initializeSpriteTextures();
        }

        public override BasicGame Clone()
        {
            return new ChessGame(board, player0, player1);
        }

        public override void playGame () {
            monitorMouse();

            while (gameActive) {
                AdvanceGame();
                display();
                gameLoops++;
                Thread.Sleep(TimeSpan.FromMilliseconds(4));
            }
        }

        protected override void decideMove(Optional<MoveAction> overridingMove)
        {
            currentPlayer = (gameLoops % 2) == 0 ? this.player0 : this.player1;

            MoveAction nextMove = currentPlayer.decideNextMove();

            nextMove.commit();

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        public void display ()
        {
            var stringRepresentation = board.ToString();

            Vec2<uint> windowSize = window.Size;
            var middle = windowSize / 2;

            foreach (var file in board)
            {
                foreach (Square square in file)
                {
                    var piece = square.Piece;
                    
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
                AdvanceGame();
                gameLoops++;
            }
        }    

        protected override void decideMove(Optional<MoveAction> overridingMove)
        {
                        
            if (overridingMove.HasValue) {
                overridingMove.Value.commit();
            }
            else {

                MoveAction nextMove = currentPlayer.decideNextMove();
                nextMove.commit();
            }
            //and definitely don't sleep
        }
    }
    
    public delegate void GameStateAdvancedAction(BasicGame game);
}