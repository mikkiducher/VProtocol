using UnityEngine;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Config.Math
{
    [CreateAssetMenu(
        fileName = "MathConfig",
        menuName = "VProtocol/MiniGame/Config/Math",
        order = 0)]
    public sealed class MathConfig : ScriptableObject
    {
        [SerializeField] private int minOperandValue = 1;
        [SerializeField] private int maxOperandValue = 10;
        [SerializeField] private MathOperator allowedOperators = MathOperator.Add | MathOperator.Subtract;

        public int MinOperandValue => minOperandValue;
        public int MaxOperandValue => maxOperandValue;
        public MathOperator AllowedOperators => allowedOperators;

        public void Configure(int minValue, int maxValue, MathOperator operatorsMask)
        {
            minOperandValue = minValue;
            maxOperandValue = maxValue < minValue ? minValue : maxValue;
            allowedOperators = operatorsMask == MathOperator.None ? MathOperator.Add : operatorsMask;
        }
    }
}
