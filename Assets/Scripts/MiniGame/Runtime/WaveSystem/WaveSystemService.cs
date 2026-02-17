using System;
using System.Collections.Generic;
using VProtocol.MiniGame.Config.Levels;
using VProtocol.MiniGame.Config.Waves;
using VProtocol.MiniGame.Runtime.Contracts;

namespace VProtocol.MiniGame.Runtime.WaveSystem
{
    public sealed class WaveSystemService : IWaveSystem
    {
        private readonly List<WaveSpawnDescriptor> _spawnPlan = new();
        private int _descriptorIndex;
        private int _remainingInDescriptor;
        private float _spawnCooldown;
        private bool _isRunning;

        public bool IsCompleted { get; private set; }

        public event Action<WaveSpawnDescriptor> SpawnRequested;
        public event Action WaveCompleted;

        public void Initialize(LevelConfig levelConfig)
        {
            _spawnPlan.Clear();
            IsCompleted = false;
            _descriptorIndex = 0;
            _remainingInDescriptor = 0;
            _spawnCooldown = 0f;
            _isRunning = false;

            if (levelConfig == null || levelConfig.WaveConfig == null)
            {
                var spamFallback = new WaveSpawnDescriptor();
                spamFallback.Configure("SpamBot", 10, 0.75f);
                _spawnPlan.Add(spamFallback);

                var bruteFallback = new WaveSpawnDescriptor();
                bruteFallback.Configure("BruteWorm", 3, 1.2f);
                _spawnPlan.Add(bruteFallback);
                return;
            }

            _spawnPlan.AddRange(levelConfig.WaveConfig.Spawns);
        }

        public void StartWaves()
        {
            _isRunning = true;
            IsCompleted = _spawnPlan.Count == 0;

            if (_spawnPlan.Count > 0)
            {
                _descriptorIndex = 0;
                _remainingInDescriptor = Math.Max(0, _spawnPlan[0].Count);
                _spawnCooldown = 0f;
            }
            else
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

            if (_descriptorIndex >= _spawnPlan.Count)
            {
                CompleteWave();
                return;
            }

            if (_remainingInDescriptor <= 0)
            {
                AdvanceDescriptor();
                return;
            }

            _spawnCooldown -= deltaTime;
            if (_spawnCooldown > 0f)
            {
                return;
            }

            var descriptor = _spawnPlan[_descriptorIndex];
            SpawnRequested?.Invoke(descriptor);
            _remainingInDescriptor--;
            _spawnCooldown = Math.Max(0.05f, descriptor.IntervalSeconds);
        }

        public void Reset()
        {
            IsCompleted = false;
            _isRunning = false;
            _descriptorIndex = 0;
            _remainingInDescriptor = 0;
            _spawnCooldown = 0f;
        }

        private void AdvanceDescriptor()
        {
            _descriptorIndex++;
            if (_descriptorIndex >= _spawnPlan.Count)
            {
                CompleteWave();
                return;
            }

            var next = _spawnPlan[_descriptorIndex];
            _remainingInDescriptor = Math.Max(0, next.Count);
            _spawnCooldown = 0f;
        }

        private void CompleteWave()
        {
            IsCompleted = true;
            _isRunning = false;
            WaveCompleted?.Invoke();
        }
    }
}
