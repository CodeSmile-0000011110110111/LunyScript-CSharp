### Variable System Refactor — Status & Decisions

---

### Current State (as of 2026-02-14)

The variable refactor is **in progress**. LunyScript.csproj builds cleanly but test projects have **114 compile errors** due to `GVar("name")` → `GVar["name"]` migration (mechanical text replacement needed).

---

### Completed Changes

#### 1. Removed unnamed `Define(Variable value)` (was `Const(Variable)`)
- **Rationale:** Literals are our constants; named `Define` makes sense for debugging views. Unnamed has no remaining use case.
- **Files changed:** `Script.cs`, `IScript` interface — removed the `Define(Variable value)` overload.
- Only `Define(String name, Variable value)` remains (returns `VariableBlock` via `TableVariableBlock.Create`).

#### 2. Removed all `Is*()` comparison methods from `VariableBlock`
- **Removed methods:** `IsEqualTo`, `IsNotEqualTo`, `IsGreaterThan`, `IsLessThan`, `IsAtLeast`, `IsAtMost`, `IsTrue`, `IsFalse`
- **Rationale:** These were exact duplicates of operator overloads (`==`, `!=`, `>`, `<`, `>=`, `<=`, implicit bool, `!`). Both produced the same deferred blocks. Operators are more beginner-friendly and less verbose.
- **Important:** Operators ARE deferred (they create block trees, not immediate comparisons) because `VariableBlock` overloads them to return `VariableBlock`, not `Boolean`. So there is no loss of deferred behavior.
- **File changed:** `VariableBlock.cs` — removed lines 136–155 (the Conditions section).

#### 3. Updated `VariableComparisonTests.cs`
- Replaced all `Is*()` method calls with operator equivalents.
- Removed the "Method variants" assertion section (m_eq, m_neq, etc.).
- Kept "Variable-to-variable operators" and "Variable-to-literal operators" sections.

#### 4. Updated `VariableArithmeticTests.cs`
- Changed unnamed `Define(3)` etc. to named `Define("three", 3)`, `Define("four", 4)`, `Define("five", 5)`, `Define("ten", 10)`.

