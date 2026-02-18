# Task 10: Комбо-таймер как прогресс бар и боевая готовность

## Goal
Добавить наглядный прогресс-бар окна комбо, показывающий время до распада страйка.

## Context
Игроку нужен визуальный таймер, чтобы понимать, успевает ли он поддержать боевую готовность оружия.

## Key Steps
1. Ввести `comboWindowSeconds` в боевые настройки.
2. Считать `comboTimeRemaining` в runtime (decay при отсутствии правильного ответа).
3. Добавить `ComboReady01` в runtime stats и UI telemetry.
4. Отрисовать прогресс-бар (debug HUD) с цветовой индикацией:
   - green (safe), yellow (warning), red (critical).
5. При успешном ответе обновлять/продлевать окно комбо по правилам.
6. Обновить документацию баланса и значения по умолчанию.

## Blockers & Risks
- Неконсистентность сброса комбо между wrong answer и timeout.
- Слишком короткое окно, создающее фрустрацию.

## Acceptance Criteria
- Игрок видит текущее окно комбо в виде прогресс-бара.
- Комбо предсказуемо распадается по времени.
- Статистика комбо-окна доступна извне через runtime API.

## Unity Verification Steps
1. Запустить сцену и набрать 3 быстрых правильных.
2. Наблюдать прогресс-бар: заполнен после ответа, убывает со временем.
3. Дождаться полного спада без ответа: комбо сбрасывается.
4. Проверить что wrong answer также мгновенно сбрасывает бар.

## Implementation Notes (current)
- Added level-driven combat timing config in `LevelConfig -> Combat`:
  - `FastAnswerThresholdSeconds`
  - `ComboWindowSeconds`
- Runtime now tracks `comboTimeRemainingSeconds` and resets combo on timeout.
- Added runtime stats fields:
  - `ComboTimeRemainingSeconds`
  - `ComboWindowSeconds`
  - `ComboReady01`
- HUD now renders combo progress bar with color zones:
  - green / yellow / red.

## Handoff в основной проект (2026-02-18)
- Статус в этом репозитории: `Pending` (базовая реализация есть, финализация перенесена).
- Решение: довести UI/UX прогресс-бара в основном проекте, где используется единый HUD.
- Что переносим:
  - runtime-поля `ComboTimeRemainingSeconds`, `ComboWindowSeconds`, `ComboReady01`,
  - правила decay/reset (timeout и wrong answer),
  - требования к цветовым зонам и проверке поведения.
