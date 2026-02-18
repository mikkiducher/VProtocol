# Task 8: Темизация уровня и UI-скин

## Goal
Добавить управляемую смену фона уровня и визуального оформления интерфейса (по уровню/теме).

## Context
Нужна масштабируемая подача контента: разные уровни должны отличаться визуально без форков сцен.

## Key Steps
1. Ввести `LevelThemeConfig` в `VProtocol.MiniGame.Config.Presentation`:
   - background sprite/prefab,
   - palette,
   - optional VFX style preset.
2. Ввести `UISkinConfig` в `VProtocol.MiniGame.Config.UI`:
   - panel/background sprites,
   - button sprites (normal/pressed),
   - font/style references.
3. В `MiniGameBootstrap` добавить связывание темы с уровнем (`LevelConfig -> ThemeConfig`).
4. Вынести текущий `OnGUI`-style UI к адаптеру skin-конфига (минимум цвета/иконки/фон панели).
5. Поддержать hot-swap темы в Editor для быстрой проверки (context menu).
6. Сохранить fallback на базовый стиль, если конфиги не назначены.

## Blockers & Risks
- Привязка к одной сцене и ручной расстановке background.
- Появление дублирующих UI-предустановок без стандарта.

## Acceptance Criteria
- Фон и UI-оформление меняются через конфиги уровня.
- Один и тот же runtime-код работает для разных тем.
- Нет поломки core loop при пустых визуальных конфигах.

## Unity Verification Steps
1. Создать 2 theme-конфига (например `TronBlue`, `RedAlert`) с разными background sprites и UI style.
2. Запустить сцену с первым конфигом и проверить фон/панель/кнопки.
3. Переключить на второй конфиг и повторить запуск.
4. Убедиться, что gameplay-поведение не изменилось, меняется только presentation.

## Handoff в основной проект (2026-02-18)
- Статус в этом репозитории: `Pending`.
- Решение: реализацию завершать в основном проекте, где уже есть целевой UI/HUD и темы окружения.
- Что переносим:
  - требования `LevelThemeConfig`/`UISkinConfig`,
  - правила fallback при пустых конфигах,
  - критерии приемки и шаги верификации.