#### 5. Added `implicit operator VariableBlock(Variable v)` to `VariableBlock`
- **Location:** `VariableBlock.cs` line 11
- **Implementation:** `public static implicit operator VariableBlock(Variable value) => ConstantVariableBlock.Create(value);`
- **Purpose:** Enables the `VarAccessor` indexer setter to accept `Variable` literals (since C# indexer get/set must share the same type `VariableBlock`). Also useful elsewhere — allows `VariableBlock x = 5` via the chain `Int32 → Variable → VariableBlock`.

#### 6. Created `VarAccessor` readonly struct
- **File:** `LunyScript/VarAccessor.cs` (new file)
- **Design:** Single type for both `GVar` and `Var` — wraps an `ITable`.
- **Getter:** `this[String name]` returns `TableVariableBlock.Create(_table.GetHandle(name))` — a `VariableBlock` for use in expressions/conditions/actions.
- **Setter:** `this[String name] = value` performs immediate assignment via `_table.GetHandle(name).Value = value.GetValue(null)` — Build-time only.
- **Key safety:** `On.Heartbeat(GVar["Steps"] = true)` will NOT compile because the assignment result is `VariableBlock` (not `IScriptActionBlock`). This enforces the distinction: indexer = immediate (Build-time), `.Set()` = deferred (runtime block). ✅

#### 7. Changed `Script.GVar`/`Script.Var` from methods to properties
- **Before:** `public VariableBlock GVar(String name)` / `public VariableBlock Var(String name)`
- **After:** `public VarAccessor GVar => new(_runtimeContext.GlobalVariables)` / `public VarAccessor Var => new(_runtimeContext.LocalVariables)`
- **IScript interface updated:** `VarAccessor Var { get; }` / `VarAccessor GVar { get; }`

---

### Remaining Steps (In Order)

#### Step A — Text Replace: `GVar("name")` → `GVar["name"]` (114 occurrences in tests)
- **Regex:** `GVar\("([^"]+)"\)` → `GVar["$1"]`
- **Scope:** `LunyScript-Test/` folder (all .cs files)
- No `Var("name")` usages exist in test files currently (0 occurrences found).
- Also check LunyScript source files for any `GVar("` or `Var("` usage patterns.
- After replacement: build and run all 125 tests.

#### Step B — Standalone `.Set()` Bug (known latent issue)
In `VariableArithmeticTests.cs` lines 91-92:
```csharp
v_upd_inc.Set(0);      // Creates AssignmentVariableBlock, then DISCARDS it!
v_upd_tog.Set(false);  // Same — this block is never executed!
```
These "work" by coincidence (null Variable → 0/false). Now that `VarAccessor` exists, these should become:
```csharp
GVar["v_upd_inc"] = 0;
GVar["v_upd_tog"] = false;
```

#### Step C — Continue Abstract Base Class Migration (from prior plan)
Once variables are stable, proceed with migrating concrete blocks to abstract bases:
1. **Step 3** — Migrate concrete blocks to extend abstract bases (internal blocks first, public types last)
   - 3a. Simple action blocks → extend `ScriptActionBlock`
   - 3b. Abstract intermediates (`ObjectCreateBlock`, `CoroutineControlBlock`) → extend `ScriptActionBlock`
   - 3c. `SequenceBlock` → extend `ScriptSequenceBlock`
   - 3d. `CoroutineBlock` → extend `ScriptCoroutineBlock`
   - 3e. `VariableBlock` → extend `ScriptVariableBlock` (removes duplicate Evaluate/GetValue)
   - 3f. Builders (`ForBlockBuilder`, `IfBlockBuilder`, `WhileBlockBuilder`) → extend `ScriptActionBlock`
   - 3g. `AssignmentVariableBlock` → extend `ScriptActionBlock` (keeps separate `IScriptVariableBlock` impl)
2. **Step 4** — Update `Create()` return types from interfaces to abstract classes
3. **Step 5** — Update consumer API signatures (`OnApi`, `WhenApi`, `Script.If/While/AND/OR/NOT`, etc.)

---

### Key Design Decisions

#### Arithmetic methods `.Add()`, `.Sub()`, `.Mul()`, `.Div()`, `.Inc()`, `.Dec()`, `.Toggle()` — KEPT
These are **NOT** duplicates of operators. They are compound-assignment actions (like `+=`):
- `v.Add(5)` → ACTION block: `v.Set(v + 5)` — modifies v in-place (deferred)
- `v + 5` → EXPRESSION block: returns new `ArithmeticVariableBlock` — pure, read-only
Usage is extensive across the codebase (coroutine tests, flow tests, activation tests).

#### `.Set()` — KEPT (for deferred assignment)
C# assignment (`=`) cannot be overloaded, so `.Set()` must exist for creating deferred action blocks:
```csharp
On.Heartbeat(GVar["Steps"].Set(true));  // deferred — creates block executed at runtime
```

#### `VarAccessor` — Single type for both GVar and Var
`VarHandle._owner` tracks which table it belongs to, so no need for separate `GVarAccessor`/`VarAccessor` types.

#### `VariableBlock` operator overloads are ALL deferred
Both `==` operator and the removed `.IsEqualTo()` method produced identical `IsEqualToVariableBlock` instances. The operator does NOT perform immediate comparison during Build() — it creates a block tree evaluated at runtime.

#### Known gotcha: `var result = hp == 0; if (result) { .. }`
`result` is `VariableBlock` (reference type), not `Boolean`. `if (result)` is a null-check — always true. Cannot be prevented without `implicit operator Boolean` which would cause worse ambiguity. Removing `Is*()` methods actually helps because IDE shows the result type.

#### Abstract base class hierarchy (already implemented in `IScriptBlock.cs`)
```
ScriptBlock (abstract, implements IScriptBlock)
├── ScriptActionBlock (abstract, implements IScriptActionBlock)
│   ├── ScriptSequenceBlock (abstract, implements IScriptSequenceBlock)
│   └── ScriptCoroutineBlock (abstract, implements IScriptCoroutineBlock)
├── ScriptConditionBlock (abstract, implements IScriptConditionBlock)
│   └── ScriptVariableBlock (abstract, implements IScriptVariableBlock + IScriptConditionBlock)
```
`ScriptVariableBlock.Evaluate()` defaults to `GetValue(runtimeContext).AsBoolean()` — matches existing `VariableBlock` behavior.

#### `AssignmentVariableBlock` implements BOTH `IScriptActionBlock` and `IScriptVariableBlock`
During base class migration: should extend `ScriptActionBlock` (primary use is as action from `.Set()`), and separately implement `IScriptVariableBlock`.

#### `AndBlock`, `OrBlock`, `NotBlock` extend `VariableBlock` — KEEP as-is
They ARE variable blocks because they produce `Variable` values and participate in operator chains (e.g., `Set(!this)` in `Toggle()`).

---

### File Inventory (changed/created)

| File | Status |
|---|---|
| `LunyScript/Script.cs` | Modified — removed unnamed Define, GVar/Var → VarAccessor properties |
| `LunyScript/Blocks/Variables/VariableBlock.cs` | Modified — removed Is*() methods, added implicit operator |
| `LunyScript/Blocks/IScriptBlock.cs` | Modified (prior session) — added abstract base classes |
| `LunyScript/VarAccessor.cs` | **New** — VarAccessor readonly struct |
| `LunyScript-Test/Variables/VariableComparisonTests.cs` | Modified — operators only |
| `LunyScript-Test/Variables/VariableArithmeticTests.cs` | Modified — named Define() |

### Build Status
- `LunyScript.csproj`: ✅ Builds with 0 errors, 0 warnings
- `LunyScript-Test.csproj`: ❌ 114 errors — all `CS1955: Non-invocable member 'Script.GVar' cannot be used like a method` — needs text replacement `GVar("name")` → `GVar["name"]`
