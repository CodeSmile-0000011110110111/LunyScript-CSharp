namespace ILunyScript
{
	public abstract partial class ILunyScript
	{
		// these API outlines exist to get a feel for the intellisense/autocompletion behaviour ...

		// planned API outline
		public AnimationApi Animation => new(this);
		public readonly struct AnimationApi
		{
			private readonly ILunyScript _script;
			internal AnimationApi(ILunyScript script) => _script = script;
		}

		public ApplicationApi Application => new(this);
		public readonly struct ApplicationApi
		{
			private readonly ILunyScript _script;
			internal ApplicationApi(ILunyScript script) => _script = script;
		}

		public AssetApi Asset => new(this);
		public readonly struct AssetApi
		{
			private readonly ILunyScript _script;
			internal AssetApi(ILunyScript script) => _script = script;
		}

		public CameraApi Camera => new(this);
		public readonly struct CameraApi
		{
			private readonly ILunyScript _script;
			internal CameraApi(ILunyScript script) => _script = script;
		}

		public DiagnosticsApi Diagnostics => new(this);
		public readonly struct DiagnosticsApi
		{
			private readonly ILunyScript _script;
			internal DiagnosticsApi(ILunyScript script) => _script = script;
		}

		public HUDApi HUD => new(this);
		public readonly struct HUDApi
		{
			private readonly ILunyScript _script;
			internal HUDApi(ILunyScript script) => _script = script;
		}

		public InputApi Input => new(this);
		public readonly struct InputApi
		{
			private readonly ILunyScript _script;
			internal InputApi(ILunyScript script) => _script = script;
		}

		public MenuApi Menu => new(this);
		public readonly struct MenuApi
		{
			private readonly ILunyScript _script;
			internal MenuApi(ILunyScript script) => _script = script;
		}

		public PhysicsApi Physics => new(this);
		public readonly struct PhysicsApi
		{
			private readonly ILunyScript _script;
			internal PhysicsApi(ILunyScript script) => _script = script;
		}

		public PlayerApi Player => new(this);
		public readonly struct PlayerApi
		{
			private readonly ILunyScript _script;
			internal PlayerApi(ILunyScript script) => _script = script;
		}

		public StorageApi Storage => new(this);
		public readonly struct StorageApi
		{
			private readonly ILunyScript _script;
			internal StorageApi(ILunyScript script) => _script = script;
		}

		// possible future expansions
		public AccessibilityApi Accessibility => new(this);
		public readonly struct AccessibilityApi
		{
			private readonly ILunyScript _script;
			internal AccessibilityApi(ILunyScript script) => _script = script;
		}

		public AccountApi Account => new(this);
		public readonly struct AccountApi
		{
			private readonly ILunyScript _script;
			internal AccountApi(ILunyScript script) => _script = script;
		}

		public AIApi AI => new(this);
		public readonly struct AIApi
		{
			private readonly ILunyScript _script;
			internal AIApi(ILunyScript script) => _script = script;
		}

		public AsyncApi Async => new(this);
		public readonly struct AsyncApi
		{
			private readonly ILunyScript _script;
			internal AsyncApi(ILunyScript script) => _script = script;
		}

		public AvatarApi Avatar => new(this);
		public readonly struct AvatarApi
		{
			private readonly ILunyScript _script;
			internal AvatarApi(ILunyScript script) => _script = script;
		}

		public CloudApi Cloud => new(this);
		public readonly struct CloudApi
		{
			private readonly ILunyScript _script;
			internal CloudApi(ILunyScript script) => _script = script;
		}

		public CutsceneApi Cutscene => new(this);
		public readonly struct CutsceneApi
		{
			private readonly ILunyScript _script;
			internal CutsceneApi(ILunyScript script) => _script = script;
		}

		public EnvironmentApi Environment => new(this);
		public readonly struct EnvironmentApi
		{
			private readonly ILunyScript _script;
			internal EnvironmentApi(ILunyScript script) => _script = script;
		}

		public GraphicsApi Graphics => new(this);
		public readonly struct GraphicsApi
		{
			private readonly ILunyScript _script;
			internal GraphicsApi(ILunyScript script) => _script = script;
		}

		public L18nApi L18n => new(this);
		public readonly struct L18nApi
		{
			private readonly ILunyScript _script;
			internal L18nApi(ILunyScript script) => _script = script;
		}

		public LocaleApi Locale => new(this);
		public readonly struct LocaleApi
		{
			private readonly ILunyScript _script;
			internal LocaleApi(ILunyScript script) => _script = script;
		}

		public LocalizationApi Localization => new(this);
		public readonly struct LocalizationApi
		{
			private readonly ILunyScript _script;
			internal LocalizationApi(ILunyScript script) => _script = script;
		}

		public NavigationApi Navigation => new(this);
		public readonly struct NavigationApi
		{
			private readonly ILunyScript _script;
			internal NavigationApi(ILunyScript script) => _script = script;
		}

		public NetworkApi Network => new(this);
		public readonly struct NetworkApi
		{
			private readonly ILunyScript _script;
			internal NetworkApi(ILunyScript script) => _script = script;
		}

		public NPCApi NPC => new(this);
		public readonly struct NPCApi
		{
			private readonly ILunyScript _script;
			internal NPCApi(ILunyScript script) => _script = script;
		}

		public ParticlesApi Particles => new(this);
		public readonly struct ParticlesApi
		{
			private readonly ILunyScript _script;
			internal ParticlesApi(ILunyScript script) => _script = script;
		}

		public PoolApi Pool => new(this);
		public readonly struct PoolApi
		{
			private readonly ILunyScript _script;
			internal PoolApi(ILunyScript script) => _script = script;
		}

		public PostFxApi PostFx => new(this);
		public readonly struct PostFxApi
		{
			private readonly ILunyScript _script;
			internal PostFxApi(ILunyScript script) => _script = script;
		}

		public ProgressApi Progress => new(this);
		public readonly struct ProgressApi
		{
			private readonly ILunyScript _script;
			internal ProgressApi(ILunyScript script) => _script = script;
		}

		public QualityApi Quality => new(this);
		public readonly struct QualityApi
		{
			private readonly ILunyScript _script;
			internal QualityApi(ILunyScript script) => _script = script;
		}

		public ScriptApi Script => new(this);
		public readonly struct ScriptApi
		{
			private readonly ILunyScript _script;
			internal ScriptApi(ILunyScript script) => _script = script;
		}

		public SessionApi Session => new(this);
		public readonly struct SessionApi
		{
			private readonly ILunyScript _script;
			internal SessionApi(ILunyScript script) => _script = script;
		}

		public SettingsApi Settings => new(this);
		public readonly struct SettingsApi
		{
			private readonly ILunyScript _script;
			internal SettingsApi(ILunyScript script) => _script = script;
		}

		public SpawnApi Spawn => new(this);
		public readonly struct SpawnApi
		{
			private readonly ILunyScript _script;
			internal SpawnApi(ILunyScript script) => _script = script;
		}

		public SpriteApi Sprite => new(this);
		public readonly struct SpriteApi
		{
			private readonly ILunyScript _script;
			internal SpriteApi(ILunyScript script) => _script = script;
		}

		public StageApi Stage => new(this);
		public readonly struct StageApi
		{
			private readonly ILunyScript _script;
			internal StageApi(ILunyScript script) => _script = script;
		}

		public TerrainApi Terrain => new(this);
		public readonly struct TerrainApi
		{
			private readonly ILunyScript _script;
			internal TerrainApi(ILunyScript script) => _script = script;
		}

		public TilemapApi Tilemap => new(this);
		public readonly struct TilemapApi
		{
			private readonly ILunyScript _script;
			internal TilemapApi(ILunyScript script) => _script = script;
		}

		public TutorialApi Tutorial => new(this);
		public readonly struct TutorialApi
		{
			private readonly ILunyScript _script;
			internal TutorialApi(ILunyScript script) => _script = script;
		}

		public UIApi UI => new(this);
		public readonly struct UIApi
		{
			private readonly ILunyScript _script;
			internal UIApi(ILunyScript script) => _script = script;
		}

		public VFXApi VFX => new(this);
		public readonly struct VFXApi
		{
			private readonly ILunyScript _script;
			internal VFXApi(ILunyScript script) => _script = script;
		}

		public VideoApi Video => new(this);
		public readonly struct VideoApi
		{
			private readonly ILunyScript _script;
			internal VideoApi(ILunyScript script) => _script = script;
		}

		// publishing expansions
		public PlatformApi Platform => new(this);
		public readonly struct PlatformApi
		{
			private readonly ILunyScript _script;
			internal PlatformApi(ILunyScript script) => _script = script;

			public DesktopApi Desktop => new(_script);
			public readonly struct DesktopApi
			{
				private readonly ILunyScript _script;
				internal DesktopApi(ILunyScript script) => _script = script;
			}

			public LinuxApi Linux => new(_script);
			public readonly struct LinuxApi
			{
				private readonly ILunyScript _script;
				internal LinuxApi(ILunyScript script) => _script = script;
			}

			public MobileApi Mobile => new(_script);
			public readonly struct MobileApi
			{
				private readonly ILunyScript _script;
				internal MobileApi(ILunyScript script) => _script = script;
			}

			public OSXApi OSX => new(_script);
			public readonly struct OSXApi
			{
				private readonly ILunyScript _script;
				internal OSXApi(ILunyScript script) => _script = script;
			}

			public WebApi Web => new(_script);
			public readonly struct WebApi
			{
				private readonly ILunyScript _script;
				internal WebApi(ILunyScript script) => _script = script;
			}

			public WindowsApi Windows => new(_script);
			public readonly struct WindowsApi
			{
				private readonly ILunyScript _script;
				internal WindowsApi(ILunyScript script) => _script = script;
			}

			public XRApi XR => new(_script);
			public readonly struct XRApi
			{
				private readonly ILunyScript _script;
				internal XRApi(ILunyScript script) => _script = script;
			}
		}

		public StoreApi Store => new(this);
		public readonly struct StoreApi
		{
			private readonly ILunyScript _script;
			internal StoreApi(ILunyScript script) => _script = script;

			public AppleApi Apple => new(_script);
			public readonly struct AppleApi
			{
				private readonly ILunyScript _script;
				internal AppleApi(ILunyScript script) => _script = script;
			}

			public EpicApi Epic => new(_script);
			public readonly struct EpicApi
			{
				private readonly ILunyScript _script;
				internal EpicApi(ILunyScript script) => _script = script;
			}

			public GoogleApi Google => new(_script);
			public readonly struct GoogleApi
			{
				private readonly ILunyScript _script;
				internal GoogleApi(ILunyScript script) => _script = script;
			}

			public SteamApi Steam => new(_script);
			public readonly struct SteamApi
			{
				private readonly ILunyScript _script;
				internal SteamApi(ILunyScript script) => _script = script;
			}
		}
	}
}
