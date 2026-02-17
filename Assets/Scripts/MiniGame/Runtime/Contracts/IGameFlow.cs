using System;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.Contracts
{
    public interface IGameFlow
    {
        GameState State { get; }
        event Action<GameState> StateChanged;
        void StartGame();
        void CompleteGame(bool isWin);
        void Reset();
    }
}
