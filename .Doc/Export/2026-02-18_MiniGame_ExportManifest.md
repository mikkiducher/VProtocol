# MiniGame Export Manifest (2026-02-18)

## Цель
Передать mini-game модуль в основной проект с минимальными рисками потери ссылок и без регрессий core loop.

## Базовые требования окружения
- Unity: `6000.2.6f2`
- Пакеты (минимум): `com.unity.inputsystem`, `com.unity.ugui`, `com.unity.textmeshpro`

## Обязательный состав экспорта
1. Код mini-game:
   - `Assets/Scripts/MiniGame/**` (включая `.meta`)
2. Конфиги:
   - `Assets/Configs/LevelConfig.asset`
   - `Assets/Configs/MathConfig.asset`
   - `Assets/Configs/WaveConfig.asset`
   - `Assets/Configs/Enemies/SpamArchetypeConfig.asset`
   - `Assets/Configs/Enemies/BruteWormArchetypeConfig.asset`
3. Префабы presentation:
   - `Assets/Prefabs/Robot.prefab`
   - `Assets/Prefabs/Enemy.prefab`
   - `Assets/Prefabs/Laser.prefab`
4. Графические ассеты лазера из сцены:
   - `Assets/art/Core/shadow_square.png`
   - `Assets/art/Core/ui_circle_128.png`
   - `Assets/art/Core/ui_glow_256.png`
5. Input actions:
   - `Assets/InputSystem_Actions.inputactions`

Важно: переносить вместе с `.meta`, иначе сломаются GUID-ссылки в `LevelConfig` и префабах.

## Runtime-зависимости, которые должны остаться валидными
- `SampleScene`/bootstrap ссылается на:
  - `LevelConfig.asset`
  - `Laser.prefab`
  - laser sprites из `Assets/art/Core/*`
- `LevelConfig.asset` ссылается на:
  - `MathConfig.asset`
  - `WaveConfig.asset`
  - `SpamArchetypeConfig.asset`
  - `BruteWormArchetypeConfig.asset`
  - `Robot.prefab`
- Enemy archetypes ссылаются на:
  - `Enemy.prefab`

## Что можно не переносить в host-проект
- `Assets/Scenes/SampleScene.unity` (если интеграция идет в существующую host-сцену)
- `Assets/Tests/**` (в текущем репозитории тестов mini-game не найдено)

## Handoff по незавершенным задачам
1. `#8` Theme/UI skin:
   - реализовать `LevelThemeConfig` и `UISkinConfig` в host UI-пайплайне.
2. `#9` Prefab-driven presentation:
   - финализировать host-иерархию префабов и marker points.
3. `#10` Combo progress bar:
   - довести визуал в едином HUD host-проекта, сохранить текущую runtime-логику decay/reset.

## Чеклист приемки после импорта
1. Проект собирается без compile errors.
2. `MiniGameBootstrap` запускается с host-конфигом уровня.
3. Враги спавнятся и получают урон.
4. Луч стартует из `LaserMuzzle` (если задан) и целится в `HitPoint` (если задан).
5. При отсутствии префабов/маркеров отрабатывает fallback-поведение.
6. Комбо-окно (`ComboReady01`) обновляется и корректно сбрасывается по timeout/wrong answer.
