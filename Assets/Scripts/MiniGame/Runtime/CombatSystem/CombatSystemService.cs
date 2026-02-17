using System;
using VProtocol.MiniGame.Runtime.Contracts;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.CombatSystem
{
    public sealed class CombatSystemService : ICombatSystem
    {
        public CombatResult ComputeDamage(MathQuestion question, int answer, float responseTimeSeconds, int comboStreak, int difficultyTier)
        {
            if (answer != question.CorrectAnswer)
            {
                return new CombatResult(0, 1f, 0);
            }

            var baseDamage = Math.Abs(question.CorrectAnswer);
            if (baseDamage < 1)
            {
                baseDamage = 1;
            }

            var normalizedResponse = responseTimeSeconds < 0f ? 0f : responseTimeSeconds;
            var comboTier = comboStreak / 2;
            var speedBonus = normalizedResponse <= 1.6f ? 0.2f : 0f;
            var multiplier = 1f + (comboTier * 0.2f) + (difficultyTier * 0.1f) + speedBonus;

            var complexityFloor = ResolveComplexityFloor(question, difficultyTier);
            var rawDamage = (int)Math.Round(baseDamage * multiplier, MidpointRounding.AwayFromZero);
            var finalDamage = Math.Max(rawDamage, complexityFloor);
            return new CombatResult(finalDamage, multiplier, complexityFloor);
        }

        public void Reset()
        {
            // Combo model is implemented in Task #3.
        }

        private static int ResolveComplexityFloor(MathQuestion question, int difficultyTier)
        {
            var operatorWeight = question.Operation switch
            {
                '*' => 2,
                '/' => 3,
                '-' => 1,
                _ => 0
            };

            var tierComponent = difficultyTier + 1;
            return tierComponent + operatorWeight;
        }
    }
}
