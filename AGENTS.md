# Repository Guidelines

## Project Structure & Module Organization
This repository is a Unity 6 project (`ProjectVersion.txt`: `6000.2.6f2`).
- `Assets/`: game content and scripts. Current playable scene is `Assets/Scenes/SampleScene.unity`.
- `Assets/InputSystem_Actions.inputactions`: Input System action map referenced by build settings.
- `Packages/manifest.json`: Unity package dependencies (includes `com.unity.test-framework`).
- `ProjectSettings/`: editor, build, physics, and platform settings.
- Generated folders (`Library/`, `Temp/`, `Logs/`, `UserSettings/`) should not be edited manually.
- Planned game direction: embed-ready 2D math Tower Defence mini-game.

## Build, Test, and Development Commands
Use Unity Editor for day-to-day iteration, or run in batch mode for automation.
- Open locally: Unity Hub -> Add/open this folder.
- Build player (example):
  `Unity.exe -batchmode -quit -projectPath . -buildWindows64Player Build/VProtocol.exe -logFile Logs/build.log`
- Run EditMode tests:
  `Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode-tests.xml -logFile Logs/tests-editmode.log`
- Run PlayMode tests:
  `Unity.exe -batchmode -quit -projectPath . -runTests -testPlatform PlayMode -testResults Logs/playmode-tests.xml -logFile Logs/tests-playmode.log`
- Optional local compile check:
  `dotnet build Assembly-CSharp.csproj`

## Coding Style & Naming Conventions
- Language: C# (Unity).
- Indentation: 4 spaces; UTF-8 text files.
- Naming: `PascalCase` for classes/methods/properties, `camelCase` for locals/parameters, `_camelCase` for private serialized fields.
- Keep one `MonoBehaviour` per file; file name should match class name.
- Organize scripts by feature under `Assets/` (for example, `Assets/Scripts/Gameplay/`).
- Code content rule: English only in code files (`.cs`, comments, hardcoded strings, logs, exceptions).

## Architecture Rules (Mini-Game)
- Build systems as independent modules with clear boundaries (no direct cross-feature coupling).
- Keep gameplay config data decoupled from logic. Use dedicated config namespaces, for example:
  `VProtocol.MiniGame.Config.Levels`, `VProtocol.MiniGame.Config.Waves`, `VProtocol.MiniGame.Config.Enemies`, `VProtocol.MiniGame.Config.Samples`.
- Keep tunables (sizes, animation timings, spawn intervals) internal to module unless shared externally.
- Prefer data-driven setup (`ScriptableObject` or config DTOs) to simplify export and integration.
- Export target is code/package transfer into a larger host project; avoid hard dependencies on project-global singletons.

## Testing Guidelines
- Framework: Unity Test Framework (`com.unity.test-framework`).
- Place tests in `Assets/Tests/EditMode/` and `Assets/Tests/PlayMode/`.
- Name test files as `<Feature>NameTests.cs`; use clear test names like `Move_WhenInputForward_UpdatesPosition`.
- Add tests for new gameplay logic and bug fixes before merging.
- Prioritize tests for wave sequencing, enemy math rules, and deterministic tower behavior.

## Task Tracking & Documentation Workflow
- Use two-level tracking:
  `/.Doc/Issues.md` (registry + statuses) and `/.Doc/Tasks/<N>_<TaskName>.md` (detailed plan).
- Status markers: `[ ] Pending`, `[→] In Progress`, `[✓] Done`.
- Before planning, read relevant existing code first; do not plan from docs only.
- If blocked, update blockers in task docs and keep task open.
- For backlog planning, move item from backlog list to a numbered task document.

## Commit & Pull Request Guidelines
- Recommended commit format: `type(scope): short summary` (for example, `feat(input): add jump action binding`).
- Create commits only when explicitly requested by the maintainer.
- PRs should include: summary, test evidence (Editor/CLI logs), linked issue/task, and screenshots/video for scene or UI changes.
- Do not delete tracked files without explicit confirmation.

## Agent Collaboration Rules
- Chat with maintainer: Russian.
- Documentation text (`.md`): Russian preferred; code snippets remain English.
