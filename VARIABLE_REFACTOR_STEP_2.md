### Step 2 Complete: .Set() Bug Fix + Abstract Base Class Migration

---

#### Changes Made

##### Step B — Fixed `.Set()` No-Op Bug
- **`VariableArithmeticTests.cs` lines 91-92**: Changed standalone `v_upd_inc.Set(0)` / `v_upd_tog.Set(false)` (no-ops that created discarded blocks) to `GVar["v_upd_inc"] = 0` / `GVar["v_upd_tog"] = false` (immediate Build-time assignment via VarAccessor indexer).

##### VarAccessor Fix — Changed from `readonly struct` to `sealed class`
- **Problem**: `GVar["x"] = 0` didn't compile because `GVar` property returned a temporary struct — C# won't allow indexer setter on a temporary value.
- **Fix**: Changed `VarAccessor` from `readonly struct` to `sealed class`. Instances are cached as fields in `Script` (initialized in `Initialize()`). Build-time allocation is acceptable per guidelines.
- **Files**: `LunyScript/VarAccessor.cs`, `LunyScript/Script.cs`

##### Primitive-to-VariableBlock Implicit Conversions
- Added `implicit operator VariableBlock` for `Int32`, `Int64`, `Single`, `Double`, `Boolean`, `String` to `VariableBlock.cs`.
- **Reason**: C# doesn't chain two implicit conversions (`int → Variable → VariableBlock`). Direct conversions enable `GVar["x"] = 0` without explicit casts.

##### Step C — All Concrete Blocks Migrated to Abstract Base Classes

| Block | Now Extends | Previously Implemented |
|---|---|---|
| `DebugBreakBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `DebugLogBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `EditorPausePlayerBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `EngineLogBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `SceneReloadBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `RunActionBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `ObjectCreateBlock` (abstract) | `ScriptActionBlock` | `IScriptActionBlock` |
| `ObjectDestroySelfBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `ObjectDestroyTargetBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `ObjectDisableSelfBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `ObjectDisableTargetBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `ObjectEnableSelfBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `ObjectEnableTargetBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `CoroutineControlBlock` (abstract) | `ScriptActionBlock` | `IScriptActionBlock` |
| `CoroutineBlock` | `ScriptCoroutineBlock` | `IScriptCoroutineBlock` |
| `SequenceBlock` | `ScriptSequenceBlock` | `IScriptSequenceBlock` |
| `ForBlockBuilder` | `ScriptActionBlock` | `IScriptActionBlock` |
| `ForBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `IfBlockBuilder` | `ScriptActionBlock` | `IScriptActionBlock` |
| `IfBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `WhileBlockBuilder` | `ScriptActionBlock` | `IScriptActionBlock` |
| `WhileBlock` | `ScriptActionBlock` | `IScriptActionBlock` |
| `AssignmentVariableBlock` | `ScriptActionBlock` + `IScriptVariableBlock` | `IScriptActionBlock` + `IScriptVariableBlock` |
| `CheckConditionBlock` | `ScriptConditionBlock` | `IScriptConditionBlock` |
| `VariableBlock` (abstract) | `ScriptVariableBlock` | `IScriptVariableBlock` + `IScriptConditionBlock` |

##### Return Type Updates — Interfaces → Abstract Classes
- All `Create()` static factory methods now return abstract class types (`ScriptActionBlock`, `ScriptConditionBlock`, `ScriptCoroutineBlock`, `ScriptSequenceBlock`) instead of interface types.
- API methods (`DebugApi`, `EditorApi`, `EngineApi`, `MethodApi`, `ObjectApi`, `SceneApi`, `PrefabApi`) return `ScriptActionBlock`/`ScriptConditionBlock`.
- `VariableBlock` action methods (`.Set()`, `.Add()`, `.Inc()`, etc.) return `ScriptActionBlock`.
- `IScriptCoroutineBlock` interface methods (`Start`, `Stop`, `Pause`, `Resume`) return `ScriptActionBlock`.
- `IScriptTimerCoroutineBlock.TimeScale` returns `ScriptActionBlock`.
- **Exception**: `OnApi` and `WhenApi` still return `IScriptSequenceBlock` because `ScriptEventScheduler` stores sequences as `IScriptSequenceBlock` internally.

---

#### Build & Test Status
- **LunyScript.csproj**: ✅ 0 errors, 0 warnings
- **LunyScript-Test**: ✅ 125/125 tests passed
- **Luny-Test**: ✅ 83/83 tests passed

---

#### Final Type Hierarchy

```
ScriptBlock (abstract, implements IScriptBlock)
├── ScriptActionBlock (abstract, implements IScriptActionBlock)
│   ├── ScriptSequenceBlock (abstract, implements IScriptSequenceBlock)
│   │   └── SequenceBlock
│   ├── ScriptCoroutineBlock (abstract, implements IScriptCoroutineBlock)
│   │   └── CoroutineBlock → TimerCoroutineBlock, CounterCoroutineBlock
│   ├── CoroutineControlBlock (abstract) → Start/Stop/Pause/Resume/TimeScale blocks
│   ├── ObjectCreateBlock (abstract) → Empty/Cube/Sphere/... blocks
│   ├── DebugLogBlock → Info/Warning/Error blocks
│   ├── DebugBreakBlock, EditorPausePlayerBlock, EngineLogBlock, SceneReloadBlock
│   ├── RunActionBlock
│   ├── AssignmentVariableBlock (also implements IScriptVariableBlock)
│   ├── ForBlockBuilder, ForBlock, IfBlockBuilder, IfBlock, WhileBlockBuilder, WhileBlock
│   └── ObjectDestroy/Enable/Disable blocks
├── ScriptConditionBlock (abstract, implements IScriptConditionBlock)
│   ├── CheckConditionBlock
│   └── ScriptVariableBlock (abstract, implements IScriptVariableBlock)
│       └── VariableBlock (abstract, operators + actions)
│           ├── TableVariableBlock, ConstantVariableBlock, LoopCounterVariableBlock
│           ├── ArithmeticVariableBlock → Add/Sub/Mul/Div blocks
│           ├── ComparisonVariableBlock → IsEqualTo/IsNotEqualTo/IsGreaterThan/... blocks
│           └── AndBlock, OrBlock, NotBlock
```

---

#### What Remains
- The interfaces (`IScriptActionBlock`, `IScriptConditionBlock`, etc.) are still in `IScriptBlock.cs` and still used by some internal infrastructure (scheduler, runtime context). They can be gradually phased out if desired.
- `BUILDER_PATTERN_REFACTOR.md` documents the broader builder pattern refactor (terminal-free DSL with implicit conversions) which builds on this foundation.
