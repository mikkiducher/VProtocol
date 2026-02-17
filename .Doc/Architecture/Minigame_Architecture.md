# Архитектура миниигры (черновой baseline)

## Принципы
- Максимальная модульность: каждая подсистема имеет один публичный вход и минимум внешних зависимостей.
- Data-driven: поведение уровня и контента задается конфигами.
- Подготовка к встраиванию: мини-игра должна подключаться сценой или prefab-контейнером.

## Предлагаемые модули
- `GameFlow`: состояния матча (`Init`, `Playing`, `Win`, `Lose`), управление стартом/завершением.
- `WaveSystem`: расписание спавна, состав волны, прогресс волны.
- `EnemySystem`: движение справа налево, HP, достижение робота, пробитие барьера.
- `MathSystem`: генерация примеров, проверка ответа, генерация дистракторов.
- `CombatSystem`: расчет урона, комбо-множитель, событие выстрела робота.
- `BarrierSystem`: слои защиты робота и правила поражения.
- `UISystem`: показ примера и 3 ответов, обработка touch/click, фидбек correct/wrong.
- `VFXSystem`: лазерный выстрел и визуальный отклик попадания.
- `PresentationSystem` (planned): визуальный слой сущностей (sprite/animator bindings), фон уровня, UI skin/theme.

## Базовые контракты (реализованы в Foundation)
- `IGameFlow`: управление жизненным циклом матча и сменой состояний.
- `IWaveSystem`: инициализация волны из `LevelConfig`, запуск и сигнал завершения.
- `IEnemySystem`: регистрация архетипов врагов из конфигов.
- `IMathSystem`: генерация и проверка математических вопросов.
- `ICombatSystem`: расчет урона на основе вопроса и ответа.
- `IBarrierSystem`: управление слоями защиты робота.
- `IUISystem`: отображение вопроса и фидбека по ответу.

## Изоляция конфигов
Выделить отдельный namespace для внешних конфигов:
- `VProtocol.MiniGame.Config.Levels`
- `VProtocol.MiniGame.Config.Waves`
- `VProtocol.MiniGame.Config.Enemies`
- `VProtocol.MiniGame.Config.Math`
- `VProtocol.MiniGame.Config.Samples`
- `VProtocol.MiniGame.Config.Presentation` (planned)
- `VProtocol.MiniGame.Config.UI` (planned)

### План по арту (новое)
- Для врагов и робота вводится конфиг-слой визуала: какие спрайты/аниматоры использовать для каждого `EnemyVariant` и для robot states.
- Для уровня вводится theme-config: фон(ы), цветовая палитра, UI sprite set, опциональные VFX пресеты.
- Логика gameplay не должна знать о конкретных ассетах: только ID/ключи и presentation adapters.

Внутренние технические параметры (например, длительность локальной анимации UI) можно оставлять внутри модулей, если они не нужны другим системам.

## Интеграционный контракт (цель)
- Вход: `StartLevel(LevelConfig config)` (реализовано в `MiniGameBootstrap`).
- Выход: `MiniGameResult` (`Win/Lose`, время, точность, max combo, средний response) через `GameCompleted` event.
- Никакой жесткой привязки к глобальным singleton основного проекта.

## Текущий runtime baseline
- `MiniGameBootstrap` выступает composition root и связывает системы через контракты.
- UI baseline реализован через `OnGUI` для быстрой проверяемости в Editor и touch-эмуляции.
- Combat baseline включает positive-only multiplier, fast-answer combo streak и dynamic difficulty tier для следующего вопроса.
