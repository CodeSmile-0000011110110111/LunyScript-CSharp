# LunyScript Next Steps

## Instructions

Read .claude/context.md for chat instructions/settings

## Current State (After Step 1)

### ✅ Completed Features
- **Core block system** with IBlock (leaf) and IRunnable (container) interfaces
- **RunContext** containing ScriptID, ScriptType, Object, Variables, GlobalVariables, InspectorVariables, DebugHooks
- **Block types:** LogMessageBlock, ActionBlock, RunnableSequence
- **LunyScript base class** with Build() method, Variables/GlobalVariables properties, Log/Do factory methods
- **Registration helpers:** OnUpdate(), OnFixedStep(), OnLateUpdate() - auto-wrap blocks in RunnableSequence
- **LunyScriptRunner** with full lifecycle execution (Update/FixedStep/LateUpdate) and debug hook placeholders
- **Global variables** shared across all scripts via Variables instance in runner
- **Deterministic execution** via sorted ObjectID iteration in RunContextRegistry
- **Scene preprocessing** binds scripts to objects by name matching (exact, case-sensitive)

### Key Architecture Decisions

1. **Stateful sequences** - Not stateless, blocks can have fields. Hot reload recreates entire RunContext.
2. **Context-owned runnables** - RunContext stores List<IRunnable> for each lifecycle phase, not script instances.
3. **Always wrap in sequence** - Single blocks wrapped in RunnableSequence (or other IRunnable) for consistency.
4. **IBlock vs IRunnable** - Leaf blocks (Log, Action) implement IBlock. Runnable containers (Sequence, FSM, BT) implement IRunnable with RunnableID.
5. **No RunContext in LunyScript public API (eg Do())** - Users access Variables/GlobalVariables directly via scope, cleaner API.
    - RECONSIDER: we could put RunContext back into the Do() action but split RunContext in public and internal contexts.
    - the public context (ScriptContext?) would contain the Object + the three Variables references
    - benefit: clear separation, hot reload may benefit too, and blocks could run in Do() lambdas: Log("TEST TEST TEST").Execute(ctx);
