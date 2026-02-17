using VProtocol.MiniGame.Config.Math;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.Contracts
{
    public interface IMathSystem
    {
        void Initialize(MathConfig mathConfig);
        MathRound GenerateRound(int difficultyTier);
        bool IsCorrect(MathRound round, int answer);
        void Reset();
    }
}
