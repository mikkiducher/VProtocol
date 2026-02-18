namespace VProtocol.MiniGame.Runtime.Core
{
    public enum GameState
    {
        Init = 0,
        Playing = 1,
        Win = 2,
        Lose = 3
    }

    public enum RobotVisualState
    {
        Idle = 0,
        Shoot = 1,
        Hit = 2,
        Break = 3
    }

    public readonly struct MiniGameResult
    {
        public MiniGameResult(
            bool isWin,
            float durationSeconds,
            int correctAnswers,
            int totalAnswers,
            int maxComboStreak,
            float averageResponseSeconds)
        {
            IsWin = isWin;
            DurationSeconds = durationSeconds;
            CorrectAnswers = correctAnswers;
            TotalAnswers = totalAnswers;
            MaxComboStreak = maxComboStreak;
            AverageResponseSeconds = averageResponseSeconds;
        }

        public bool IsWin { get; }
        public float DurationSeconds { get; }
        public int CorrectAnswers { get; }
        public int TotalAnswers { get; }
        public int MaxComboStreak { get; }
        public float AverageResponseSeconds { get; }
        public float Accuracy => TotalAnswers > 0 ? (float)CorrectAnswers / TotalAnswers : 0f;
    }

    public readonly struct MiniGameRuntimeStats
    {
        public MiniGameRuntimeStats(
            GameState gameState,
            int barriers,
            int comboStreak,
            int fastStreak,
            int difficultyTier,
            float multiplier,
            float comboTimeRemainingSeconds,
            float comboWindowSeconds,
            float comboReady01,
            int correctAnswers,
            int totalAnswers,
            float averageResponseSeconds)
        {
            GameState = gameState;
            Barriers = barriers;
            ComboStreak = comboStreak;
            FastStreak = fastStreak;
            DifficultyTier = difficultyTier;
            Multiplier = multiplier;
            ComboTimeRemainingSeconds = comboTimeRemainingSeconds;
            ComboWindowSeconds = comboWindowSeconds;
            ComboReady01 = comboReady01;
            CorrectAnswers = correctAnswers;
            TotalAnswers = totalAnswers;
            AverageResponseSeconds = averageResponseSeconds;
        }

        public GameState GameState { get; }
        public int Barriers { get; }
        public int ComboStreak { get; }
        public int FastStreak { get; }
        public int DifficultyTier { get; }
        public float Multiplier { get; }
        public float ComboTimeRemainingSeconds { get; }
        public float ComboWindowSeconds { get; }
        public float ComboReady01 { get; }
        public int CorrectAnswers { get; }
        public int TotalAnswers { get; }
        public float AverageResponseSeconds { get; }
        public float Accuracy => TotalAnswers > 0 ? (float)CorrectAnswers / TotalAnswers : 0f;
    }
}
