
using Chess.Game;

namespace Chess.Input
{
    public interface InputController
    {
        Player Player { set; }
        Move NextMove { get; }
    }
}