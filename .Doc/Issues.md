# Issues

Единый реестр задач mini-game.  
Статусы: `[ ] Pending`, `[→] In Progress`, `[✓] Done`.

## Активные задачи

## #1 Foundation и каркас модульности
- Status: [✓] Done
- Goal: Подготовить модульный каркас, базовые интерфейсы систем и конфиги уровня.
- Blockers: Нет (Unity runtime smoke-check pending in Editor)
- Task Doc: `.Doc/Tasks/1_Foundation_ModularSkeleton.md`

## #2 Core Loop (одна волна, 3 ответа, touch/click)
- Status: [✓] Done
- Goal: Реализовать playable vertical slice с одним типом врага и базовой математикой.
- Blockers: Нет
- Task Doc: `.Doc/Tasks/2_CoreLoop_VerticalSlice.md`

## #3 Боевая математика и комбо
- Status: [✓] Done
- Goal: Ввести расчет урона от ответа, тайминги ответа и положительный комбо-множитель.
- Blockers: Нет
- Task Doc: `.Doc/Tasks/3_Damage_Combo_Balance.md`

## #4 Контентный рост: вариативность врагов и волн
- Status: [✓] Done
- Goal: Добавить минимум 2 типа вирусов и расширяемую схему волн.
- Blockers: Нет
- Task Doc: `.Doc/Tasks/4_EnemyVariety_WaveScaling.md`

## #5 Интеграция и упаковка
- Status: [✓] Done
- Goal: Подготовить миниигру к переносу в основной проект (scene-first + prefab-ready).
- Blockers: Нет
- Task Doc: `.Doc/Tasks/5_Integration_ExportReadiness.md`

## #6 Рефакторинг-гейт после каждого блока
- Status: [✓] Done
- Goal: После каждой крупной задачи фиксировать технический долг и убирать связность.
- Blockers: Нет (for blocks #1-#5)
- Task Doc: `.Doc/Tasks/6_RefactorGate_PerBlock.md`

## #7 Арт-пайплайн сущностей (робот/враги/анимации)
- Status: [✓] Done
- Goal: Ввести управляемое подключение спрайтов и анимационных блоков для врагов и робота.
- Blockers: Нет
- Task Doc: `.Doc/Tasks/7_EntityArtPipeline.md`

## #8 Темизация уровня и UI-скин
- Status: [ ] Pending
- Goal: Добавить конфигурируемые фоны уровня и визуальные ассеты интерфейса.
- Blockers: Нет
- Task Doc: `.Doc/Tasks/8_LevelTheme_AndUISkin.md`

## #9 Полный переход на prefab-driven presentation
- Status: [→] In Progress
- Goal: Перевести врагов, робота, лазер и UI-блоки на префабную сборку с точками привязки и анимационными состояниями.
- Blockers: Нет
- Task Doc: `.Doc/Tasks/9_PrefabDrivenPresentation.md`

## #10 Комбо-таймер как прогресс бар и боевая готовность
- Status: [ ] Pending
- Goal: Визуализировать окно комбо через прогресс бар (ready/decay), чтобы игрок видел тайминг поддержания страйка.
- Blockers: Нет
- Task Doc: `.Doc/Tasks/10_ComboTimer_ProgressBar.md`

## ЗАДАЧИ НА БУДУЩЕЕ (BACKLOG)
- Полная свайп-механика выбора ответа с поэтапным уточнением (десятки -> точное значение).
- Система оружия (unlock/upgrade) с ростом сложности примеров.
- Многоуровневая кампания и мета-прогрессия.
- Особые поведения вирусов (разделение, ускорение, иммунные окна).
