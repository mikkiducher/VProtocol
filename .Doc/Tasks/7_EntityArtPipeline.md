# Task 7: Арт-пайплайн сущностей (робот/враги/анимации)

## Goal
Сделать контролируемое подключение арта для врагов и робота без правки gameplay-кода.

## Context
Сейчас визуал генерируется примитивами. Для production нужен data-driven слой: какие спрайты/анимации у какого типа сущности.

## Key Steps
1. Ввести `EntityVisualConfig` (ScriptableObject) в `VProtocol.MiniGame.Config.Presentation`.
2. Для каждого `EnemyVariant` задать:
   - `sprite`/`prefab`,
   - `animatorController` (опционально),
   - scale/offset и material preset.
3. Добавить `RobotVisualConfig` для robot states (`Idle`, `Shoot`, `Hit`, `Break`).
4. Выделить адаптер `EntityPresentationAdapter`, который применяет визуал по конфигу при spawn/событиях.
5. Переключить `EnemySystem` и robot marker с примитивов на конфигурируемый presentation слой.
6. Добавить fallback (если ассет не задан) — безопасный placeholder без креша.

## Blockers & Risks
- Смешивание визуальной логики с боевой.
- Жесткая зависимость на конкретные prefab-иерархии.

## Acceptance Criteria
- Замена арта врага/робота делается через конфиг, без изменения runtime-логики.
- При отсутствии ассетов игра остается рабочей за счет fallback.

## Unity Verification Steps
1. Создать `EntityVisualConfig` asset и назначить спрайты для `SpamBot` и `BruteWorm`.
2. Привязать конфиг в bootstrap.
3. Запустить сцену: убедиться, что отображаются назначенные спрайты, а не примитивы.
4. Сменить ассет в конфиге и повторно запустить: визуал меняется без правки скриптов.

## Implemented Update (current)
- `EnemyArchetypeConfig` now supports visual bindings per archetype:
  - `ViewPrefab`, `ViewSprite`, `ViewAnimatorController`, `VisualScale`, `VisualOffset`.
- `LevelConfig` now contains `RobotVisualProfile`:
  - `RobotPrefab`, state sprites (`Idle/Shoot/Hit/Break`), optional animator controller, scale.
- Enemy spawn now supports randomized Y offset via `MiniGameBootstrap.spawnYJitter`.
- Laser VFX now supports sprite-based rendering:
  - stretched sliced body sprite,
  - emitter sprite near robot,
  - impact sprite near enemy,
  - fallback to old line-renderer when sprites are not assigned.
- Debug UI control and stats access:
  - Added runtime button `HUD On/HUD Off` for overlay toggle.
  - Added `MiniGameBootstrap.GetRuntimeStats()` and `RuntimeStatsUpdated` event for host-side stats access without HUD.

## Unity Verification Steps (updated practical)
1. In `MiniGameBootstrap`, set `Spawn Y Jitter` to `1.0+` and run scene:
   - enemies should spawn with vertical variation, not in one line.
2. Assign `Laser Body Sprite` (sliced), `Laser Emitter Sprite`, `Laser Impact Sprite`.
3. Run scene and answer correctly:
   - laser should stretch from robot to enemy,
   - emitter visible at robot side,
   - impact visible on enemy side.
4. Clear sprite fields and rerun:
   - fallback line-renderer laser should still work.