6. **Type-based hot reload** - RunContext.ScriptType used to match scripts for hot reload, entire context recreated preserving Variables.
7. **Variables in LunyScript/** - Not moved to Luny/, stays in LunyScript namespace.
8. **Debug hooks in all lifecycles** - Placeholders in Update, FixedStep, LateUpdate. (maybe extend to startup/shutdown)
9. LunyScript: exposes the user API, must not expose implementation details
10. Blocks: should avoid Lambdas (code samples in this document still use them)

### File Structure
```
LunyScript/
├── Core/
│   ├── IBlock.cs - Base interface for all blocks
│   ├── IRunnable.cs - Container blocks with ID and Children
│   ├── RunContext.cs - Runtime context per object
│   ├── ScriptID.cs - Sequential ID for script definitions
│   ├── RunnableID.cs - Sequential ID for runnables
│   └── Variables.cs - Dictionary-based storage (placeholder for LuaTable)
│
├── Runtime/
│   ├── Blocks/
│   │   ├── LogMessageBlock.cs
│   │   └── ActionBlock.cs
│   └── Runnables/
│       └── RunnableSequence.cs
│
├── Registry/
│   ├── ScriptDefinition.cs
│   ├── ScriptRegistry.cs
│   ├── RunContextRegistry.cs
│   └── ScenePreprocessor.cs
│
├── Debugging/
│   └── DebugHooks.cs (placeholder)
│
├── LunyScript.cs - Base class
├── LunyScript.API.cs - partial containing public API methods/properties
└── LunyScriptRunner.cs - Lifecycle observer
```

### Usage Pattern
```csharp
public class PlayerScript : LunyScript.LunyScript
{
    public override void Build()
    {
        Variables["Health"] = 100;
        GlobalVariables["Score"] = 0;

        OnUpdate(Log("Update tick"));
        OnUpdate(
            Log("Multi-block"),
            Do(() => Variables["Health"] = 99)
        );
        OnFixedStep(Log("Fixed tick"));
    }
}
```

---

## Step 2: Complete Debug Hooks & Execution Tracing

**Goal:** Full debugging infrastructure for execution visibility

### Implementation Details

**1. Expand DebugHooks class:**
```csharp
public sealed class DebugHooks
{
    public event Action<IBlock, RunContext> OnBlockExecute;
    public event Action<IBlock, RunContext> OnBlockComplete;
    public event Action<IBlock, RunContext> OnBlockError;

    public bool EnableTracing { get; set; }
    public List<ExecutionTrace> Traces { get; }

    internal void RecordTrace(ExecutionTrace trace)
    {
        if (EnableTracing)
            Traces.Add(trace);
    }
}
```

**2. Create ExecutionTrace struct:**
```csharp
public struct ExecutionTrace
{
    public double Timestamp;
    public RunnableID RunnableID;
    public ObjectID ContextObject;
    public string Action; // "Execute", "Complete", "Error"
    public string BlockType; // For debugging
}
```

**3. Uncomment hook invocations in LunyScriptRunner:**
- In OnUpdate(), OnFixedStep(), OnLateUpdate()
- Call OnBlockExecute before runnable.Execute()
- Call OnBlockComplete after successful execution
- Wrap in try/catch at runner level, call OnBlockError on exception

**4. Add variable change tracking to Variables class:**
```csharp
public event Action<string, object> OnVariableChanged;

public object this[string key]
{
    get => ...;
    set
    {
        _vars[key] = value;
        OnVariableChanged?.Invoke(key, value);
    }
}
```

**5. Wire DebugHooks.OnVariableChanged to Variables events in RunContext constructor**

### Files to Modify/Create
- Modify: `LunyScript/Debugging/DebugHooks.cs`
- Create: `LunyScript/Debugging/ExecutionTrace.cs`
- Modify: `LunyScript/Core/Variables.cs` (add events)
- Modify: `LunyScript/LunyScriptRunner.cs` (uncomment hooks, add try/catch)

### Testing
- Enable tracing on a context
- Run script with multiple blocks
- Verify Traces list populated
- Subscribe to OnBlockExecute, verify called
- Change variable, verify OnVariableChanged fired

**Estimated:** ~150 lines, 30 minutes

---

## Step 3: Hot Reload Infrastructure

**Goal:** Reload scripts without losing state (variables preserved, blocks recreated)

### Design

**Hot reload is per-ScriptType:**
1. Detect script type changed (manual API trigger for now, file watcher later)
2. Find all RunContexts with matching ScriptType
3. For each context:
   - Preserve: Object, Variables, InspectorVariables
   - Clear: UpdateRunnables, FixedStepRunnables, LateUpdateRunnables
   - Create new script instance
   - Call script.Initialize(context) with SAME context
   - Call script.Build() - repopulates runnables
4. GlobalVariables unchanged (shared reference)
5. Ask user about LunyScript callbacks for unload and reload - enabling users to make scripted hot reload changes (ie reset variables on hot reload). These should be blocks in the Build() method: OnBeforeReload(Log("unloading ..")), OnAfterReload(Log("loading .."))
   - decision: OnBeforeReload() runs from within the existing runnable (modify variables before new instance's OnStartup runs)
   - decision: OnAfterReload() runs after OnStartup() on the new instance
   - decide: should hot reloading scripts run their OnStartup() and OnShutdown() runnables or not?

**No state preservation attributes** - entire context recreated, all blocks replaced. Assume full invalidation.

### Implementation

**1. Create ScriptReloader:**
```csharp
public sealed class ScriptReloader
{
    private readonly ScriptRegistry _scriptRegistry;
    private readonly RunContextRegistry _contextRegistry;

    public void ReloadScript(Type scriptType)
    {
        var contexts = _contextRegistry.AllContexts
            .Where(ctx => ctx.ScriptType == scriptType)
            .ToList();

        foreach (var context in contexts)
        {
            // Clear runnables
            context.UpdateRunnables.Clear();
            context.FixedStepRunnables.Clear();
            context.LateUpdateRunnables.Clear();

            // Re-build
            var scriptDef = _scriptRegistry.GetByName(scriptType.Name);
            var scriptInstance = (LunyScript)Activator.CreateInstance(scriptType);
            scriptInstance.Initialize(context);
            scriptInstance.Build();

            LunyLogger.LogInfo($"Hot reloaded: {scriptType.Name} for {context.Object.Name}", this);
        }
    }
}
```

**2. Manual trigger API:**
```csharp
// For testing hot reload manually
public void TriggerHotReload<TScript>() where TScript : LunyScript
{
    _reloader.ReloadScript(typeof(TScript));
}
```

**3. Add to LunyScriptRunner:**
- Add `_scriptReloader` field
- Initialize in OnStartup()
- Expose trigger method (or integrate with file watcher in future)

### Files to Create/Modify
- Create: `LunyScript/HotReload/ScriptReloader.cs`
- Modify: `LunyScript/LunyScriptRunner.cs` (add reloader, expose trigger)

### Testing
1. Run script, modify variable
2. Edit script code (add new log block)
3. Trigger hot reload manually
4. Verify: variable preserved, new block executes

**Estimated:** ~150 lines, 30 minutes

---

## Step 4: Composite Blocks & Conditionals

**Goal:** More expressive script building with control flow

### Notes

- Not using lambdas!
- decide: then/else pattern - parameters or functional calls or something else?

### Implementation

**1. Conditional block:**
```csharp
public sealed class IfBlock : IBlock
{
    private readonly Func<bool> _condition;
    private readonly IBlock _thenBlock;
    private readonly IBlock _elseBlock;

    public IfBlock(Func<bool> condition, IBlock thenBlock, IBlock elseBlock = null)
    {
        _condition = condition;
        _thenBlock = thenBlock;
        _elseBlock = elseBlock;
    }

    public void Execute(RunContext context)
    {
        if (_condition())
            _thenBlock.Execute(context);
        else
            _elseBlock?.Execute(context);
    }
}
```

**2. Repeat block:**
```csharp
public sealed class RepeatBlock : IBlock
{
    private readonly int _count;
    private readonly IBlock _block;

    public void Execute(RunContext context)
    {
        for (int i = 0; i < _count; i++)
            _block.Execute(context);
    }
}
```

**3. Add to LunyScript base class:**
```csharp
protected IfBlock If(Func<bool> condition, IBlock then, IBlock @else = null)
    => new IfBlock(condition, then, @else);

protected RepeatBlock Repeat(int count, IBlock block)
    => new RepeatBlock(count, block);

// Explicit sequence (optional, since auto-wrap exists)
protected RunnableSequence Sequence(params IBlock[] blocks)
    => new RunnableSequence(blocks);
```

### Usage Example
```csharp
OnUpdate(
    If(() => Variables.Get<int>("Health") > 0,
        then: Log("Alive"),
        @else: Log("Dead")
    ),
    Repeat(3, Log("Repeated message"))
);
```

### Files to Create/Modify
- Create: `LunyScript/Runtime/Blocks/IfBlock.cs`
- Create: `LunyScript/Runtime/Blocks/RepeatBlock.cs`
- Modify: `LunyScript/LunyScript.cs` (add factory methods)

### Testing
- If block: change variable, verify correct branch executes
- Repeat block: verify log appears N times
- Nested: If inside Repeat, Repeat inside If

**Estimated:** ~120 lines, 25 minutes

---

## Step 5: Variable Blocks & Utilities

**Goal:** Common variable operations as reusable blocks (avoid lambdas for simple ops)

### Implementation

**1. SetVariableBlock:**
```csharp
public sealed class SetVariableBlock : IBlock
{
    private readonly string _name;
    private readonly object _value;

    public void Execute(RunContext context)
    {
        context.Variables[_name] = _value;
    }
}
```

**2. IncrementVariableBlock:**
```csharp
public sealed class IncrementVariableBlock : IBlock
{
    private readonly string _name;
    private readonly int _amount;

    public void Execute(RunContext context)
    {
        var current = context.Variables.Get<int>(_name);
        context.Variables[_name] = current + _amount;
    }
}
```

**3. LogVariableBlock:**
```csharp
public sealed class LogVariableBlock : IBlock
{
    private readonly string _name;

    public void Execute(RunContext context)
    {
        var value = context.Variables[_name];
        LunyLogger.LogInfo($"{_name} = {value}", this);
    }
}
```

**4. Add to LunyScript:**
```csharp
protected SetVariableBlock SetVariable(string name, object value)
    => new SetVariableBlock(name, value);

protected IncrementVariableBlock IncrementVariable(string name, int amount = 1)
    => new IncrementVariableBlock(name, amount);

protected LogVariableBlock LogVariable(string name)
    => new LogVariableBlock(name);
```

### Usage Example
```csharp
OnUpdate(
    SetVariable("Score", 100),
    IncrementVariable("Score", 10),
    LogVariable("Score") // Logs: Score = 110
);
```

### Files to Create/Modify
- Create: `LunyScript/Runtime/Blocks/SetVariableBlock.cs`
- Create: `LunyScript/Runtime/Blocks/IncrementVariableBlock.cs`
- Create: `LunyScript/Runtime/Blocks/LogVariableBlock.cs`
- Modify: `LunyScript/LunyScript.cs` (add factory methods)

**Estimated:** ~100 lines, 20 minutes

---

## Step 6: Event System Foundation

**Goal:** Input and collision events (blocks that check conditions internally)

### Design Philosophy

**Event blocks are regular IBlocks registered to Update:**
- They run every Update
- Internally track event state (was key down last frame? did collision occur?)
- Only execute child block when event fires
- State stored in block fields (stateful design)

### Implementation

**1. OnKeyPressedBlock:**
```csharp
public sealed class OnKeyPressedBlock : IBlock
{
    private readonly string _keyName;
    private readonly IBlock _childBlock;
    private readonly IInputServiceProvider _input;
    private bool _wasPressed;

    public OnKeyPressedBlock(string keyName, IBlock childBlock, IInputServiceProvider input)
    {
        _keyName = keyName;
        _childBlock = childBlock;
        _input = input;
    }

    public void Execute(RunContext context)
    {
        bool isPressed = _input.IsKeyPressed(_keyName);
        if (isPressed && !_wasPressed)
        {
            _childBlock.Execute(context);
        }
        _wasPressed = isPressed;
    }
}
```

**2. Service provider interfaces:**
```csharp
// Luny/Providers/IInputServiceProvider.cs
public interface IInputServiceProvider : IEngineServiceProvider
{
    bool IsKeyPressed(string keyName);
    bool IsKeyDown(string keyName);
    bool IsKeyUp(string keyName);
}

// Luny/Providers/IPhysicsServiceProvider.cs
public interface IPhysicsServiceProvider : IEngineServiceProvider
{
    bool CheckCollision(LunyObject obj, string tag);
    // More methods as needed
}
```

**3. Unity implementations:**
```csharp
// Luny.Unity/Providers/UnityInputServiceProvider.cs
public sealed class UnityInputServiceProvider : IInputServiceProvider
{
    public bool IsKeyPressed(string keyName)
    {
        return Input.GetKey(keyName);
    }
    // etc.
}
```

**4. Factory methods in LunyScript:**
```csharp
// Need to access input service - stored in context or passed?
// Option: Store service references in RunContext
protected OnKeyPressedBlock OnKeyPressed(string key, IBlock block)
{
    var input = _context.GetService<IInputServiceProvider>();
    return new OnKeyPressedBlock(key, block, input);
}
```

**5. Add service access to RunContext:**
```csharp
public T GetService<T>() where T : class, IEngineServiceProvider
{
    return LunyEngine.Instance.GetService<T>();
}
```

### Usage Example
```csharp
OnUpdate(
    OnKeyPressed("Space", Log("Jump!")),
    OnCollision("Enemy", Do(() => Variables["Health"] = 0))
);
```

### Files to Create/Modify
- Create: `Luny/Providers/IInputServiceProvider.cs`
- Create: `Luny/Providers/IPhysicsServiceProvider.cs`
- Create: `Luny.Unity/Providers/UnityInputServiceProvider.cs`
- Create: `Luny.Unity/Providers/UnityPhysicsServiceProvider.cs`
- Create: `LunyScript/Runtime/Blocks/OnKeyPressedBlock.cs`
- Create: `LunyScript/Runtime/Blocks/OnCollisionBlock.cs`
- Modify: `LunyScript/Core/RunContext.cs` (add GetService helper)
- Modify: `LunyScript/LunyScript.cs` (add event factory methods)

**Estimated:** ~300 lines, 1 hour

---

## Step 7: Inspector Variables Integration (DEFERRED)

**Goal:** Designer-set values in Unity/Godot inspector

**Complexity:** High - requires engine-specific MonoBehaviour/Node bridges, preprocessing, attribute scanning

**Defer until:** Steps 2-6 complete and stable

---

## Priority for Next Session

**Recommended order:**
1. **Step 2** (Debug Hooks) - 30 min - Critical for development
2. **Step 3** (Hot Reload) - 30 min - Huge productivity boost
3. **Step 4** (Conditionals) - 25 min - Makes scripts useful
4. **Step 5** (Variable Blocks) - 20 min - Quality of life
5. **Step 6** (Events) - 1 hour - Real game functionality

**Total estimated time:** ~2.5 hours

**After Step 6:** System is feature-complete for basic game scripting. Can build simple games with input, collision, variables, conditionals.

---

## Important Notes for Next Session

### Design Principles to Maintain
1. **Beginner-friendly API** - No context exposure in Do(), clean factory methods
2. **Stateful is OK** - Not pursuing stateless optimization, blocks can have fields
3. **Always wrap in sequence** - Consistency over micro-optimization
4. **IBlock for leaves, IRunnable for containers** - Clear separation
5. **Single-threaded** - No concurrency concerns in ID generation or execution

### Common Gotchas
- **Namespace collision:** `LunyScript.LunyScript` class vs namespace, users must use full qualifier or alias
- **Variables events:** If adding change tracking, ensure no infinite loops
- **Service access:** Event blocks need service providers, store reference or get from context
- **Hot reload:** Clear ALL runnable lists (Update, FixedStep, LateUpdate), preserve Variables

### Testing Checklist
- [ ] ExampleLunyScript still works after changes
- [ ] GameObject named "ExampleLunyScript" in scene
- [ ] Console shows discovery, binding, building, execution logs
- [ ] Variables persist across frames
- [ ] GlobalVariables shared between multiple scripts
- [ ] No exceptions in console

---

## End State After Step 6

**Users will be able to write:**
```csharp
public class PlayerScript : LunyScript.LunyScript
{
    public override void Build()
    {
        Variables["Health"] = 100;

        OnUpdate(
            OnKeyPressed("Space", Sequence(
                Log("Jump!"),
                IncrementVariable("JumpCount")
            )),

            If(() => Variables.Get<int>("Health") <= 0,
                then: Log("Game Over"),
                @else: LogVariable("Health")
            ),

            OnCollision("Enemy", Do(() => {
                var health = Variables.Get<int>("Health");
                Variables["Health"] = health - 10;
            }))
        );
    }
}
```

**This enables:**
- Input handling
- Collision detection
- Variable manipulation
- Conditional logic
- Debug tracing
- Hot reload during development
- Global state management

**Ready for real game development!**
