using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.Contracts
{
    public interface ICombatSystem
    {
        CombatResult ComputeDamage(MathQuestion question, int answer, float responseTimeSeconds, int comboStreak, int difficultyTier);
        void Reset();
    }
}
