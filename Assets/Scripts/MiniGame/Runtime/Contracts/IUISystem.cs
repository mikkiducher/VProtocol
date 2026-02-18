using System;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.Contracts
{
    public interface IUISystem
    {
        bool IsOverlayEnabled { get; }
        event Action<int> AnswerSelected;
        void Initialize();
        void ShowQuestion(MathRound round);
        void ShowCorrectFeedback(int correctAnswer);
        void ShowWrongFeedback(int correctAnswer);
        void SetGameState(GameState state, int barrierLayers);
        void SetCombatTelemetry(int comboStreak, int fastStreak, int difficultyTier, float multiplier, float comboReady01, float avgResponseSeconds, float accuracy);
        void ToggleOverlay();
        void Reset();
    }
}
