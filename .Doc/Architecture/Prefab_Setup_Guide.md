# Prefab Setup Guide

## Где используется
- Основной рантайм: `MiniGameBootstrap`.
- Точки привязки:
  - `RobotViewBindings` (точка выстрела `LaserMuzzle`)
  - `EnemyViewBindings` (точка попадания `HitPoint`)
  - `LaserViewPresenter` (body/emitter/impact визуал луча)

## 1) Префаб робота
Минимум:
1. Root `GameObject` робота.
2. Компонент `RobotViewBindings` на root (или дочернем объекте).
3. Дочерний `Transform` для `LaserMuzzle` (позиция выхода луча).

Опционально:
- `SpriteRenderer` / `Animator` для состояний.
- Child-иерархия для VFX.

Назначение:
- В `LevelConfig -> RobotVisual -> RobotPrefab`.

## 2) Префаб врага
Минимум:
1. Root `GameObject` врага.
2. Компонент `EnemyViewBindings`.
3. Дочерний `Transform` для `HitPoint` (куда визуально бьёт луч).

Опционально:
- `Animator`, VFX, hit flash, custom colliders.

Назначение:
- В `EnemyArchetypeConfig -> ViewPrefab`.

## 3) Префаб лазера
Минимум:
1. Root `GameObject`.
2. Компонент `LaserViewPresenter`.

Опционально (если не добавлены вручную, создаются автоматически):
- `SpriteRenderer` для `Emitter`.
- `SpriteRenderer` для `Body` (рекомендуется sprite с `Sliced`).
- `SpriteRenderer` для `Impact`.

Назначение:
- В `MiniGameBootstrap -> Laser View Prefab`.

## 4) Обязательные проверки
1. Луч стартует из `LaserMuzzle` робота.
2. Луч заканчивается в `HitPoint` врага.
3. При отсутствии любого префаба игра не падает (fallback активен).
