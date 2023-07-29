using DLS.Game.GameStates;

namespace DLS.Game.Messages
{
    public struct GameStateMessage
    {
        public GameState State { get; }

        public GameStateMessage(GameState state)
        {
            State = state;
        }
    }
}