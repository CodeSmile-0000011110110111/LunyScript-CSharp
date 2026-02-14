### VARIABLE REFACTOR — Step 1: GVar/Var Indexer Migration

**Status:** READY FOR YOU (IDE refactoring)

---

### What Changed (already done by Junie)

1. `Script.GVar` and `Script.Var` changed from **methods** to **properties** returning `VarAccessor`
2. `VarAccessor` is a `readonly struct` with an indexer `this[String name]`
3. `IScript` interface updated: `VarAccessor Var { get; }` / `VarAccessor GVar { get; }`

**Result:** `GVar("name")` no longer compiles (CS1955: non-invocable member). Must become `GVar["name"]`.

---

### What You Need To Do

**Find & Replace (Regex) in `LunyScript-Test/` folder:**

| Search (Regex) | Replace |
|---|---|
| `GVar\("([^"]+)"\)` | `GVar["$1"]` |

**Scope:** `LunyScript-Test/` folder, all `.cs` files

**Count:** 114 occurrences across test files

**No `Var("...")` usages** exist in test files (0 occurrences found), so no replacement needed for `Var`.

Also check these **LunyScript source files** for any `GVar("` or `Var("` patterns (should be zero, but verify):
- `LunyScript/` folder (excluding test folder)

---

### After Replacement

Run:
```
dotnet build LunyScript-Test\LunyScript-Test.csproj
dotnet test LunyScript-Test\LunyScript-Test.csproj
```

Expected: 0 errors, 125 tests pass.

---

### Then Tell Junie to Continue

Next steps after this replacement:
- **Step B**: Fix standalone `.Set()` no-op bug (lines 91-92 in `VariableArithmeticTests.cs`)
- **Step C**: Continue abstract base class migration (steps 3–5 from original plan)

Tell Junie: "Step 1 done, continue with Step B"
