# Integration Guide (scene-first, prefab-ready)

## Runtime API
Основная точка интеграции: `MiniGameBootstrap`.

Публичные методы/события:
- `StartLevel(LevelConfig config)` — запуск миниигры с внешним конфигом.
- `StopSession()` — остановка текущей сессии.
- `GameCompleted` (`Action<MiniGameResult>`) — результат завершенной сессии.

`MiniGameResult` содержит:
- `IsWin`
- `DurationSeconds`
- `CorrectAnswers`
- `TotalAnswers`
- `MaxComboStreak`
- `AverageResponseSeconds`
- `Accuracy` (computed)

## Host-side usage example
```csharp
using UnityEngine;
using VProtocol.MiniGame.Config.Levels;
using VProtocol.MiniGame.Runtime.Bootstrap;

public sealed class MiniGameHostExample : MonoBehaviour
{
    [SerializeField] private MiniGameBootstrap miniGame;
    [SerializeField] private LevelConfig levelConfig;

    private void Start()
    {
        miniGame.GameCompleted += OnMiniGameCompleted;
        miniGame.StartLevel(levelConfig);
    }

    private void OnDestroy()
    {
        miniGame.GameCompleted -= OnMiniGameCompleted;
    }

    private void OnMiniGameCompleted(VProtocol.MiniGame.Runtime.Core.MiniGameResult result)
    {
        Debug.Log($"MiniGame finished. Win={result.IsWin}, Accuracy={result.Accuracy:0.00}");
    }
}
```

## Unity smoke-check for integration
1. На `Main Camera` с `MiniGameBootstrap` отключить `Auto Start On Awake`.
2. В Play Mode открыть context menu компонента и нажать `Start Assigned Level`.
3. Доиграть раунд и проверить `Console`:
   - лог `MiniGameResult => ...`
4. Проверить `Stop Session` через context menu — спавн/движение врагов останавливаются.
