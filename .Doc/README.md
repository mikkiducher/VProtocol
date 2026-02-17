# Документация mini-game: быстрый гайд

Этот файл нужен как короткая точка входа для актуализации контекста перед работой.

## Что читать в каком порядке
1. Общий статус и очередь задач: `.Doc/Issues.md`
2. Видение и продуктовые ограничения: `.Doc/VProtocol_Guide.md`
3. Архитектурные границы модулей: `.Doc/Architecture/Minigame_Architecture.md`
4. Контракт встраивания: `.Doc/Architecture/Integration_Guide.md`
5. Настройка префабов: `.Doc/Architecture/Prefab_Setup_Guide.md`
6. Детальный план текущей задачи: `.Doc/Tasks/<N>_<TaskName>.md`

## Быстрый рабочий цикл (перед началом любой задачи)
1. Открыть `.Doc/Issues.md` и найти задачу со статусом `[→] In Progress`.
2. Прочитать соответствующий файл из `.Doc/Tasks/`.
3. Сверить ограничения с `.Doc/VProtocol_Guide.md`.
4. Проверить архитектурные контракты в `.Doc/Architecture/Minigame_Architecture.md`.
5. Только после этого вносить изменения в код.

## Как обновлять документы по ходу работы
- Начало задачи: сменить статус в `.Doc/Issues.md` на `[→] In Progress`.
- При блокере: заполнить `Blockers` в `Issues.md` и task-файле, задачу не закрывать.
- Завершение задачи: поставить `[✓] Done` в `.Doc/Issues.md` и обновить заметки в task-файле.
- После каждого крупного блока: выполнить проверку модульности по `.Doc/Tasks/6_RefactorGate_PerBlock.md`.

## Где лежит декомпозиция
- Реестр: `.Doc/Issues.md`
- Детальные планы:
  - `.Doc/Tasks/1_Foundation_ModularSkeleton.md`
  - `.Doc/Tasks/2_CoreLoop_VerticalSlice.md`
  - `.Doc/Tasks/3_Damage_Combo_Balance.md`
  - `.Doc/Tasks/4_EnemyVariety_WaveScaling.md`
  - `.Doc/Tasks/5_Integration_ExportReadiness.md`
  - `.Doc/Tasks/6_RefactorGate_PerBlock.md`
  - `.Doc/Tasks/7_EntityArtPipeline.md`
  - `.Doc/Tasks/8_LevelTheme_AndUISkin.md`
  - `.Doc/Tasks/9_PrefabDrivenPresentation.md`
  - `.Doc/Tasks/10_ComboTimer_ProgressBar.md`
