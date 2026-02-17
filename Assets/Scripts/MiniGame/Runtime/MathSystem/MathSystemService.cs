using System;
using System.Collections.Generic;
using System.Linq;
using VProtocol.MiniGame.Config.Math;
using VProtocol.MiniGame.Runtime.Contracts;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.MathSystem
{
    public sealed class MathSystemService : IMathSystem
    {
        private readonly Random _random = new();
        private MathConfig _config;

        public void Initialize(MathConfig mathConfig)
        {
            _config = mathConfig;
        }

        public MathRound GenerateRound(int difficultyTier)
        {
            if (_config == null)
            {
                var fallbackQuestion = new MathQuestion(1, 1, '+', 2);
                return new MathRound(fallbackQuestion, new[] { 2, 1, 3 });
            }

            var min = Math.Min(_config.MinOperandValue, _config.MaxOperandValue);
            var max = Math.Max(_config.MinOperandValue, _config.MaxOperandValue);
            var tier = difficultyTier < 0 ? 0 : difficultyTier;
            var tierRangeBoost = tier * 2;
            var dynamicMax = max + tierRangeBoost;

            var left = _random.Next(min, dynamicMax + 1);
            var right = _random.Next(min, dynamicMax + 1);
            var operation = PickOperation(_config.AllowedOperators);

            var correctAnswer = operation switch
            {
                '+' => left + right,
                '-' => left - right,
                '*' => left * right,
                '/' => right == 0 ? 0 : left / right,
                _ => left + right
            };

            if (operation == '/')
            {
                right = right == 0 ? 1 : right;
                left = right * Math.Max(1, _random.Next(min, dynamicMax + 1));
                correctAnswer = left / right;
            }

            var question = new MathQuestion(left, right, operation, correctAnswer);
            var options = BuildAnswerOptions(question, tier);
            return new MathRound(question, options);
        }

        public bool IsCorrect(MathRound round, int answer)
        {
            return round.Question.CorrectAnswer == answer;
        }

        public void Reset()
        {
            // Intentionally empty for foundation phase.
        }

        private char PickOperation(MathOperator allowedOperators)
        {
            var options = new System.Collections.Generic.List<char>(4);
            if ((allowedOperators & MathOperator.Add) != 0) options.Add('+');
            if ((allowedOperators & MathOperator.Subtract) != 0) options.Add('-');
            if ((allowedOperators & MathOperator.Multiply) != 0) options.Add('*');
            if ((allowedOperators & MathOperator.Divide) != 0) options.Add('/');

            if (options.Count == 0)
            {
                return '+';
            }

            return options[_random.Next(0, options.Count)];
        }

        private int[] BuildAnswerOptions(MathQuestion question, int difficultyTier)
        {
            var correctAnswer = question.CorrectAnswer;
            var candidates = new HashSet<int> { correctAnswer };
            var smallOffset = 1 + (difficultyTier / 2);
            candidates.Add(correctAnswer + smallOffset);
            candidates.Add(correctAnswer - smallOffset);

            // Common operator confusion distractor (e.g. 4-3 vs 4+3).
            if (question.Operation == '-')
            {
                candidates.Add(question.LeftOperand + question.RightOperand);
            }
            else if (question.Operation == '+')
            {
                candidates.Add(question.LeftOperand - question.RightOperand);
            }
            else if (question.Operation == '*')
            {
                candidates.Add(question.LeftOperand + question.RightOperand);
            }
            else if (question.Operation == '/')
            {
                candidates.Add(question.LeftOperand + question.RightOperand);
            }

            if (candidates.Count < 3)
            {
                candidates.Add(correctAnswer + smallOffset + 1);
            }

            var list = candidates.Take(3).ToList();
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = _random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list.ToArray();
        }
    }
}
