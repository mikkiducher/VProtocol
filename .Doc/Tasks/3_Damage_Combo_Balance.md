# Task 3: Боевая математика и комбо

## Goal
Сделать урон от решения примеров понятным и добавить систему позитивного комбо-ускорения.

## Context
Игрок должен быть вознагражден за быстрые и точные ответы, без наказаний через отрицательные множители.

## Key Steps
1. Реализовать базовый урон от правильного ответа.
2. Ввести таймер реакции для каждого вопроса.
3. Реализовать комбо-логику: после N быстрых правильных ответов повышать сложность следующего примера.
4. Добавить положительный множитель урона при активном комбо.
5. Проверить баланс для "сложный пример -> маленький результат" (ввести корректирующий коэффициент при необходимости).
6. Логировать ключевые метрики (accuracy, combo streak, avg response time) для тестов.

## Blockers & Risks
- Дисбаланс при малых результатах примеров.
- Слишком резкий рост сложности при комбо.

## Acceptance Criteria
- Комбо ощущается как награда и ускоряет уничтожение врагов.
- Нет ситуаций, где верный сложный ответ ощущается "слабым" без компенсации.

## Notes
После завершения выполнить Refactor Gate (Task 6).

## Implementation Notes (current)
- Added response-time aware combat calculation with positive-only multiplier.
- Added fast-streak/combo rules:
  - fast answer threshold: `<= 2.0s`;
  - combo activates on `3+` fast correct answers in a row;
  - dynamic `difficultyTier` grows from combo streak and is used for next question generation.
- Added complexity floor damage to prevent weak damage on hard-but-small-result questions.
- Added runtime telemetry in UI:
  - combo streak, fast streak, difficulty tier, current multiplier, accuracy, avg response time.
- Added debug metric logs to Console for balancing sessions.

## Unity Verification Steps
1. Open `Assets/Scenes/SampleScene.unity`, run Play with `MiniGameBootstrap` on `Main Camera`.
2. Give 3 fast correct answers in a row (`< 2 sec` each):
   - expect combo > 0, difficulty tier > 0, multiplier > `x1.00`.
3. Continue fast correct answers:
   - expect harder generated questions (larger operand range).
4. Give one wrong answer:
   - expect combo/fast streak reset to 0 and multiplier back to `x1.00`.
5. Check Console logs:
   - `Combat: correct ... combo=... tier=... x... dmg=... floor=...`.
6. Verify difficult operations (`*` and `/`) with small result still produce meaningful damage (complexity floor).
