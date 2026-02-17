# Task 2: Core Loop (одна волна, 3 ответа, touch/click)

## Goal
Собрать минимально играбельный цикл: враг идет, игрок отвечает, робот атакует, возможны win/lose.

## Context
Нужен рабочий baseline, который можно проверять руками и масштабировать по контенту.

## Key Steps
1. Реализовать спавн врагов справа и движение влево к роботу.
2. Добавить пробитие одного барьера при достижении робота.
3. Реализовать генерацию примера и 3 вариантов ответа (1 правильный).
4. Подключить touch-first ввод с fallback на click в Editor.
5. На правильный ответ: выстрел робота + урон врагу.
6. На ошибку: снять 1 слой барьера, показать правильный ответ кратко, скрыть неверные варианты.
7. Добавить завершение уровня по одной волне (`Win`) и по потере всех барьеров (`Lose`).

## Blockers & Risks
- Некорректная синхронизация UI ответа и боевой логики.
- Слабая читаемость фидбека correct/wrong для ребенка.

## Acceptance Criteria
- Игрок может пройти или проиграть волну без ручных костылей.
- Все действия выполняются одним тапом/кликом без кнопки подтверждения.
- Поведение стабильно в Editor и touch-сценарии.

## Notes
После завершения выполнить Refactor Gate (Task 6).

## Implementation Notes (current)
- Implemented in code:
  - Wave spawn scheduler with timed spawn events.
  - Enemy runtime with movement right-to-left and barrier hit on robot line.
  - Math round generation (`question + 3 options`) with one-tap answer acceptance.
  - UI layer via `OnGUI` (`touch/click` friendly) with feedback for correct/wrong answers.
  - Win condition: wave completed and no alive enemies.
  - Lose condition: barrier layers reach zero.
  - Basic laser shot VFX (`LineRenderer`) from robot marker to front enemy.
- Build check:
  - `dotnet build Assembly-CSharp.csproj` -> success, 0 errors.

## Unity Verification Steps
1. Open `Assets/Scenes/SampleScene.unity`.
2. Select `Main Camera` and add component `MiniGameBootstrap`.
3. Press Play.
4. Verify in Game view:
   - Left panel shows `State: Playing` and `Barriers`.
   - Enemies spawn on the right and move left to robot marker.
   - Clicking/tapping an answer immediately resolves (no confirm button).
   - Correct answer shows `Correct` feedback and damages/kills enemies.
   - Wrong answer removes one barrier layer.
   - Enemy reaching robot also removes one barrier layer.
   - Game ends with `Win` after wave clear or `Lose` after all barriers are spent.
