# Task 9: Полный переход на prefab-driven presentation

## Goal
Перевести визуальный слой на префабы: враги, робот, лазер и UI-контейнеры с точками привязки и анимациями.

## Context
Спрайтовых полей недостаточно для production-контроля. Нужны префабы для масштабов, анимационных блоков, pivot/anchors и marker points.

## Key Steps
1. Ввести `EnemyViewPrefab` contract:
   - root prefab,
   - optional marker for hit point / aim point.
2. Ввести `RobotViewPrefab` contract:
   - root prefab,
   - child marker `LaserMuzzle`.
3. Ввести `LaserViewPrefab` contract:
   - emitter, body (sliced), impact как child-элементы,
   - API для установки start/end точек.
4. UI:
   - сделать отдельный debug HUD префабом, привязываемым через bootstrap.
5. В `MiniGameBootstrap` заменить прямое создание ad-hoc GO на фабрики/адаптеры префабов.
6. Сохранить fallback при отсутствии префаба.

## Blockers & Risks
- Разные prefab-структуры без стандарта именования точек.
- Случайные зависимости геймплея от конкретной иерархии.

## Acceptance Criteria
- Положение/размер/анимация сущностей и лазера контролируются через префабы.
- Смена префаба не требует изменения gameplay-скриптов.

## Unity Verification Steps
1. Назначить prefab для робота с `LaserMuzzle`.
2. Назначить prefab врага с hit marker.
3. Назначить prefab лазера.
4. Проверить, что луч стартует из muzzle и бьет в marker врага.

## Implementation Notes (current)
- Implemented prefab binding components:
  - `RobotViewBindings` with `LaserMuzzle` support.
  - `EnemyViewBindings` with optional `HitPoint`.
  - `LaserViewPresenter` for prefab-driven emitter/body/impact composition.
- `MiniGameBootstrap` now supports `Laser View Prefab`.
- Laser origin now uses robot muzzle when available.
- Enemy targeting now uses enemy hit marker when available.
- Fallback behavior preserved when prefabs/markers are missing.

## Unity Verification Steps (current practical)
1. Robot prefab:
   - add `RobotViewBindings` component,
   - assign child transform to `Laser Muzzle`.
2. Enemy prefab:
   - add `EnemyViewBindings`,
   - assign `Hit Point` child transform.
3. Laser prefab:
   - optional child renderers or empty root;
   - `LaserViewPresenter` can auto-create missing renderers.
4. Assign laser prefab into `MiniGameBootstrap -> Laser View Prefab`.
5. Run scene and verify:
   - beam starts from `LaserMuzzle`,
   - beam ends at enemy `HitPoint`.
