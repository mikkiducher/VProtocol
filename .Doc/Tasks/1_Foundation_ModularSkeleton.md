# Task 1: Foundation и каркас модульности

## Goal
Создать базовый каркас миниигры с четкими границами модулей и конфигов.

## Context
Без строгого каркаса проект быстро вырастет в связанную "лапшу", особенно с будущими механиками оружия и меты.

## Анализ текущего состояния (2026-02-17)
- В проекте отсутствуют runtime-скрипты и `asmdef` (чистая стартовая точка).
- Сцена `Assets/Scenes/SampleScene.unity` содержит только `Main Camera`.
- Есть `InputSystem_Actions.inputactions`, что позволяет сразу строить touch-first ввод без Legacy Input.
- Риск: при быстром старте core-loop легко смешать UI, математику и боевую логику в одном MonoBehaviour.

## План реализации (шаги Task #1)
1. Создать модульную структуру каталогов в `Assets/Scripts/MiniGame/`.
2. Ввести базовые контракты модулей (`IGameFlow`, `IWaveSystem`, `IEnemySystem`, `IMathSystem`, `ICombatSystem`, `IBarrierSystem`, `IUISystem`).
3. Создать конфиг-пространства `VProtocol.MiniGame.Config.*` и базовые DTO/ScriptableObject для уровня и волны.
4. Реализовать `MiniGameBootstrap` как единую точку сборки зависимостей (без singleton).
5. Добавить минимальную модель состояния матча (`Init/Playing/Win/Lose`) без геймплейной логики Task #2.
6. Проверить, что сцена запускается с bootstrap и не требует внешних менеджеров.
7. Обновить архитектурный документ при изменении контрактов.

## Key Steps
1. Создать базовую структуру папок для модулей (`GameFlow`, `EnemySystem`, `MathSystem`, `UISystem`, `CombatSystem`, `WaveSystem`, `BarrierSystem`).
2. Определить интерфейсы взаимодействия между модулями (events/contracts), без прямых ссылок "все на все".
3. Создать namespace-слой конфигов `VProtocol.MiniGame.Config.*`.
4. Ввести `LevelConfig` (границы чисел, операции, слои барьеров, параметры волны).
5. Подготовить bootstrap/sceneflow для запуска миниигры.
6. Зафиксировать документ решений по архитектурным границам.

## Blockers & Risks
- Риск завязки UI и логики математики в один слой.
- Риск ранних shortcut-решений через singleton.

## Acceptance Criteria
- Модули отделены по зонам ответственности.
- Конфиги вынесены в отдельные namespaces.
- Запуск сцены не требует глобальных зависимостей проекта-хоста.

## Notes
- Implemented:
  - Added modular runtime structure under `Assets/Scripts/MiniGame/Runtime/*`.
  - Added contracts for all baseline systems (`IGameFlow`, `IWaveSystem`, `IEnemySystem`, `IMathSystem`, `ICombatSystem`, `IBarrierSystem`, `IUISystem`).
  - Added config namespaces and assets under `Assets/Scripts/MiniGame/Config/*`.
  - Added `MiniGameBootstrap` as scene-level composition root (no singleton dependency).
- Refactor Gate (Task 6) check:
  - No direct `UI -> Enemy internals` coupling.
  - Cross-system dependencies are contract-first.
  - No critical architectural blockers detected for Task #2.
- Validation note:
  - Unity Editor runtime validation is pending (not executable from current CLI session).
