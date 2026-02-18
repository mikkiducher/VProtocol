using System;
using UnityEngine;
using VProtocol.MiniGame.Runtime.Contracts;
using VProtocol.MiniGame.Runtime.Core;

namespace VProtocol.MiniGame.Runtime.UISystem
{
    public sealed class UISystemService : MonoBehaviour, IUISystem
    {
        private MathRound _round;
        private bool _hasRound;
        private string _feedbackText = string.Empty;
        private float _feedbackHideTime;
        private GameState _state = GameState.Init;
        private int _barriers;
        private int _comboStreak;
        private int _fastStreak;
        private int _difficultyTier;
        private float _multiplier = 1f;
        private float _comboReady01;
        private float _avgResponse;
        private float _accuracy;
        private bool _overlayEnabled = true;

        public bool IsOverlayEnabled => _overlayEnabled;
        public event Action<int> AnswerSelected;

        public void Initialize()
        {
            _feedbackText = string.Empty;
            _feedbackHideTime = 0f;
            _hasRound = false;
        }

        public void ShowQuestion(MathRound round)
        {
            _round = round;
            _hasRound = true;
            _feedbackText = string.Empty;
        }

        public void ShowCorrectFeedback(int correctAnswer)
        {
            _feedbackText = $"Correct: {correctAnswer}";
            _feedbackHideTime = Time.time + 0.8f;
        }

        public void ShowWrongFeedback(int correctAnswer)
        {
            _feedbackText = $"Wrong. Correct: {correctAnswer}";
            _feedbackHideTime = Time.time + 1.2f;
        }

        public void SetGameState(GameState state, int barrierLayers)
        {
            _state = state;
            _barriers = barrierLayers;
        }

        public void SetCombatTelemetry(int comboStreak, int fastStreak, int difficultyTier, float multiplier, float comboReady01, float avgResponseSeconds, float accuracy)
        {
            _comboStreak = comboStreak;
            _fastStreak = fastStreak;
            _difficultyTier = difficultyTier;
            _multiplier = multiplier;
            _comboReady01 = Mathf.Clamp01(comboReady01);
            _avgResponse = avgResponseSeconds;
            _accuracy = accuracy;
        }

        public void Reset()
        {
            _hasRound = false;
            _feedbackText = string.Empty;
            _feedbackHideTime = 0f;
            _overlayEnabled = true;
        }

        public void ToggleOverlay()
        {
            _overlayEnabled = !_overlayEnabled;
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(18f, 18f, 86f, 24f), _overlayEnabled ? "HUD Off" : "HUD On"))
            {
                ToggleOverlay();
            }

            if (!_overlayEnabled)
            {
                return;
            }

            var panelRect = new Rect(18f, 18f, 320f, 320f);
            GUI.Box(panelRect, "Math Defense");

            GUI.Label(new Rect(30f, 48f, 250f, 24f), $"State: {_state}");
            GUI.Label(new Rect(30f, 72f, 250f, 24f), $"Barriers: {_barriers}");
            GUI.Label(new Rect(30f, 96f, 280f, 24f), $"Combo: {_comboStreak}  Fast: {_fastStreak}  Tier: {_difficultyTier}");
            GUI.Label(new Rect(30f, 120f, 280f, 24f), $"x{_multiplier:0.00}  Acc: {_accuracy * 100f:0}%  Avg: {_avgResponse:0.00}s");
            DrawComboBar(new Rect(30f, 144f, 230f, 16f), _comboReady01);

            if (!string.IsNullOrEmpty(_feedbackText) && Time.time <= _feedbackHideTime)
            {
                GUI.Label(new Rect(30f, 164f, 250f, 24f), _feedbackText);
            }

            if (_state != GameState.Playing || !_hasRound)
            {
                return;
            }

            var question = _round.Question;
            GUI.Label(
                new Rect(30f, 194f, 250f, 24f),
                $"{question.LeftOperand} {question.Operation} {question.RightOperand} = ?");

            var options = _round.Options;
            for (var i = 0; i < options.Length; i++)
            {
                var y = 224f + (i * 34f);
                if (GUI.Button(new Rect(30f, y, 230f, 30f), options[i].ToString()))
                {
                    AnswerSelected?.Invoke(options[i]);
                }
            }
        }

        private static void DrawComboBar(Rect rect, float ready01)
        {
            GUI.Box(rect, GUIContent.none);
            if (ready01 <= 0f)
            {
                return;
            }

            var fillWidth = Mathf.Max(1f, rect.width * ready01);
            var fillRect = new Rect(rect.x + 1f, rect.y + 1f, fillWidth - 2f, rect.height - 2f);
            var previousColor = GUI.color;
            GUI.color = ready01 > 0.66f
                ? new Color(0.2f, 0.95f, 0.2f, 1f)
                : ready01 > 0.33f
                    ? new Color(0.95f, 0.85f, 0.2f, 1f)
                    : new Color(0.95f, 0.25f, 0.2f, 1f);
            GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
            GUI.color = previousColor;
        }
    }
}
