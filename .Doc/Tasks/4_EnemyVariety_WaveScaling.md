# Task 4: Контентный рост: вариативность врагов и волн

## Goal
Добавить вариативность давления на игрока через типы вирусов и масштабирование волны.

## Context
Разные профили врагов меняют темп игры и стимулируют использование комбо.

## Key Steps
1. Ввести минимум 2 типа врагов (`SpamBot`, `BruteWorm`) через конфиги.
2. Сделать wave-конфиг с составом (тип, количество, интервал спавна).
3. Реализовать рост сложности от уровня к уровню за счет состава и плотности волны.
4. Добавить задел под будущие спец-поведения (`Phisher`) без внедрения в MVP-логику.
5. Протестировать уровень "быстрые слабые" и "медленные толстые" как разные сценарии.

## Blockers & Risks
- Усложнение конфигов до появления реальной пользы.
- Зависимость логики врагов от конкретного UI.

## Acceptance Criteria
- Волна полностью задается конфигом.
- Новые типы врагов добавляются без правки core loop кода.

## Notes
После завершения выполнить Refactor Gate (Task 6).

## Implementation Notes (current)
- Added explicit enemy variants:
  - `SpamBot`: fast/light.
  - `BruteWorm`: slow/heavy (higher HP, larger visual scale).
  - `Phisher`: reserved special pattern with sine-wave movement.
- Enemy runtime now applies per-variant visuals and movement pattern, isolated inside `EnemySystem`.
- Fallback wave now includes mixed composition (`SpamBot` + `BruteWorm`) to validate pressure variance without extra asset setup.
- Wave composition remains config-driven via `WaveSpawnDescriptor` list.
- Wave scheduler updated to support parallel spawn groups by `startAfterSeconds`:
  - each descriptor can start independently from level start time,
  - groups can overlap in time.

## Unity Verification Steps
1. Open `Assets/Scenes/SampleScene.unity`, Play.
2. Observe fallback composition if `LevelConfig` is not assigned:
   - first wave part: many small cyan `SpamBot`.
   - second wave part: larger orange `BruteWorm`.
3. Confirm behavioral contrast:
   - `SpamBot` moves faster and dies quicker.
   - `BruteWorm` moves slower and requires more correct hits.
4. (Optional) Create custom `EnemyArchetypeConfig` assets and assign into `LevelConfig`:
   - set `Variant` and verify runtime uses it without code changes.
5. Ensure win still triggers only after wave completion and enemy cleanup.

## Wave Config Tips
- `enemyId` must match `EnemyArchetypeConfig.Id` (case-insensitive).
- Use `startAfterSeconds` for overlapping groups.
  Example:
  - `BruteWorm`, `count=3`, `interval=10`, `startAfterSeconds=0`
  - `SpamBot`, `count=12`, `interval=1`, `startAfterSeconds=2`
  Result: Brute starts immediately, Spam starts at 2s in parallel.
