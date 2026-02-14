### Builder Pattern Refactor: Abstract Block Classes & Terminal-Free DSL

#### Current Design & Rationale
The refactor aims to transform the LunyScript fluent API into a high-performance, beginner-friendly, and terminal-free DSL.

**Key Design Pillars:**
1.  **Zero-Allocation Fluent Chain**: Builders (`ObjectBuilder<T>`, etc.) are `readonly struct`s that maintain state without heap allocations.
2.  **Compile-Time State Safety**: Marker interfaces (`IStart`, `IStateNameSet`) on the generic builders restrict valid transitions (e.g., preventing `.AsCube().AsSphere()`).
3.  **Terminal-Free DSL**: Avoiding the need for `.Do()` by using **Implicit Conversions**. Since C# does not allow implicit conversion to interfaces, we are moving to a hierarchy of **Abstract Base Classes** (`ScriptActionBlock`, `ScriptConditionBlock`, etc.).
4.  **Auto-Finalization**: Builders register a `Finalizer` action on a `BuilderToken`. If a builder is used standalone (e.g., `Coroutine("Pulse").OnUpdate(...)`) and not passed to a consumer, `Script.Shutdown()` executes any "dangling" tokens to ensure the block is registered.

---

#### Status Quo
The refactor is currently in **Phase 1: Foundation (Abstract Block Classes)**.
- `IScriptBlock.cs` has been updated with the base class hierarchy.
- Several core blocks (`ObjectCreate`, `DebugLog`, `Sequence`, `For`, `If`) have been migrated to inherit from the base classes.
- `OnApi` has been updated to accept `ScriptActionBlock[]` instead of `IScriptActionBlock[]`.

**Current Build State:** **FAILING** (27 errors).
The project is in a partial migration state where API signatures expect classes, but many block factories and existing scripts still provide interfaces.

---

#### Perceived Problems & Blockers

1.  **Interface-to-Class Conversion Ripple**: 
    - Changing `OnApi.Ready(params ScriptActionBlock[] blocks)` causes immediate failures in every script that passes a block still returning `IScriptActionBlock`.
    - Because `IScriptActionBlock` does not implicitly convert to `ScriptActionBlock` (even if the implementation does), we must migrate **all** block-returning methods (factories) and signatures simultaneously, or provide temporary shims.

2.  **Variable System Complexity**:
    - `VariableBlock` inherits from `IScriptVariableBlock` and `IScriptConditionBlock`. 
    - Migrating it to inherit from `ScriptVariableBlock` (which now inherits from `ScriptConditionBlock`) caused "member hiding" warnings and inheritance conflicts.
    - Specifically, `VariableBlock.GetValue` needs to be `override abstract` to satisfy the new base class, and all its arithmetic operators and methods (`Set`, `Inc`, etc.) must return `ScriptActionBlock` or `ScriptVariableBlock`.

3.  **Static Factory Return Types**:
    - Many blocks use `public static IScriptActionBlock Create(...)`. These must all be changed to `public static ScriptActionBlock Create(...)`.
    - Until this is done project-wide, calling `On.Ready(Log.Info("..."))` fails because `Log.Info` returns the interface.

4.  **`params` Array Type Mismatch**:
    - `Script.If(params IScriptConditionBlock[] conditions)` passes an interface array to `IfBlockBuilder`, which now expects `params ScriptConditionBlock[]`. 
    - Arrays of interfaces do not convert to arrays of classes, requiring explicit casting or signature updates in `Script.cs`.

5.  **Scope of Work**:
    - The migration touches every "consumer" (APIs like `On`, `When`, `If`, `While`) and every "producer" (the dozens of block types in `Blocks/`). 
    - Reverting to a "Minimal Adapter" approach was considered to reduce risk, but the current decision is to stick with the "Abstract Base Class" design as it is architecturally superior.

---

#### Continuing the Refactor (Next Steps)

1.  **Complete Block Migration**: Focus on `LunyScript/Blocks/` subfolders (Engine, Editor, Scene, Run) to ensure all `Create()` methods return concrete `ScriptActionBlock` types.
2.  **Fix Variable System**: Resolve `VariableBlock.cs` and `ArithmeticVariableBlocks.cs` by ensuring they properly override the new abstract members and return class types.
3.  **Update Script.cs**: Align `If`, `While`, `AND`, `OR`, `NOT` signatures to use the abstract base classes.
4.  **Test Adaption**: Drop `.Do()` calls in `ObjectCreateTests.cs` and other test-drive sites once implicit conversion is live in `ObjectBuilder`.
