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
        public static readonly ushort MaximumPossibleMoveDistance = (ushort) Board.DefaultEmptySquares.GetLength(0);
        
        protected static ulong IDs = 0;

        protected ulong ID { get; } = IDs++;
        
        protected ulong iterations = 0;

        private Board board;
        
        public Board Board 
        { 
            get { return board; }
            
            protected set
            {
                this.board = value;
                
                if (Board != null)
                {
                    Board.Game = this;
                }
            }
        }

        private Player player0;
        private Player player1;
        
        /* holds references to pieces at index (0, 0) through (1, 15) */
        public Player Player0
        {
            get { return player0; }
            
            protected set { initializePlayer(ref player0, value); }
        }
        
        /* holds references to pieces at index (6, 0) through (7, 7) */
        public Player Player1
        {
            get { return player1; }
            
            protected set { initializePlayer(ref player1, value); }
        }
        
        // ReSharper disable once RedundantAssignment
        protected void initializePlayer(ref Player playerMember, Player player)
        {
            playerMember = player;
            
            if (playerMember != null)
            {
                playerMember.Board = Board;
            }
        }

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
        
        protected BasicGame(Board board, Player player0 = null, Player player1 = null)
        {
            this.Board   = board;
            this.Player0 = player0 ?? new AI(white, this.Board);
            this.Player1 = player1 ?? new AI(black, this.Board);
        }

        protected BasicGame(BasicGame other) :
            /* call copy constructor directly if class doesn't expect to have subclasses,
            call Clone() method where inheritance is in play and polymorphism is needed */
            this(new Board(other.Board), 
                 (other.Player0 == null) ? null : other.Player0.Clone(),
                 (other.Player1 == null) ? null : other.Player1.Clone())
        {
            
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract BasicGame Clone();
        
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
        public Player FindMatchingPlayer(Player player) 
        {
            return (player.Color == Player0.Color) ? Player0 : Player1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The Player from this <see cref="Chess.Game"/> with the color opposite <paramref name="player"/></returns>
        public Player FindOpponentPlayer(Player player) 
        {
            return (player.Color != Player0.Color) ? Player0 : Player1;
        }
    }
    
    namespace Graphical
    {
        public class ChessGame : BasicGame
        {
            protected Window window = new Window();

            protected Board GraphicalBoard { get { return (Graphical.Board) base.Board; } }

            public ChessGame() :
                base(new Board())
            {
                GraphicalBoard.InitializeGraphicalElements();
                GraphicalBoard.Initialize2DCoordinates((0, 0));
            }
    
            public ChessGame(BasicGame other) :
                base(other)
            {
                GraphicalBoard.InitializeGraphicalElements();
                GraphicalBoard.Initialize2DCoordinates((0, 0));
            }
    
            public ChessGame(Game.Board board, Player player0, Player player1) :
                base(board, player0, player1)
            {
                GraphicalBoard.InitializeGraphicalElements();
                GraphicalBoard.Initialize2DCoordinates((0, 0));
            }
    
            public override BasicGame Clone()
            {
                return new ChessGame(this);
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
                foreach (var square in GraphicalBoard)
                {
                    var graphicalSquare = square as Graphical.Square;
                    
                    var piece = graphicalSquare.GraphicalPiece;
                        
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
    }

    public class SimulatedGame : BasicGame 
    {

        public SimulatedGame() :
            base(new Board())
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