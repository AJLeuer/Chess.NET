﻿using System;
using System.Collections.Generic;
using System.Threading;

using Chess.Utility;

using static Chess.Configuration.Config;
using static Chess.Game.Constants;
using static Chess.Game.Color;
 
using AI = Chess.Game.Real.AI;
using Window = Chess.View.Window;


namespace Chess.Game
{
    public interface GameEntity
    {
        BasicGame Game { get; }
    }
    
    public abstract class BasicGame : ICloneable, GameEntity
    {
        public const ushort MaximumPossibleMoveDistance = BoardWidth;

        protected static ulong IDs = 0;

        protected ulong ID { get; } = IDs++;
        
        public bool GameActive = false;
        
        protected ulong iterations = 0;
        
        public BasicGame Game { get { return this; } }

        private Board board;
        
        public virtual Board Board 
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

            ensureUniquePlayerColors();
        }

        private void ensureUniquePlayerColors()
        {
            if (Player0?.Color == Player1?.Color)
            {
                throw new Exception("One player of each color");
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

        public GameStateAdvancedAction OnGameAdvanced { get; set; } = () => { };
        
        protected BasicGame(Board board, Player player0 = null, Player player1 = null)
        {
            this.Board   = board;
            this.Player0 = player0 ?? new AI(white);
            this.Player1 = player1 ?? new AI(black);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public abstract BasicGame Clone();
        
        public void Play() 
        {
            GameActive = true;
            
            setup();

            ThreadStart gameLoop = () => 
            {
                while (GameActive)
                {
                    advance();
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

        protected void advance()
        {
            playTurn();

            OnGameAdvanced.Invoke();
            
            sleep();
            
            iterations++;
        }

        protected virtual void playTurn()
        {
            CurrentPlayer = (iterations % 2) == 0 ? this.WhitePlayer : this.BlackPlayer;
    
            Move nextMove = CurrentPlayer.DecideNextMove();
    
            nextMove.Commit();
        }

        protected virtual void sleep()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
        
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
        public class Game : BasicGame
        {
            protected Window window = new Window();
            
            public override Chess.Game.Board Board 
            {
                get { return base.Board; }
                protected set
                {                    
                    if ((value is Graphical.Board) == false)
                    {
                        throw new ArgumentException("A Graphical.Game's Board must be a Graphical.Board");
                    }
                    else
                    {
                        base.Board = value;
                    }
                }
            }
            
            public Board Board2D { get { return (Graphical.Board) Board; } }

            public Game() :
                base(new Graphical.Board())
            {
                Board2D.InitializeGraphicalElements();
                Board2D.Initialize2DCoordinates(new Vec2<uint>((MainWindowSize.Width / 4), (MainWindowSize.Height / 32)));
            }
    
            public Game(BasicGame other) :
                base(new Graphical.Board(other.Board),
                     (other.Player0 == null) ? null : other.Player0.Clone(),
                     (other.Player1 == null) ? null : other.Player1.Clone())
            {
                Board2D.InitializeGraphicalElements();
                Board2D.Initialize2DCoordinates(new Vec2<uint>((MainWindowSize.Width / 4), (MainWindowSize.Height / 32)));
            }
    
            public Game(Chess.Game.Board board, Chess.Game.Player player0, Chess.Game.Player player1) :
                base(board, player0, player1)
            {
                Board2D.InitializeGraphicalElements();
                Board2D.Initialize2DCoordinates(new Vec2<uint>((MainWindowSize.Width / 4), (MainWindowSize.Height / 32)));
            }
    
            public override BasicGame Clone()
            {
                return new Game(this);
            }
    
            protected override void setup()
            {
                monitorMouse();
            }
    
            protected override void display()
            {
                window.DispatchEvents();
                window.Clear();
                display2D();
                window.Display();
                Thread.Sleep(TimeSpan.FromMilliseconds(DefaultDisplayRefreshInterval));
            }
    
            protected void display2D()
            {
                Board2D.Draw(window);
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

    namespace Simulation
    {
        public class Game : BasicGame 
        {

            public Game() :
                base(new Simulation.Board())
            {
            
            }

            public Game(BasicGame other) :
                base(new Simulation.Board(other.Board),
                     (other.Player0 == null) ? null : other.Player0.Clone(),
                     (other.Player1 == null) ? null : other.Player1.Clone())
            {

            }

            public Game(Board board, Chess.Game.Player player0, Chess.Game.Player player1) :
                base(board, player0, player1)
            {
            
            }

            public override BasicGame Clone()
            {
                return new Game(this);
            }

            protected override void setup() {}
            
            protected override void sleep()
            {
                //do nothing
            }

            protected override void display()
            {
                Thread.Yield();
            }
        }
    
        /// <summary>
        /// A BasicGame used only for testing the outcomes of potential moves
        /// </summary>
        public class TemporaryGame : Simulation.Game 
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
        
            protected override void playTurn() {}
        }
    }

    
    public delegate void GameStateAdvancedAction();
}