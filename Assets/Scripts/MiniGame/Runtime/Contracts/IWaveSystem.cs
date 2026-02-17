using System;
using VProtocol.MiniGame.Config.Levels;
using VProtocol.MiniGame.Config.Waves;

namespace VProtocol.MiniGame.Runtime.Contracts
{
    public interface IWaveSystem
    {
        bool IsCompleted { get; }
        event Action<WaveSpawnDescriptor> SpawnRequested;
        event Action WaveCompleted;
        void Initialize(LevelConfig levelConfig);
        void StartWaves();
        void Tick(float deltaTime);
        void Reset();
    }
}
