using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SFML.Window;

using Chess.Util;
using Chess.View;
using SFML.Graphics;
using static Chess.Util.Config;
using static Chess.Game.Color;
using Window = Chess.View.Window;


namespace Chess.Game
{
    public abstract class BasicGame : ICloneable
    {
        public static readonly ushort MaximumPossibleMoveDistance = (ushort) Board.EmptySquares.GetLength(0);
        
        protected static ulong IDs = 0;

        protected ulong ID { get; } = IDs++;
        
        protected ulong iterations = 0;

        /* Note: Must be initialized first */
        public virtual Board Board { get; protected set; }
        
        /* holds references to pieces at index (0, 0) through (1, 15) */
        public Player Player0 { get; protected set; }
        
        /* holds references to pieces at index (6, 0) through (7, 7) */
        public Player Player1 { get; protected set; }

        public Player WhitePlayer
        {
            get
            {
                if (Player0.Color == white)
                {
                    return Player0;
                }
                else
                {
                    return Player1;
                }
            }
        }
        
        public Player BlackPlayer
        {
            get
            {
                if (Player1.Color == black)
                {
                    return Player1;
                }
                else
                {
                    return Player0;
                }
            }
        }

        protected Player CurrentPlayer = null;
        
        protected List<GameRecordEntry> gameRecord;

        public GameStateAdvancedAction OnGameAdvanced { get; } = () => { };

        protected BasicGame() :
            this(new Board())
        {
            Player0 = new AI(white, this.Board);
            Player1 = new AI(black, this.Board);
        }

        protected BasicGame(BasicGame other) :
            /* call copy constructor directly if class doesn't expect to have subclasses,
            call Clone() method where inheritance is in play and polymorphism is needed */
            this(new Board(other.Board), 
                 (other.Player0 == null) ? null : other.Player0.Clone(),
                 (other.Player1 == null) ? null : other.Player1.Clone())
        {
            
        }

        protected BasicGame(Board board, Player player0 = null, Player player1 = null)
        {
            this.Board = board;
            this.Player0 = player0;
            this.Player1 = player1;
            board.Game = this;

            initializePlayers();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract BasicGame Clone();
        
        protected void initializePlayers()
        {
            if (Player0 != null)
            {
                Player0.Board = Board;
            }
            if (Player1 != null)
            {
                Player1.Board = Board;
            }
        }
        
        public void PlayGame ()
        {
            GameActive = true;
            
            setup();

            ThreadStart gameLoop = () =>
            {
                while (GameActive)
                {
                    advance();
                    iterations++;
                }
            };
            
            var gameThread = new Thread(gameLoop);
            gameThread.Start();

            while (GameActive) 
            {
                display();
            }
        }

        protected abstract void setup();

        public virtual void advance()
        {
            decideMove();

            OnGameAdvanced.Invoke();
            
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        protected abstract void decideMove();
        
        /// <summary>
        /// Display or record any output intended for the user, where the type of output is defined by the implementation
        /// (i.e. it could be graphical, console-based, written to a log file, etc.)
        /// </summary>
        protected abstract void display();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The Player from this <see cref="Chess.Game"/> with the same color as <paramref name="player"/></returns>
        public Player FindMatchingPlayer(Player player) {
            return (player.Color == Player0.Color) ? Player0 : Player1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The Player from this <see cref="Chess.Game"/> with the color opposite <paramref name="player"/></returns>
        public Player FindOpponentPlayer(Player player) {
            return (player.Color != Player0.Color) ? Player0 : Player1;
        }
    }
    
    public class ChessGame : BasicGame, ChessDrawable
    {

        protected Window window = new Window();

        public Sprite Sprite { get; set; }

        public ChessGame() :
            base()
        {
            InitializeSprite();
        }

        public ChessGame(BasicGame other) :
            base(other)
        {
            InitializeSprite();
        }

        public ChessGame(Board board, Player player0, Player player1) :
            base(board, player0, player1)
        {
            InitializeSprite();
        }

        public override BasicGame Clone()
        {
            return new ChessGame(this);
        }

        public void InitializeSprite()
        {
            Board.InitializeSprite();
        }

        protected override void setup()
        {
            monitorMouse();
        }

        protected override void display()
        {
            window.DispatchEvents();
            window.Clear();
            drawChessBoard();
            window.Display();
            Thread.Sleep(TimeSpan.FromMilliseconds(2));
        }

        protected override void decideMove()
        {
            CurrentPlayer = (iterations % 2) == 0 ? this.Player0 : this.Player1;

            Move nextMove = CurrentPlayer.DecideNextMove();

            nextMove.Commit();
        }

        protected void drawChessBoard()
        {
            foreach (var square in Board)
            {
                var piece = square.Piece;
                    
                if (piece.HasValue)
                {
                    window.Draw(piece.Object.Sprite);
                }
            }
        }

        private void monitorMouse () 
        {
            // var mouseMonitor = new ThreadStart(() => 
            // {
            //     while (GameActive) 
            //     {
            //         if (Mouse.IsButtonPressed(ButtonMain)) 
            //         {
            //             Mouse.GetPosition();
            //         }
            //         Thread.Sleep(TimeSpan.FromMilliseconds(1));
            //     }
            // });
            //
            // var mouseMonitoringThread = new Thread(mouseMonitor);
            //
            // mouseMonitoringThread.Start();
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

        public override BasicGame Clone()
        {
            return new SimulatedGame(this);
        }

        protected override void setup() {}
        
        public override void advance()
        {
            decideMove();

            OnGameAdvanced.Invoke();
            
            //and definitely don't sleep
        }

        protected override void decideMove()
        {
            Move nextMove = CurrentPlayer.DecideNextMove();
            nextMove.Commit();
        }
        
        protected override void display(){}
    }
    
    /// <summary>
    /// A BasicGame used only for testing the outcomes of potential moves
    /// </summary>
    public class TemporaryGame : SimulatedGame
    {
        public TemporaryGame() :
            base()
        {
            
        }

        public TemporaryGame(BasicGame other) :
            base(other)
        {
            
        }

        public TemporaryGame(Board board, Player player0 = null, Player player1 = null) :
            base(board, player0, player1)
        {
            
        }

        public override BasicGame Clone()
        {
            return new TemporaryGame(this);
        }
        
        protected override void decideMove() {}
    }
    
    public delegate void GameStateAdvancedAction();
}