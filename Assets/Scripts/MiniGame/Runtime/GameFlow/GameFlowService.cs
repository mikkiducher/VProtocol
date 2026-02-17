using System;
using VProtocol.MiniGame.Runtime.Contracts;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.GameFlow
{
    public sealed class GameFlowService : IGameFlow
    {
        public GameState State { get; private set; } = GameState.Init;

        public event Action<GameState> StateChanged;

        public void StartGame()
        {
            SetState(GameState.Playing);
        }

        public void CompleteGame(bool isWin)
        {
            SetState(isWin ? GameState.Win : GameState.Lose);
        }

        public void Reset()
        {
            SetState(GameState.Init);
        }

        private void SetState(GameState state)
        {
            if (State == state)
            {
                return;
            }

            State = state;
            StateChanged?.Invoke(State);
        }
    }
}
