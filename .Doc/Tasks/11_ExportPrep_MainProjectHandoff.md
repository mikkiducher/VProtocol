# Task 11: Подготовка к экспорту и handoff в основной проект

## Goal
Подготовить mini-game модуль к переносу в основной проект и зафиксировать пакет передачи для задач `#8-#10`.

## Context
Оставшиеся задачи должны завершаться в host-проекте, где есть целевые UI/HUD, контент и пайплайн префабов.

## Key Steps
1. Зафиксировать границы экспорта:
   - runtime-скрипты mini-game,
   - конфиги и ScriptableObject-контракты,
   - необходимые prefab/binding компоненты.
2. Подготовить handoff по незавершенным задачам:
   - `#8` Theme/UI skin,
   - `#9` prefab-driven finalization,
   - `#10` combo progress bar final UX.
3. Проверить, что в docs есть критерии приемки и Unity verification steps для host-команды.
4. Зафиксировать список рисков интеграции и fallback-поведение.
5. Подготовить короткий чеклист приемки после импорта в основной проект.

## Blockers & Risks
- Отличия host UI/HUD и структуры префабов от sandbox-сцены.
- Возможные зависимости на scene-specific объекты.

## Export Handoff Checklist
1. Импортированы runtime-скрипты mini-game без ошибок компиляции.
2. Перенесены и назначены обязательные config assets.
3. Подключены prefab bindings (`RobotViewBindings`, `EnemyViewBindings`, laser presenter путь).
4. Проверен fallback при отсутствии theme/prefab config.
5. Подтверждено, что core loop работает без regressions.
6. Открыты задачи `#8-#10` в основном проекте с ссылкой на этот пакет.

## Acceptance Criteria
- В `.Doc/Issues.md` отражено, что `#8-#10` выполняются в основном проекте.
- Есть отдельная активная задача на экспорт-подготовку и handoff.
- Host-команда получает достаточный набор критериев для продолжения работ без контекстных пробелов.

## Implementation Notes (current, 2026-02-18)
- Подготовлен export manifest:
  - `.Doc/Export/2026-02-18_MiniGame_ExportManifest.md`
- Зафиксирован обязательный состав переноса:
  - `Assets/Scripts/MiniGame/**`,
  - `Assets/Configs/*` (уровень, математика, волны, archetypes),
  - `Assets/Prefabs/{Robot,Enemy,Laser}.prefab`,
  - `Assets/art/Core/*` (используется лазером в сцене),
  - `Assets/InputSystem_Actions.inputactions`.
- Зафиксированы runtime-зависимости и post-import acceptance checklist для host-команды.

## Next Step
1. Выполнить фактический перенос в основной проект и пройти чеклист из manifest.
2. После успешного импорта обновить статусы `#11` -> `Done`, а `#8-#10` открыть в трекере основного проекта.
