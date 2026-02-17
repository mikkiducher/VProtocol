# Task 5: Интеграция и упаковка

## Goal
Подготовить миниигру к безопасному переносу в основной проект.

## Context
Формат поставки: сначала отдельная сцена, затем возможность перетащить в prefab-контейнер.

## Key Steps
1. Выделить публичную точку входа (`StartLevel(LevelConfig)`).
2. Определить модель результата (`MiniGameResult`).
3. Убрать проект-специфичные зависимости и проверить автономный запуск сцены.
4. Сформировать минимальный integration guide (что подключить, что вызвать, что получить).
5. Проверить, что конфиги и ассеты не завязаны на абсолютные пути и внешние сцены.

## Blockers & Risks
- Скрытые зависимости на глобальные менеджеры.
- Сложный порядок инициализации при переносе.

## Acceptance Criteria
- Сцена запускается автономно и возвращает результат через контракт.
- Перенос в host-проект не требует переписывания core-кода.

## Notes
После завершения выполнить Refactor Gate (Task 6).

## Implementation Notes (current)
- `MiniGameBootstrap` now provides runtime integration contract:
  - `StartLevel(LevelConfig config)`
  - `StopSession()`
  - `GameCompleted` event with `MiniGameResult`.
- Added context menu helpers for Unity runtime verification:
  - `Start Assigned Level`
  - `Stop Session`
- Added result model `MiniGameResult` with core KPI fields.
- Added integration doc:
  - `.Doc/Architecture/Integration_Guide.md`

## Unity Verification Steps
1. Open `Assets/Scenes/SampleScene.unity`.
2. On `MiniGameBootstrap`, disable `Auto Start On Awake`.
3. Enter Play Mode and trigger `Start Assigned Level` from component context menu.
4. Finish a round (`Win` or `Lose`) and verify `Console` contains:
   - `MiniGameResult => win=... duration=... accuracy=... maxCombo=...`
5. Trigger `Stop Session` and verify gameplay loop stops.
