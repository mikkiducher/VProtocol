using System;
using System.Collections.Generic;
using VProtocol.MiniGame.Config.Levels;
using VProtocol.MiniGame.Config.Waves;
using VProtocol.MiniGame.Runtime.Contracts;

namespace VProtocol.MiniGame.Runtime.WaveSystem
{
    public sealed class WaveSystemService : IWaveSystem
    {
        private sealed class SpawnRuntime
        {
            public SpawnRuntime(WaveSpawnDescriptor descriptor)
            {
                Descriptor = descriptor;
                Remaining = Math.Max(0, descriptor.Count);
                StartAfterSeconds = Math.Max(0f, descriptor.StartAfterSeconds);
                Cooldown = 0f;
            }

            public WaveSpawnDescriptor Descriptor { get; }
            public int Remaining { get; set; }
            public float StartAfterSeconds { get; set; }
            public float Cooldown { get; set; }
            public bool IsStarted => StartAfterSeconds <= 0f;
            public bool IsCompleted => Remaining <= 0;
        }

        private readonly List<SpawnRuntime> _spawnPlan = new();
        private bool _isRunning;

        public bool IsCompleted { get; private set; }

        public event Action<WaveSpawnDescriptor> SpawnRequested;
        public event Action WaveCompleted;

        public void Initialize(LevelConfig levelConfig)
        {
            _spawnPlan.Clear();
            IsCompleted = false;
            _isRunning = false;

            if (levelConfig == null || levelConfig.WaveConfig == null)
            {
                var spamFallback = new WaveSpawnDescriptor();
                spamFallback.Configure("SpamBot", 10, 0.75f, 0f);
                _spawnPlan.Add(new SpawnRuntime(spamFallback));

                var bruteFallback = new WaveSpawnDescriptor();
                bruteFallback.Configure("BruteWorm", 3, 1.2f, 2f);
                _spawnPlan.Add(new SpawnRuntime(bruteFallback));
                return;
            }

            foreach (var descriptor in levelConfig.WaveConfig.Spawns)
            {
                if (descriptor == null || descriptor.Count <= 0)
                {
                    continue;
                }

                _spawnPlan.Add(new SpawnRuntime(descriptor));
            }
        }

        public void StartWaves()
        {
            _isRunning = true;
            IsCompleted = _spawnPlan.Count == 0;
            if (IsCompleted)
            {
                WaveCompleted?.Invoke();
            }
        }

        public void Tick(float deltaTime)
        {
            if (!_isRunning || IsCompleted)
            {
                return;
            }

            if (_spawnPlan.Count == 0)
            {
                CompleteWave();
                return;
            }

            var hasAnyPending = false;
            for (var i = 0; i < _spawnPlan.Count; i++)
            {
                var runtime = _spawnPlan[i];
                if (runtime.IsCompleted)
                {
                    continue;
                }

                hasAnyPending = true;

                if (!runtime.IsStarted)
                {
                    runtime.StartAfterSeconds -= deltaTime;
                    continue;
                }

                runtime.Cooldown -= deltaTime;
                if (runtime.Cooldown > 0f)
                {
                    continue;
                }

                SpawnRequested?.Invoke(runtime.Descriptor);
                runtime.Remaining--;
                runtime.Cooldown = Math.Max(0.05f, runtime.Descriptor.IntervalSeconds);
            }

            if (!hasAnyPending)
            {
                CompleteWave();
            }
        }

        public void Reset()
        {
            IsCompleted = false;
            _isRunning = false;
            _spawnPlan.Clear();
        }

        private void CompleteWave()
        {
            IsCompleted = true;
            _isRunning = false;
            WaveCompleted?.Invoke();
        }
    }
}
