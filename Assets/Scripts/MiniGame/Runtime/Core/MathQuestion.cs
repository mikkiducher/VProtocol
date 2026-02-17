using System;

namespace VProtocol.MiniGame.Runtime.Core
{
    public readonly struct MathQuestion
    {
        public MathQuestion(int leftOperand, int rightOperand, char operation, int correctAnswer)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operation = operation;
            CorrectAnswer = correctAnswer;
        }

        public int LeftOperand { get; }
        public int RightOperand { get; }
        public char Operation { get; }
        public int CorrectAnswer { get; }
    }

    public readonly struct MathRound
    {
        public MathRound(MathQuestion question, int[] options)
        {
            Question = question;
            Options = options ?? Array.Empty<int>();
        }

        public MathQuestion Question { get; }
        public int[] Options { get; }
    }

    public readonly struct CombatResult
    {
        public CombatResult(int damage, float multiplier, int complexityFloorDamage)
        {
            Damage = damage;
            Multiplier = multiplier;
            ComplexityFloorDamage = complexityFloorDamage;
        }

        public int Damage { get; }
        public float Multiplier { get; }
        public int ComplexityFloorDamage { get; }
    }
}
