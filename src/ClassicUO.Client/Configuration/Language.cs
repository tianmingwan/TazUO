using System;
using System.IO;
using System.Text.Json;

namespace ClassicUO.Configuration
{
    using System.Text.Json.Serialization;

    [JsonSerializable(typeof(Language))]
    [JsonSerializable(typeof(ModernOptionsGumpLanguage))]
    [JsonSerializable(typeof(ScriptingLanguage))]
    [JsonSerializable(typeof(UiCommonsLanguage))]
    [JsonSerializable(typeof(ErrorsLanguage))]
    [JsonSerializable(typeof(MapLanguage))]
    [JsonSerializable(typeof(TopBarGumpLanguage))]
    [JsonSerializable(typeof(AssistantLanguage))]
    [JsonSerializable(typeof(AgentsLanguage))]
    [JsonSerializable(typeof(BandageAgentLanguage))]
    [JsonSerializable(typeof(AutoLootAgentLanguage))]
    [JsonSerializable(typeof(DressAgentLanguage))]
    [JsonSerializable(typeof(AutoBuyAgentLanguage))]
    [JsonSerializable(typeof(AutoSellAgentLanguage))]
    [JsonSerializable(typeof(OrganizerAgentLanguage))]
    [JsonSerializable(typeof(GraphicReplacementLanguage))]
    [JsonSerializable(typeof(JournalFilterLanguage))]
    [JsonSerializable(typeof(SoundFilterLanguage))]
    [JsonSerializable(typeof(MusicFilterLanguage))]
    [JsonSerializable(typeof(SeasonFilterLanguage))]
    [JsonSerializable(typeof(ItemDatabaseLanguage))]
    [JsonSerializable(typeof(ItemDetailLanguage))]
    [JsonSerializable(typeof(MacrosLanguage))]
    [JsonSerializable(typeof(SkillsLanguage))]
    [JsonSerializable(typeof(HudLanguage))]
    [JsonSerializable(typeof(SpellBarLanguage))]
    [JsonSerializable(typeof(TitleBarLanguage))]
    [JsonSerializable(typeof(SpellIndicatorLanguage))]
    [JsonSerializable(typeof(FriendsListLanguage))]
    [JsonSerializable(typeof(PathfindingLanguage))]
    [JsonSerializable(typeof(TazUOChatLanguage))]
    [JsonSerializable(typeof(JournalGumpLanguage))]
    [JsonSerializable(typeof(WorldMapGumpLanguage))]
    [JsonSerializable(typeof(MapGumpLanguage))]
    [JsonSerializable(typeof(GridContainerLanguage))]
    [JsonSerializable(typeof(CounterBarLanguage))]
    public partial class LanguageJsonContext : JsonSerializerContext
    {
    }

    public class Language
    {
        public ModernOptionsGumpLanguage GetModernOptionsGumpLanguage { get; set; } = new();
        public ErrorsLanguage ErrorsLanguage { get; set; } = new();
        public MapLanguage MapLanguage { get; set; } = new();
        public TopBarGumpLanguage TopBarGump { get; set; } = new();
        public ScriptingLanguage Scripting { get; set; } = new();
        public AssistantLanguage Assistant { get; set; } = new();
        public UiCommonsLanguage UiCommons { get; set; } = new();
        public TazUOChatLanguage TazUOChat { get; set; } = new();
        public JournalGumpLanguage JournalGump { get; set; } = new();
        public WorldMapGumpLanguage WorldMapGump { get; set; } = new();
        public MapGumpLanguage MapGump { get; set; } = new();
        public GridContainerLanguage GridContainer { get; set; } = new();
        public CounterBarLanguage CounterBar { get; set; } = new();

        public string TazuoVersionHistory { get; set; } = "TazUO Version History";
        public string CurrentVersion { get; set; } = "Current Version: ";
        public string TazUOWiki { get; set; } = "TazUO Wiki";
        public string TazUODiscord { get; set; } = "TazUO Discord";
        public string CommandGump { get; set; } = "Available Client Commands";
        public string VersionHistoryNotice { get; set; } = "Version history can now be found on our GitHub repo CHANGELOG.md file.";
        public string MainBranchChangelog { get; set; } = "Main branch changelog";
        public string DevBranchChangelog { get; set; } = "Dev branch changelog";

        [JsonIgnore]
        public static Language Instance { get; private set; } = new();

        private static string _loadedLanguageCode = "EN";

        public static void Load()
        {
            string uiLang;
            try
            {
                uiLang = Settings.GlobalSettings?.UILanguage ?? "EN";
            }
            catch (NullReferenceException)
            {
                uiLang = "EN";
            }

            _loadedLanguageCode = uiLang;
            string path = GetLanguageFilePath(uiLang);

            var options = new JsonSerializerOptions { WriteIndented = true };

            if (File.Exists(path))
            {
                Instance = JsonSerializer.Deserialize<Language>(File.ReadAllText(path), options);
                Save(options);
            }
            else if (!uiLang.Equals("EN", StringComparison.OrdinalIgnoreCase))
            {
                string enPath = GetLanguageFilePath("EN");
                if (File.Exists(enPath))
                {
                    Instance = JsonSerializer.Deserialize<Language>(File.ReadAllText(enPath), options);
                }
                else
                {
                    CreateNewLanguageFile(options);
                }
            }
            else
            {
                CreateNewLanguageFile(options);
            }
        }

        private static void CreateNewLanguageFile(JsonSerializerOptions options)
        {
            Directory.CreateDirectory(Path.Combine(CUOEnviroment.ExecutablePath, "Data"));

            string defaultLanguage = JsonSerializer.Serialize(Instance, options);
            File.WriteAllText(GetLanguageFilePath(_loadedLanguageCode), defaultLanguage);
        }

        private static void Save(JsonSerializerOptions options)
        {
            string language = JsonSerializer.Serialize(Instance, options);
            File.WriteAllText(GetLanguageFilePath(_loadedLanguageCode), language);
        }

        private static string GetLanguageFilePath(string code)
        {
            string fileName = code.Equals("EN", StringComparison.OrdinalIgnoreCase)
                ? "Language.json"
                : $"Language.{code}.json";
            return Path.Combine(CUOEnviroment.ExecutablePath, "Data", fileName);
        }
    }

    public class ModernOptionsGumpLanguage
    {
        public string OptionsTitle { get; set; } = "Options";
        public string Search { get; set; } = "Search";

        public string ButtonGeneral { get; set; } = "General";
        public string ButtonSound { get; set; } = "Sound";
        public string ButtonVideo { get; set; } = "Video";
        public string ButtonMacros { get; set; } = "Macros";
        public string ButtonTooltips { get; set; } = "Tooltips";
        public string ButtonSpeech { get; set; } = "Speech";
        public string ButtonCombatSpells { get; set; } = "Combat & Spells";
        public string ButtonCounters { get; set; } = "Counters";
        public string ButtonInfobar { get; set; } = "Infobar";
        public string ButtonContainers { get; set; } = "Containers";
        public string ButtonExperimental { get; set; } = "Experimental";
        public string ButtonIgnoreList { get; set; } = "Ignore List";
        public string ButtonNameplates { get; set; } = "Nameplate Options";
        public string ButtonCooldowns { get; set; } = "Cooldown bars";
        public string ButtonTazUO { get; set; } = "TazUO Specific";
        public string ButtonMobiles { get; set; } = "Mobiles";
        public string ButtonGumpContext { get; set; } = "Gumps & Context";
        public string ButtonMisc { get; set; } = "Misc";
        public string ButtonTerrainStatics { get; set; } = "Terrain & Statics";
        public string ButtonGameWindow { get; set; } = "Game window";
        public string ButtonZoom { get; set; } = "Zoom";
        public string ButtonLighting { get; set; } = "Lighting";
        public string ButtonShadows { get; set; } = "Shadows";

        public General GetGeneral { get; set; } = new();
        public Video GetVideo { get; set; } = new();
        public Sound GetSound { get; set; } = new();
        public Macros GetMacros { get; set; } = new();
        public ToolTips GetToolTips { get; set; } = new();
        public Speech GetSpeech { get; set; } = new();
        public CombatSpells GetCombatSpells { get; set; } = new();
        public Counters GetCounters { get; set; } = new();
        public InfoBars GetInfoBars { get; set; } = new();
        public Containers GetContainers { get; set; } = new();
        public Experimental GetExperimental { get; set; } = new();
        public NamePlates GetNamePlates { get; set; } = new();
        public Cooldowns GetCooldowns { get; set; } = new();
        public TazUO GetTazUO { get; set; } = new();

        public class General
        {
            public string SharedNone { get; set; } = "None";
            public string SharedShift { get; set; } = "Shift";
            public string SharedCtrl { get; set; } = "Ctrl";
            public string SharedAlt { get; set; } = "Alt";

            #region General->General
            public string HighlightObjects { get; set; } = "Highlight objects under cursor";
            public string Pathfinding { get; set; } = "Enable pathfinding";
            public string ShiftPathfinding { get; set; } = "Use shift for pathfinding";
            public string SingleClickPathfind { get; set; } = "Single click for pathfinding";
            public string AlwaysRun { get; set; } = "Always run";
            public string RunUnlessHidden { get; set; } = "Unless hidden";
            public string AutoOpenDoors { get; set; } = "Automatically open doors";
            public string AutoOpenPathfinding { get; set; } = "Open doors while pathfinding";
            public string AutoOpenCorpse { get; set; } = "Automatically open corpses";
            public string CorpseOpenDistance { get; set; } = "Corpse open distance";
            public string CorpseSkipEmpty { get; set; } = "Skip empty corpses";
            public string CorpseOpenOptions { get; set; } = "Corpse open options";
            public string CorpseOptNone { get; set; } = "None";
            public string CorpseOptNotTarg { get; set; } = "Not targeting";
            public string CorpseOptNotHiding { get; set; } = "Not hiding";
            public string CorpseOptBoth { get; set; } = "Both";
            public string OutRangeColor { get; set; } = "No color for out of range objects";
            public string SallosEasyGrab { get; set; } = "Enable sallos easy grab";
            public string SallosTooltip { get; set; } = "Sallos easy grab is not recommended with grid containers enabled.";
            public string ShowHouseContent { get; set; } = "Show house content";
            public string SmoothBoat { get; set; } = "Smooth boat movements";
            #endregion

            #region General->Mobiles
            public string ShowMobileHP { get; set; } = "Show mobile's HP";
            public string ShowTargetIndicator { get; set; } = "Show Target Indicator";
            public string MobileHPType { get; set; } = "Type";
            public string HPTypePerc { get; set; } = "Percentage";
            public string HPTypeBar { get; set; } = "Bar";
            public string HPTypeNBoth { get; set; } = "Both";
            public string HPShowWhen { get; set; } = "Show when";
            public string HPShowWhen_Always { get; set; } = "Always";
            public string HPShowWhen_Less100 { get; set; } = "Less than 100%";
            public string HPShowWhen_Smart { get; set; } = "Smart";
            public string HighlightPoisoned { get; set; } = "Highlight poisoned mobiles";
            public string PoisonHighlightColor { get; set; } = "Highlight color";
            public string HighlightPara { get; set; } = "Highlight paralyzed mobiles";
            public string ParaHighlightColor { get; set; } = "Highlight color";
            public string HighlightInvul { get; set; } = "Highlight invulnerable mobiles";
            public string InvulHighlightColor { get; set; } = "Highlight color";
            public string IncomingMobiles { get; set; } = "Show incoming mobile names";
            public string IncomingCorpses { get; set; } = "Show incoming corpse names";
            public string AuraUnderFeet { get; set; } = "Show aura under feet";
            public string AuraOptDisabled { get; set; } = "Disabled";
            public string AuroOptWarmode { get; set; } = "Warmode";
            public string AuraOptCtrlShift { get; set; } = "Ctrl + Shift";
            public string AuraOptAlways { get; set; } = "Always";
            public string AuraForParty { get; set; } = "Use a custom color for party members";
            public string AuraPartyColor { get; set; } = "Party aura color";
            public string IgnoreStaminaCheck { get; set; } = "Disable stamina check for movement";
            public string DisableGrayEnemies { get; set; } = "Don't make last target/enemies gray";
            public string DisableDismountWarmode { get; set; } = "Prevent dismounting in combat";
            #endregion

            #region General->Gumps
            public string DisableTopMenu { get; set; } = "Disable top menu bar";
            public string AltForAnchorsGumps { get; set; } = "Require alt to close anchored gumps";
            public string AltToMoveGumps { get; set; } = "Require alt to move gumps";
            public string CloseEntireAnchorWithRClick { get; set; } = "Close entire group of anchored gumps with right click";
            public string OriginalSkillsGump { get; set; } = "Use original skills gump";
            public string OldStatusGump { get; set; } = "Use old status gump";
            public string PartyInviteGump { get; set; } = "Show party invite gump";
            public string ModernHealthBars { get; set; } = "Use modern health bar gumps";
            public string ModernHPBlackBG { get; set; } = "Use black background";
            public string SaveHPBars { get; set; } = "Save health bars on logout";
            public string CloseHPGumpsWhen { get; set; } = "Close health bars when";
            public string CloseHPOptDisable { get; set; } = "Disabled";
            public string CloseHPOptOOR { get; set; } = "Out of range";
            public string CloseHPOptDead { get; set; } = "Dead";
            public string CloseHPOptBoth { get; set; } = "Both";
            public string GridLoot { get; set; } = "Grid Loot";
            public string GridLootOptDisable { get; set; } = "Disabled";
            public string GridLootOptOnly { get; set; } = "Grid loot only";
            public string GridLootOptBoth { get; set; } = "Grid loot and normal container";
            public string GridLootTooltip { get; set; } = "This is not the same as Grid Containers, this is a simple grid gump used for looting corpses.";
            public string ShiftContext { get; set; } = "Require shift to open context menus";
            public string ShiftSplit { get; set; } = "Require shift to split stacks of items";

            #endregion

            #region General->Misc
            public string EnableCOT { get; set; } = "Enable circle of transparency";
            public string COTDistance { get; set; } = "Distance";
            public string COTType { get; set; } = "Type";
            public string COTTypeOptFull { get; set; } = "Full";
            public string COTTypeOptGrad { get; set; } = "Gradient";
            public string COTTypeOptModern { get; set; } = "Modern";
            public string HideScreenshotMessage { get; set; } = "Hide 'screenshot stored in' message";
            public string ObjFade { get; set; } = "Enable object fading";
            public string TextFade { get; set; } = "Enable text fading";
            public string CursorRange { get; set; } = "Show target range indicator";

            public string AutoAvoidObstacules { get; set; } = "Auto Avoid Obstacles";
            public string DragSelectHP { get; set; } = "Enable drag select for health bars";
            public string DragKeyMod { get; set; } = "Key modifier";
            public string DragPlayersOnly { get; set; } = "Players only";
            public string DragMobsOnly { get; set; } = "Monsters only";
            public string DragNameplatesOnly { get; set; } = "Visible nameplates only";
            public string DragX { get; set; } = "X Position of healthbars";
            public string DragY { get; set; } = "Y Position of healthbars";
            public string DragAnchored { get; set; } = "Anchor opened health bars together";
            public string ShowStatsChangedMsg { get; set; } = "Show stats changed messages";
            public string ShowSkillsChangedMsg { get; set; } = "Show skills changed messages";
            public string ChangeVolume { get; set; } = "Every tenth (0.1)";
            #endregion

            #region General->TerrainStatics
            public string HideRoof { get; set; } = "Hide roof tiles";
            public string TreesToStump { get; set; } = "Change trees to stumps";
            public string HideVegetation { get; set; } = "Hide vegetation";
            public string MagicFieldType { get; set; } = "Field types";
            public string MagicFieldOpt_Normal { get; set; } = "Normal";
            public string MagicFieldOpt_Static { get; set; } = "Static";
            public string MagicFieldOpt_Tile { get; set; } = "Tile";
            #endregion
        }

        public class Sound
        {
            public string SharedVolume { get; set; } = "Volume";

            public string EnableSound { get; set; } = "Enable sound";
            public string EnableMusic { get; set; } = "Enable music";
            public string LoginMusic { get; set; } = "Enable login page music";
            public string PlayFootsteps { get; set; } = "Play footsteps";
            public string CombatMusic { get; set; } = "Combat music";
            public string BackgroundMusic { get; set; } = "Play sound when UO is not in focus";
        }

        public class Video
        {
            #region GameWindow
            public string FPSCap { get; set; } = "FPS Cap";
            public string BackgroundFPS { get; set; } = "Reduce FPS when game is not in focus";
            public string EnableVSync { get; set; } = "Enable VSync";
            public string FullsizeViewport { get; set; } = "Always use fullsize game world viewport";
            public string FullScreen { get; set; } = "Fullscreen window";
            public string LockViewport { get; set; } = "Lock game world viewport position/size";
            public string ViewportX { get; set; } = "Viewport position X";
            public string ViewportY { get; set; } = "Viewport position Y";
            public string ViewportW { get; set; } = "Viewport width";
            public string ViewportH { get; set; } = "Viewport height";
            #endregion

            #region Zoom
            public string DefaultZoom { get; set; } = "Default zoom";
            public string ZoomWheel { get; set; } = "Enable zooming with ctrl + mousewheel";
            public string ReturnDefaultZoom { get; set; } = "Return to default zoom after ctrl is released";
            #endregion

            #region Lighting
            public string AltLights { get; set; } = "Alternative lights";
            public string CustomLLevel { get; set; } = "Custom light level";
            public string Level { get; set; } = "Light level";
            public string LightType { get; set; } = "Light level type";
            public string LightType_Absolute { get; set; } = "Absolute";
            public string LightType_Minimum { get; set; } = "Minimum";
            public string DarkNight { get; set; } = "Dark nights";
            public string ColoredLight { get; set; } = "Colored lighting";
            #endregion

            #region Misc
            public string EnableDeathScreen { get; set; } = "Enable death screen";
            public string BWDead { get; set; } = "Black and white mode while dead";
            public string MouseThread { get; set; } = "Run mouse in seperate thread";
            public string TargetAura { get; set; } = "Aura on mouse target";
            public string AnimWater { get; set; } = "Animated water effect";
            public string EnablePostProcessing { get; set; } = "Enable post processing effects";
            public string PostProcessingType { get; set; } = "Processing type";
            #endregion

            #region Shadows
            public string EnableShadows { get; set; } = "Enable shadows";
            public string RockTreeShadows { get; set; } = "Rock and tree shadows";
            public string TerrainShadowLevel { get; set; } = "Terrain shadow level";
            #endregion
        }

        public class Macros
        {
            public string NewMacro { get; set; } = "New Macro";
            public string DelMacro { get; set; } = "Delete Macro";
            public string MoveUp { get; set; } = "Move Up";
            public string MoveDown { get; set; } = "Move Down";
        }

        public class ToolTips
        {
            public string EnableToolTips { get; set; } = "Enable tooltips";
            public string ToolTipDelay { get; set; } = "Tooltip delay";
            public string ToolTipBG { get; set; } = "Tooltip background opacity";
            public string ToolTipFont { get; set; } = "Default tooltip font color";
        }

        public class Speech
        {
            public string ScaleSpeechDelay { get; set; } = "Scale speech delay";
            public string SpeechDelay { get; set; } = "Delay";
            public string SaveJournalE { get; set; } = "Save journal entries to file";
            public string ChatEnterActivation { get; set; } = "Activate chat by pressing Enter";
            public string ChatEnterSpecial { get; set; } = "Also activate with common keys( ! ; : / \\ \\ , . [ | ~ )";
            public string ShiftEnterChat { get; set; } = "Use Shift + Enter to send message without closing chat";
            public string ChatGradient { get; set; } = "Hide chat gradient";
            public string HideGuildChat { get; set; } = "Hide guild chat";
            public string HideAllianceChat { get; set; } = "Hide alliance chat";
            public string SpeechColor { get; set; } = "Speech color";
            public string YellColor { get; set; } = "Yell color";
            public string PartyColor { get; set; } = "Party color";
            public string AllianceColor { get; set; } = "Alliance color";
            public string EmoteColor { get; set; } = "Emote color";
            public string WhisperColor { get; set; } = "Whisper color";
            public string GuildColor { get; set; } = "Guild color";
            public string CharColor { get; set; } = "Chat color";
        }

        public class CombatSpells
        {
            public string HoldTabForCombat { get; set; } = "Hold tab for combat";
            public string QueryBeforeAttack { get; set; } = "Query before attack";
            public string QueryBeforeBeneficial { get; set; } = "Query before beneficial acts on murderers/criminals/gray";
            public string EnableOverheadSpellFormat { get; set; } = "Enable overhead spell format";
            public string EnableOverheadSpellHue { get; set; } = "Enable overhead spell hue";
            public string SingleClickForSpellIcons { get; set; } = "Single click for spell icons";
            public string ShowBuffDurationOnOldStyleBuffBar { get; set; } = "Show buff duration on old style buff bar";
            public string EnableFastSpellHotkeyAssigning { get; set; } = "Enable fast spell hotkey assigning";
            public string EnableDPSCounter { get; set; } = "Enable damage-taken DPS counter with damage numbers";
            public string TooltipFastSpellAssign { get; set; } = "Ctrl + Alt + Click a spell icon the open a gump to set a hotkey";
            public string InnocentColor { get; set; } = "Innocent color";
            public string BeneficialSpell { get; set; } = "Beneficial spell";
            public string FriendColor { get; set; } = "Friend color";
            public string HarmfulSpell { get; set; } = "Harmful spell";
            public string Criminal { get; set; } = "Criminal";
            public string NeutralSpell { get; set; } = "Neutral spell";
            public string CanBeAttackedHue { get; set; } = "Can be attacked hue";
            public string Murderer { get; set; } = "Murderer";
            public string Enemy { get; set; } = "Enemy";
            public string SpellOverheadFormat { get; set; } = "Spell overhead format";
            public string TooltipSpellFormat { get; set; } = "{power} for powerword, {spell} for spell name";
        }

        public class Counters
        {
            public string EnableCounters { get; set; } = "Enable counters";
            public string HighlightItemsOnUse { get; set; } = "Highlight items on use";
            public string AbbreviatedValues { get; set; } = "Abbreviated values";
            public string AbbreviateIfAmountExceeds { get; set; } = "Abbreviate if amount exceeds";
            public string HighlightRedWhenAmountIsLow { get; set; } = "Highlight red when amount is low";
            public string HighlightRedIfAmountIsBelow { get; set; } = "Highlight red if amount is below";
            public string CounterLayout { get; set; } = "Counter layout";
            public string GridSize { get; set; } = "Grid size";
            public string Rows { get; set; } = "Rows";
            public string Columns { get; set; } = "Columns";
        }

        public class InfoBars
        {
            public string ShowInfoBar { get; set; } = "Show info bar";
            public string HighlightType { get; set; } = "Highlight type";
            public string HighLightOpt_TextColor { get; set; } = "Text color";
            public string HighLightOpt_ColoredBars { get; set; } = "Colored bars";
            public string AddItem { get; set; } = "+ Add item";
            public string Hp { get; set; } = "HP";
            public string Label { get; set; } = "Label";
            public string Color { get; set; } = "Color";
            public string Data { get; set; } = "Data";
        }

        public class Containers
        {
            public string Description { get; set; } = "These settings are for original container gumps, for grid container settings visit the TazUO section";
            public string CharacterBackpackStyle { get; set; } = "Character backpack style";
            public string BackpackOpt_Default { get; set; } = "Default";
            public string BackpackOpt_Suede { get; set; } = "Suede";
            public string BackpackOpt_PolarBear { get; set; } = "Polar bear";
            public string BackpackOpt_GhoulSkin { get; set; } = "Ghoul skin";
            public string ContainerScale { get; set; } = "Container scale";
            public string AlsoScaleItems { get; set; } = "Also scale items";
            public string UseLargeContainerGumps { get; set; } = "Use large container gumps";
            public string DoubleClickToLootItemsInsideContainers { get; set; } = "Double click to loot items inside containers";
            public string RelativeDragAndDropItemsInContainers { get; set; } = "Relative drag and drop items in containers";
            public string HighlightContainerOnGroundWhenMouseIsOverAContainerGump { get; set; } = "Highlight container on ground when mouse is over a container gump";
            public string RecolorContainerGumpByWithContainerHue { get; set; } = "Recolor container gump with container hue";
            public string OverrideContainerGumpLocations { get; set; } = "Override container gump locations";
            public string OverridePosition { get; set; } = "Override position";
            public string PositionOpt_NearContainer { get; set; } = "Near container";
            public string PositionOpt_TopRight { get; set; } = "Top right";
            public string PositionOpt_LastDraggedPosition { get; set; } = "Last dragged position";
            public string RememberEachContainer { get; set; } = "Remember each container";
            public string RebuildContainersTxt { get; set; } = "Rebuild containers.txt";
        }

        public class Experimental
        {
            public string DisableDefaultUoHotkeys { get; set; } = "Disable default UO hotkeys";
            public string DisableArrowsNumlockArrowsPlayerMovement { get; set; } = "Disable arrows & numlock arrows(player movement)";
            public string DisableTabToggleWarmode { get; set; } = "Disable tab (toggle warmode)";
            public string DisableCtrlQWMessageHistory { get; set; } = "Disable Ctrl + Q/W (message history)";
            public string DisableRightLeftClickAutoMove { get; set; } = "Disable right + left click auto move";
        }

        public class NamePlates
        {
            public string NewEntry { get; set; } = "New entry";
            public string NameOverheadEntryName { get; set; } = "Name overhead entry name";
            public string DeleteEntry { get; set; } = "Delete entry";
        }

        public class Cooldowns
        {
            public string CustomCooldownBars { get; set; } = "Custom cooldown bars";
            public string PositionX { get; set; } = "Position X";
            public string PositionY { get; set; } = "Position Y";
            public string UseLastMovedBarPosition { get; set; } = "Use last moved bar position";
            public string Conditions { get; set; } = "Conditions";
            public string AddCondition { get; set; } = "+ Add condition";
        }

        public class TazUO
        {
            #region General
            public string GridContainers { get; set; } = "Grid containers";
            public string EnableGridContainers { get; set; } = "Enable grid containers";
            public string GridContainersDefaultToOldStyleView { get; set; } = "Open new containers in the original view";
            public string GridContainerScale { get; set; } = "Grid container scale";
            public string AlsoScaleItems { get; set; } = "Also scale items";
            public string GridItemBorderOpacity { get; set; } = "Grid item border opacity";
            public string BorderColor { get; set; } = "Border color";
            public string ContainerOpacity { get; set; } = "Container opacity";
            public string BackgroundColor { get; set; } = "Background color";
            public string UseContainersHue { get; set; } = "Use container's hue";
            public string SearchStyle { get; set; } = "Search style";
            public string OnlyShow { get; set; } = "Only show";
            public string Highlight { get; set; } = "Highlight";
            public string EnableContainerPreview { get; set; } = "Enable container preview";
            public string TooltipPreview { get; set; } = "This only works on containers that you have opened, otherwise the client does not have that information yet.";
            public string MakeAnchorable { get; set; } = "Make anchorable";
            public string TooltipGridAnchor { get; set; } = "This will allow grid containers to be anchored to other containers/world map/journal";
            public string ContainerStyle { get; set; } = "Container style";
            public string HideBorders { get; set; } = "Hide borders";
            public string DefaultGridRows { get; set; } = "Default grid rows";
            public string DefaultGridColumns { get; set; } = "Default grid columns";
            public string GridHighlightSettings { get; set; } = "Grid highlight settings";
            public string GridHighlightSize { get; set; } = "Grid highlight size";
            public string GridHighlightProperties { get; set; } = "Show highlighted item properties in tooltip";
            public string GridHighlightShowRuleName { get; set; } = "Show matched rule name in tooltip";
            public string GridDisableTargeting { get; set; } = "Disable Targeting Grid Containers";
            #endregion

            #region Journal
            public string Journal { get; set; } = "Journal";
            public string MaxJournalEntries { get; set; } = "Max journal entries";
            public string JournalOpacity { get; set; } = "Journal opacity";
            public string JournalBackgroundColor { get; set; } = "Background color";
            public string JournalStyle { get; set; } = "Journal style";
            public string JournalHideBorders { get; set; } = "Hide borders";
            public string JournalHideSystemPrefix { get; set; } = "Hide \"System:\" prefix";
            public string HideTimestamp { get; set; } = "Hide timestamp";
            public string JournalAnchor { get; set; } = "Make anchorable";
            #endregion

            #region ModernPaperdoll
            public string ModernPaperdoll { get; set; } = "Modern paperdoll";
            public string EnableModernPaperdoll { get; set; } = "Enable modern paperdoll";
            public string PaperdollHue { get; set; } = "Paperdoll hue";
            public string DurabilityBarHue { get; set; } = "Durability bar hue";
            public string ShowDurabilityBarBelow { get; set; } = "Show durability bar below %";
            public string PaperdollAnchor { get; set; } = "Make anchorable";
            #endregion

            #region Nameplates
            public string Nameplates { get; set; } = "Nameplates";
            public string NameplatesAlsoActAsHealthBars { get; set; } = "Nameplates also act as health bars";
            public string HpOpacity { get; set; } = "HP opacity";
            public string HideNameplatesIfFullHealth { get; set; } = "Hide nameplates if full health";
            public string OnlyInWarmode { get; set; } = "Only in warmode";
            public string BorderOpacity { get; set; } = "Border opacity";
            public string BackgroundOpacity { get; set; } = "Background opacity";
            #endregion

            #region Mobile
            public string Mobiles { get; set; } = "Mobiles";
            public string DamageToSelf { get; set; } = "Damage to self";
            public string DamageToOthers { get; set; } = "Damage to others";
            public string DamageToPets { get; set; } = "Damage to pets";
            public string DamageToAllies { get; set; } = "Damage to allies";
            public string DamageToLastAttack { get; set; } = "Damage to last attack";
            public string DisplayPartyChatOverPlayerHeads { get; set; } = "Display party chat over player heads";
            public string TooltipPartyChat { get; set; } = "If a party member uses party chat their text will also show above their head to you";
            public string OverheadTextWidth { get; set; } = "Overhead text width";
            public string TooltipOverheadText { get; set; } = "This adjusts the maximum width for text over players, setting to 0 will allow it to use any width needed to stay one line";
            public string BelowMobileHealthBarScale { get; set; } = "Below mobile health bar scale";
            public string AutomaticallyOpenHealthBarsForLastAttack { get; set; } = "Automatically open health bars for last attack";
            public string UpdateOneBarAsLastAttack { get; set; } = "Update one bar as last attack";
            public string HiddenPlayerOpacity { get; set; } = "Hidden player opacity";
            public string HiddenPlayerHue { get; set; } = "Hidden player hue";
            public string RegularPlayerOpacity { get; set; } = "Regular player opacity";
            public string AutoFollowDistance { get; set; } = "Auto follow distance";
            public string DisableAutoFollow { get; set; } = "Disable alt click to auto follow";
            public string DisableMouseInteractionsForOverheadText { get; set; } = "Disable mouse interactions for overhead text";
            public string OverridePartyMemberHues { get; set; } = "Override party member body hues with friendly hue";
            public string TurnDelay { get; set; } = "Adjust turn delay";
            #endregion

            #region Misc
            public string Misc { get; set; } = "Misc";
            public string DisableSystemChat { get; set; } = "Disable system chat";
            public string EnableImprovedBuffGump { get; set; } = "Enable improved buff gump";
            public string BuffGumpHue { get; set; } = "Buff gump hue";
            public string MainGameWindowBackground { get; set; } = "Main game window background";
            public string EnableHealthIndicatorBorder { get; set; } = "Enable health indicator border";
            public string OnlyShowBelowHp { get; set; } = "Only show below hp %";
            public string Size { get; set; } = "Size";
            public string SpellIconScale { get; set; } = "Spell icon scale";
            public string DisplayMatchingHotkeysOnSpellIcons { get; set; } = "Display matching hotkeys on spell icons";
            public string HotkeyTextHue { get; set; } = "Hotkey text hue";
            public string EnableGumpOpacityAdjustViaAltScroll { get; set; } = "Enable gump opacity adjust via Alt + Scroll";
            public string EnableAdvancedShopGump { get; set; } = "Enable advanced shop gump";
            public string DisplaySkillProgressBarOnSkillChanges { get; set; } = "Display skill progress bar on skill changes";
            public string TextFormat { get; set; } = "Text format";
            public string EnableSpellIndicatorSystem { get; set; } = "Enable spell indicator system";
            public string ImportFromUrl { get; set; } = "Import from url";
            public string InputRequestUrl { get; set; } = "Enter the url for the spell config. \n/c[red]This will override your current config.";
            public string Download { get; set; } = "Download";
            public string Cancel { get; set; } = "Cancel";
            public string AttemptingToDownloadSpellConfig { get; set; } = "Attempting to download spell config..";
            public string SuccesfullyDownloadedNewSpellConfig { get; set; } = "Succesfully downloaded new spell config.";
            public string FailedToDownloadTheSpellConfigExMessage { get; set; } = "Failed to download the spell config. ({0})";
            public string AlsoCloseAnchoredHealthbarsWhenAutoClosingHealthbars { get; set; } = "Also close anchored healthbars when auto closing healthbars";
            public string EnableAutoResyncOnHangDetection { get; set; } = "Enable auto resync on hang detection";
            public string PlayerOffsetX { get; set; } = "Player Offset X";
            public string PlayerOffsetY { get; set; } = "Player Offset Y";
            public string UseLandTexturesWhereAvailable { get; set; } = "Use land textures where available(Experimental)";
            public string SOSGumpID { get; set; } = "SOS Gump ID";
            public string UseWASDMovement { get; set; } = "Use WASD movement instead of arrow keys";
            public string ApplyBorderCaveTiles { get; set; } = "Apply a border to cave tile art";
            public string ForcedHouseTransparencyLevel { get; set; } = "Forced house transparency";
            public string EnableHouseTransparency { get; set; } = "Enable forced house transparency";
            public string HouseTransparencyTileHue { get; set; } = "House transparency tile hue";
            public string EnableASyncMapLoading { get; set; } = "Enable ASync map loading";
            public string ForceManagedZlib { get; set; } = "Force using a managed zlib";
            #endregion

            #region Tooltips
            public string Tooltips { get; set; } = "Tooltips";
            public string AlignTooltipsToTheLeftSide { get; set; } = "Align tooltips to the left side";
            public string AlignMobileTooltipsToCenter { get; set; } = "Align mobile tooltips to center";
            public string BackgroundHue { get; set; } = "Background hue";
            public string HeaderFormatItemName { get; set; } = "Header format(Item name)";
            public string TooltipOverrideSettings { get; set; } = "Tooltip override settings";
            public string ForcedTooltips { get; set; } = "Force tooltips on pre-tooltip servers";
            #endregion

            #region Fontsettings
            public string FontSettings { get; set; } = "Font settings";
            public string TtfFontBorder { get; set; } = "TTF Font border";
            public string InfobarFont { get; set; } = "Infobar font";
            public string SharedSize { get; set; } = "Size";
            public string SystemChatFont { get; set; } = "System chat font";
            public string TooltipFont { get; set; } = "Tooltip font";
            public string OverheadFont { get; set; } = "Overhead font";
            public string JournalFont { get; set; } = "Journal font";
            public string NameplateFont { get; set; } = "Nameplate font";
            public string Optionsfont { get; set; } = "Options menu font";
            #endregion

            #region Controller
            public string Controller { get; set; } = "Controller";
            public string MouseSesitivity { get; set; } = "Mouse Sensitivity";
            public string EnableController { get; set; } = "Enable controller input";
            #endregion

            #region SettingsTransfer
            public string SettingsTransfers { get; set; } = "Settings transfers";
            public string SettingsWarning { get; set; } = "/es/c[red]! Warning !/cd\n" +
                "This will override other character's profile options!\n" +
                "This is not reversable!\n" +
                "You have {0} other profiles that will may overridden with the settings in this profile.\n\n" +
                "This will not override: Macros, skill groups, info bar, grid container data, or gump saved positions.";
            public string OverrideAll { get; set; } = "Override {0} other profiles with this one.";
            public string OverrideAllMacros { get; set; } = "Override {0} other profile's macros with this one.";
            public string OverrideSuccess { get; set; } = "{0} profiles overriden.";
            public string OverrideSame { get; set; } = "Override {0} other profiles on this same server with this one.";
            public string SetAsDefault { get; set; } = "Set this profile as the default for new characters.";
            public string SetMacrosAsDefault { get; set; } = "Set this profile's macros as the default for new characters.";
            public string SetAsDefaultSuccess { get; set; } = "This profile is now the default for new characters.";
            public string SetMacrosAsDefaultSuccess { get; set; } = "This profile's macros are now the default for new characters.";

            #endregion

            #region GumpScaling
            public string GumpScaling { get; set; } = "Gump scaling";
            public string ScalingInfo { get; set; } = "Some of these settings may only take effect after closing and reopening. Visual bugs may occur until the gump is closed and reopened.";
            public string PaperdollGump { get; set; } = "Paperdoll Gump";
            public string GlobalScaling { get; set; } = "Global scale";
            public string GlobalScale { get; set; } = "Scale";
            #endregion

            public string AutoLoot { get; set; } = "Autoloot";
            public string AutoLootEnable { get; set; } = "Enable auto loot";
            public string ScavengerEnable { get; set; } = "Enable scavenger";
            public string AutoLootProgessBarEnable { get; set; } = "Show progress bar while looting";
            public string AutoLootHumanCorpses { get; set; } = "Loot human corpses? (Potentially player corpses)";

            public string AutoSellMenu { get; set; } = "Auto Sell";
            public string AutoSellEnable { get; set; } = "Enable auto sell feature";
            public string AutoSellMaxUniques { get; set; } = "Maximum unique items per transaction";
            public string AutoSellMaxUniquesTooltip { get; set; } = "This is the maximum number of unique items that will be sold at once. A value of 0 means unlimited. A stack of items counts as one towards this limit. Some servers block transactions that sell too many unique items.";
            public string AutoSellMaxItems { get; set; } = "Maximum total items per transaction";
            public string AutoSellMaxItemsTooltip { get; set; } = "This is the maximum number of items that will be sold at once. A value of 0 means unlimited. Some servers block transactions that sell too many items.";

            public string AutoBuyMenu { get; set; } = "Auto Buy";
            public string AutoBuyEnable { get; set; } = "Enable auto buy feature";
            public string GraphicChangeFilter { get; set; } = "Graphic Filter";
            public string Hotkeys { get; set; } = "Hotkeys";


            #region VoiceRecognition
            public string VoiceRecognition { get; set; } = "Voice Recognition";
            public string VoiceRecognitionEnable { get; set; } = "Enable voice recognition";
            public string VoiceModelPath { get; set; } = "Vosk model path";
            public string VoiceModelPathTooltip { get; set; } = "Path to a Vosk speech model directory or .zip file. Download models from alphacephei.com/vosk/models - zip files will be auto-extracted to the vosk/ folder.";
            public string VoiceRecognitionStatus { get; set; } = "Status: {0}";
            public string VoiceStatusReady { get; set; } = "Ready";
            public string VoiceStatusNotInitialized { get; set; } = "Not initialized - set model path first";
            public string VoiceStatusListening { get; set; } = "Listening...";
            public string VoiceApplyModel { get; set; } = "Apply model path";
            public string VoiceCreateMacro { get; set; } = "Create macro button";
            #endregion

            #region VisibileLayers
            public string VisibleLayers { get; set; } = "Visible Layers";
            public string VisLayersInfo { get; set; } = "These settings are to hide layers on in-game mobiles. Check the box to hide that layer.";
            public string OnlyForYourself { get; set; } = "Only for yourself";
            public string HiddenLayersEnabled { get; set; } = "Enable visible layer system";
            #endregion
        }
    }

    public class ScriptingLanguage
    {
        public string OpenLocation { get; set; } = "Open Location";
        public string OpenLocationFailed { get; set; } = "Failed to open location '{0}'";

        public string ScriptManagerTitle { get; set; } = "Script Manager";
        public string NoGroup { get; set; } = "No group";
        public string Menu { get; set; } = "Menu";
        public string Add_ { get; set; } = "Add +";
        public string SearchHint { get; set; } = "Search...";
        public string Refresh_ { get; set; } = "Refresh";
        public string PublicScriptBrowser { get; set; } = "Public Script Browser";
        public string ScriptRecording { get; set; } = "Script Recording";
        public string ScriptingInfo { get; set; } = "Scripting Info";
        public string PersistentVariables { get; set; } = "Persistent Variables";
        public string RunningScripts_ { get; set; } = "Running Scripts";
        public string DisableModuleCache { get; set; } = "Disable module cache";
        public string Expand { get; set; } = "[-]";
        public string Collapse { get; set; } = "[+]";
        public string More { get; set; } = "...";
        public string Stop { get; set; } = "Stop";
        public string Play_ { get; set; } = "Play";
        public string EditConstants { get; set; } = "Edit Constants";
        public string Rename { get; set; } = "Rename";
        public string Edit_ { get; set; } = "Edit";
        public string EditExternally { get; set; } = "Edit Externally";
        public string AutostartAllChars { get; set; } = "Autostart on all chars";
        public string AutostartThisChar { get; set; } = "Autostart for this char";
        public string CreateMacroButton { get; set; } = "Create Macro Button";
        public string Delete_ { get; set; } = "Delete";
        public string DeleteScript { get; set; } = "Delete Script";
        public string DeleteScriptConfirm { get; set; } = "Are you sure you want to delete '{0}'?\nThis action cannot be undone.";
        public string RenameGroup { get; set; } = "Rename Group";
        public string NewScript { get; set; } = "New Script";
        public string NewGroup { get; set; } = "New Group";
        public string DeleteGroup { get; set; } = "Delete Group";
        public string DeleteGroupConfirm { get; set; } = "Delete group '{0}'?\nThis will permanently delete the folder and ALL scripts inside it.";
        public string EnterScriptName { get; set; } = "Enter a name for this script:";
        public string NewScriptTitle { get; set; } = "New Script";
        public string EnterGroupName { get; set; } = "Enter a name for this group:";
        public string NewGroupTitle { get; set; } = "New Group";
        public string NewNameForScript { get; set; } = "New name for '{0}':";
        public string RenameScriptTitle { get; set; } = "Rename Script";
        public string NewNameForGroup { get; set; } = "New name for group '{0}':";
        public string RenameGroupTitle { get; set; } = "Rename Group";
        public string AutostartAllCharsTooltip { get; set; } = "Autostart: All characters";
        public string AutostartThisCharTooltip { get; set; } = "Autostart: This character";
        public string InvalidScriptName { get; set; } = "Invalid script name.";
        public string InvalidTargetDirectory { get; set; } = "Invalid target directory.";
        public string CreatedScript { get; set; } = "Created script '{0}'";
        public string ScriptAlreadyExists { get; set; } = "A script named '{0}' already exists.";
        public string AccessDenied { get; set; } = "Access denied.";
        public string FileOperationFailed { get; set; } = "File operation failed: {0}";
        public string ErrorCreatingScript { get; set; } = "Error creating script: {0}";
        public string InvalidGroupName { get; set; } = "Invalid group name.";
        public string InvalidGroupLocation { get; set; } = "Invalid group location.";
        public string CreatedGroup { get; set; } = "Created group '{0}'";
        public string DirectoryOperationFailed { get; set; } = "Directory operation failed: {0}";
        public string ErrorCreatingGroup { get; set; } = "Error creating group: {0}";
        public string FileAlreadyExists { get; set; } = "A file named '{0}' already exists.";
        public string ErrorRenamingScript { get; set; } = "Error renaming script: {0}";
        public string GroupAlreadyExists { get; set; } = "A group named '{0}' already exists.";
        public string SourceGroupNotFound { get; set; } = "Source group '{0}' not found.";
        public string RenamedGroup { get; set; } = "Renamed group '{0}' to '{1}'";
        public string DirectoryNotFound { get; set; } = "Directory not found.";
        public string ErrorRenamingGroup { get; set; } = "Error renaming group: {0}";
        public string DeletedScript { get; set; } = "Deleted script '{0}'";
        public string ErrorDeletingScript { get; set; } = "Error deleting script: {0}";
        public string GroupNotFound { get; set; } = "Group '{0}' not found";
        public string DeletedGroup { get; set; } = "Deleted group '{0}' and all its contents";
        public string DeleteOperationFailed { get; set; } = "Delete operation failed: {0}";
        public string ErrorDeletingGroup { get; set; } = "Error deleting group: {0}";

        public string ScriptBrowserTitle { get; set; } = "Public Script Browser";
        public string LoadingRepositoryContents { get; set; } = "Loading repository contents...";
        public string Loading { get; set; } = "Loading...";
        public string ClosePreview { get; set; } = "Close Preview";
        public string Retry { get; set; } = "Retry";
        public string View { get; set; } = "View";
        public string Download_ { get; set; } = "Download";
        public string OpenLink { get; set; } = "Open Link";
        public string FailedToLoadScripts { get; set; } = "Failed to load scripts: {0}";
        public string InvalidScriptFilenameNoChars { get; set; } = "Invalid script filename: {0}. Filename contains invalid characters or path separators.";
        public string InvalidScriptFilename { get; set; } = "Invalid script filename: {0}. Filename contains invalid characters.";
        public string SecurityErrorScriptPath { get; set; } = "Security error: Script path must be within the scripts directory.";
        public string GeneratedPathInvalid { get; set; } = "Security error: Generated path is invalid.";
        public string TooManyDuplicateFiles { get; set; } = "Too many duplicate files. Please clean up your scripts directory.";
        public string DownloadedScript { get; set; } = "Downloaded script: {0}";
        public string ErrorSavingScript { get; set; } = "Error saving script: {0} - {1}";
        public string ErrorLoadingScriptBrowser { get; set; } = "Error loading script: {0}";
        public string ErrorLoadingFile { get; set; } = "Error loading file: {0}";

        public string SaveChanges { get; set; } = "Save Changes";
        public string FileTooLargeToEdit { get; set; } = "File too large to edit!";

        public string ScriptErrorTitle { get; set; } = "Script Error {0}";
        public string ScriptErrorHeader { get; set; } = "Your script encountered an error, here's what we know:";
        public string ClickToCopyToClipboard { get; set; } = "Click to copy to clipboard";
        public string CopiedErrorToClipboard { get; set; } = "Copied error to clipboard.";
        public string FileLineFormat { get; set; } = "File: {0}  |  Line: {1}";

        public string PersistentVarsTitle { get; set; } = "Persistent Variables Manager";
        public string Scope { get; set; } = "Scope:";
        public string ScopeCharacter { get; set; } = "Character";
        public string ScopeAccount { get; set; } = "Account";
        public string ScopeServer { get; set; } = "Server";
        public string ScopeGlobal { get; set; } = "Global";
        public string AllServersAndCharacters { get; set; } = "All servers and characters";
        public string FilterVariables { get; set; } = "Filter variables...";
        public string AddNewVariable { get; set; } = "Add New Variable";
        public string NoVariablesFound { get; set; } = "No variables found.";
        public string Key_ { get; set; } = "Key";
        public string Value_ { get; set; } = "Value";
        public string Actions_ { get; set; } = "Actions";
        public string KeyNameHint { get; set; } = "Key name...";
        public string ValueHint { get; set; } = "Value...";
        public string AddVariableTitle { get; set; } = "Add Variable";
        public string AddVariableToScope { get; set; } = "Add new variable to {0} scope:";
        public string KeyLabel { get; set; } = "Key:";
        public string ValueLabel { get; set; } = "Value:";
        public string ConfirmDelete_ { get; set; } = "Confirm Delete";
        public string DeleteVariableConfirm { get; set; } = "Delete variable '{0}'?";

        public string RunningScriptsTitle { get; set; } = "Running Scripts";
        public string NoScriptsCurrentlyRunning { get; set; } = "No scripts currently running";
        public string Unknown { get; set; } = "Unknown";
        public string PathTooltip { get; set; } = "Path: {0}";

        public string Submit { get; set; } = "Submit";
        public string Cancel_ { get; set; } = "Cancel";
        public string EnterYourResponse { get; set; } = "Enter your response...";
        public string ServerPromptTitle { get; set; } = "Server Prompt";
        public string ServerRequestingInput { get; set; } = "The server is requesting input:";
        public string DisableThisPopup { get; set; } = "Disable this popup (use chat instead)";
        public string DisablePopupTooltip { get; set; } = "When checked, server prompts will only be handled through the chat input";
    }

    public class UiCommonsLanguage
    {
        public string DragToResize { get; set; } = "Drag to resize";
        public string MinMaxWindowButtonTooltip { get; set; } = "Minimize or maximize this window";
        public string ResetWindowSizeButtonTooltip { get; set; } = "Reset window size";
        public string Delete { get; set; } = "Delete";
        public string Cancel { get; set; } = "Cancel";
        public string Add { get; set; } = "Add";
        public string Import { get; set; } = "Import";
        public string Export { get; set; } = "Export";
        public string Clear { get; set; } = "Clear";
        public string ClearAllFilters { get; set; } = "Clear All Filters";
        public string Play { get; set; } = "Play";
        public string Refresh { get; set; } = "Refresh";
        public string TestPlay { get; set; } = "Test Play";
        public string PlayAgain { get; set; } = "Play Again";
        public string AddFilter { get; set; } = "Add Filter";
        public string Apply { get; set; } = "Apply";
        public string Edit { get; set; } = "Edit";
        public string Save { get; set; } = "Save";
        public string Remove { get; set; } = "Remove";
        public string Yes { get; set; } = "Yes";
        public string No { get; set; } = "No";
        public string None { get; set; } = "None";
        public string Name { get; set; } = "Name";
        public string Searching { get; set; } = "Searching...";
        public string Ready { get; set; } = "Ready";
        public string Disabled { get; set; } = "Disabled";
        public string Error { get; set; } = "Error";
        public string Ground { get; set; } = "Ground";
        public string Container { get; set; } = "Container";
        public string Location { get; set; } = "Location";
        public string Preview { get; set; } = "Preview";
        public string Hotkey { get; set; } = "Hotkey";
        public string Actions { get; set; } = "Actions";
        public string Filter { get; set; } = "Filter...";
        public string Slot { get; set; } = "Slot {0}";
        public string DaysAgo { get; set; } = "{0}d ago";
        public string HoursAgo { get; set; } = "{0}h ago";
        public string MinutesAgo { get; set; } = "{0}m ago";
        public string JustNow { get; set; } = "Just now";
        public string Hp { get; set; } = "HP";
        public string Mp { get; set; } = "MP";
        public string Sp { get; set; } = "SP";
    }

    public class ErrorsLanguage
    {
        public string CommandNotFound { get; set; } = "Command was not found: {0}";
    }

    public class MapLanguage
    {
        public string Follow { get; set; } = "Follow";
        public string Yourself { get; set; } = "Yourself";
        public string PleaseOpenWorldMapFirst { get; set; } = "Please open the world map first";
        public string PleaseOpenWorldMapGumpFirst { get; set; } = "Please open world map gump first";
        public string MapLoadedInBrowser { get; set; } = "Map loaded in browser";
        public string FailedToLoadMapTexture { get; set; } = "Failed to load map texture";
    }

    public class TopBarGumpLanguage
    {
        public string CommandsEntry { get; set; } = "Client Commands";
        public string Assistant { get; set; } = "Assistant";
        public string LegionScript { get; set; } = "Legion Script";
        public string More { get; set; } = "More +";
        public string ToggleNameplates { get; set; } = "Toggle nameplates";
        public string Tools { get; set; } = "Tools";
        public string SpellQuickCast { get; set; } = "Spell quick cast";
        public string OpenBoatControl { get; set; } = "Open boat control";
        public string NearbyLoot { get; set; } = "Nearby loot";
        public string HealthbarCollector { get; set; } = "Healthbar Collector";
        public string RetrieveGumps { get; set; } = "Retrieve gumps";
        public string XmlGumps { get; set; } = "Xml Gumps";
        public string Reload { get; set; } = "Reload";
    }

    public class AssistantLanguage
    {
        public string VisualConfig { get; set; } = "Visual Config";
        public string DelayConfig { get; set; } = "Delay Config";
        public string CameraSmoothing { get; set; } = "Camera smoothing";
        public string CameraSmoothingTooltip { get; set; } = "Smooth camera following when moving. 0 = instant (classic), 1 = very smooth/floaty.";
        public string HighlightGameObjects { get; set; } = "Highlight game objects";
        public string ShowNameplates { get; set; } = "Show nameplates";
        public string PetScaling { get; set; } = "Pet scaling";
        public string PetScalingTooltip { get; set; } = "Toggle the display of names above characters and NPCs in the game world.";
        public string OutlineMobiles { get; set; } = "Outline mobiles";
        public string MinGumpDragDist { get; set; } = "Min gump drag distance";
        public string MinGumpDragDistTooltip { get; set; } = "How far you need to drag before a gump will move, this helps prevent accidentally dragging instead of clicking.";
        public string GameScale { get; set; } = "Game scale";
        public string GameScaleTooltip { get; set; } = "Adjust the scale of the entire game.";
        public string TurnDelay { get; set; } = "Turn delay";
        public string ObjectDelay { get; set; } = "Object delay";
        public string AutoDelayChecker { get; set; } = "Auto delay checker";
        public string AutoDelayCheckerTooltip { get; set; } = "Run a small test to try to determine the best object delay time.\nThis is an experimental feature, if it doesn't work for you just adjust your delay manually.";
        public string Misc { get; set; } = "Misc";
        public string QueueItemMoves { get; set; } = "Queue item moves";
        public string QueueItemMovesTooltip { get; set; } = "Instead of instantly moving an item, put it in a queue to prevent \"You must wait\" messages.";
        public string QueueObjectUses { get; set; } = "Queue object uses";
        public string QueueObjectUsesTooltip { get; set; } = "Instead of instantly double clicking an item or mobile, put it in a queue to prevent \"You must wait\" messages.";
        public string AutoOpenOwnCorpse { get; set; } = "Auto open own corpse";
        public string AutoOpenOwnCorpseTooltip { get; set; } = "Automatically open your own corpse when you die, even if auto open corpses is disabled.";
        public string AutoUnequipForActions { get; set; } = "Auto unequip for actions";
        public string AutoUnequipForActionsTooltip { get; set; } = "Automatically unequip weapons for spells & potions, then reequip them after.";
        public string DisableWeather { get; set; } = "Disable weather";
        public string DisableWeatherTooltip { get; set; } = "Disable weather effects (rain, snow, storms).";
        public string SetQuickHealSpell { get; set; } = "Set heal spell";
        public string SetQuickCureSpell { get; set; } = "Set cure spell";
        public string QuickSpellTooltip { get; set; } = "These are used on health-bars for party members/pets.";
        public string SingleClickLastTarg { get; set; } = "Single clicking a mobile will set it as last target.";

        public string WindowTitle { get; set; } = "Legion Assistant";
        public string TabGeneral { get; set; } = "General";
        public string TabAgents { get; set; } = "Agents";
        public string TabFilters { get; set; } = "Filters";
        public string TabItemDatabase { get; set; } = "Item Database";
        public string TabMacros { get; set; } = "Macros";
        public string TabSkills { get; set; } = "Skills";
        public string SubTabOptions { get; set; } = "Options";
        public string SubTabHUD { get; set; } = "HUD";
        public string SubTabSpellBar { get; set; } = "Spell Bar";
        public string SubTabTitleBar { get; set; } = "Title Bar";
        public string SubTabSpellIndicators { get; set; } = "Spell Indicators";
        public string SubTabFriends { get; set; } = "Friends";
        public string SubTabPathfinding { get; set; } = "Pathfinding";
        public string SubTabAutoLoot { get; set; } = "Auto Loot";
        public string SubTabDress { get; set; } = "Dress Agent";
        public string SubTabAutoBuy { get; set; } = "Auto Buy";
        public string SubTabAutoSell { get; set; } = "Auto Sell";
        public string SubTabBandage { get; set; } = "Bandage";
        public string SubTabOrganizer { get; set; } = "Organizer";
        public string SubTabStatLock { get; set; } = "Stat Lock";
        public string SubTabGraphics { get; set; } = "Graphics";
        public string SubTabJournalFilter { get; set; } = "Journal Filter";
        public string SubTabSoundFilter { get; set; } = "Sound Filter";
        public string SubTabMusicFilter { get; set; } = "Music Filter";
        public string SubTabSeasonFilter { get; set; } = "Season Filter";

        public AgentsLanguage Agents { get; set; } = new();
        public GraphicReplacementLanguage GraphicReplacement { get; set; } = new();
        public JournalFilterLanguage JournalFilter { get; set; } = new();
        public SoundFilterLanguage SoundFilter { get; set; } = new();
        public MusicFilterLanguage MusicFilter { get; set; } = new();
        public SeasonFilterLanguage SeasonFilter { get; set; } = new();
        public ItemDatabaseLanguage ItemDatabase { get; set; } = new();
        public ItemDetailLanguage ItemDetail { get; set; } = new();
        public MacrosLanguage Macros { get; set; } = new();
        public SkillsLanguage Skills { get; set; } = new();
        public HudLanguage Hud { get; set; } = new();
        public SpellBarLanguage SpellBar { get; set; } = new();
        public TitleBarLanguage TitleBar { get; set; } = new();
        public SpellIndicatorLanguage SpellIndicator { get; set; } = new();
        public FriendsListLanguage FriendsList { get; set; } = new();
        public PathfindingLanguage Pathfinding { get; set; } = new();
    }

    public class AgentsLanguage
    {
        public BandageAgentLanguage Bandage { get; set; } = new();
        public AutoLootAgentLanguage AutoLoot { get; set; } = new();
        public DressAgentLanguage Dress { get; set; } = new();
        public AutoBuyAgentLanguage AutoBuy { get; set; } = new();
        public AutoSellAgentLanguage AutoSell { get; set; } = new();
        public OrganizerAgentLanguage Organizer { get; set; } = new();
    }

    public class BandageAgentLanguage
    {
        public string ProfileNotLoaded { get; set; } = "Profile not loaded";
        public string AutoHealWhenHpBelowThreshold { get; set; } = "Automatically use bandages to heal when HP drops below threshold.";
        public string EnableBandageAgent { get; set; } = "Enable bandage agent";

        public string BandageFriendsCheckbox { get; set; } = "Bandage friends";
        public string BandageFriendsTooltip { get; set; } = "Bandage mobiles in Friends list";

        public string BandageAlliesCheckbox { get; set; } = "Bandage allies";
        public string BandageAlliesTooltip { get; set; } = "Bandage nearby guild/alliance members (notoriety: ally)";

        public string BandagePetsCheckbox { get; set; } = "Bandage pets";
        public string BandagePetsTooltip { get; set; } = "Bandage nearby owned pets";

        public string DisableSelfHealCheckbox { get; set; } = "Disable self heal";
        public string DisableSelfHealTooltip { get; set; } = "When enabled, bandage agent will only heal friends and not yourself";

        public string BandageDelayTooltip { get; set; } = "Delay between bandage attempts in milliseconds (50-30000)";

        public string BandageDelayMsLabel { get; set; } = "Delay (ms)";

        public string UseDexFormulaCheckbox { get; set; } = "Use dex formula";
        public string UseDexFormulaTooltip { get; set; } = "Use the dex formula instead of a set delay";

        public string UseBandageBuffCheckbox { get; set; } = "Use bandaging buff";
        public string UseBandageBuffTooltip { get; set; } = "Use bandaging buff instead of delay";

        public string HealthThresholdSliderLabel { get; set; } = "HP percentage threshold";
        public string UseNewPacketCheckbox { get; set; } = "Use new bandage packet";
        public string BandageIfPoisonedCheckbox { get; set; } = "Bandage if poisoned";
        public string SkipIfHidden { get; set; } = "Skip bandage if hidden";
        public string SkipIfYellowHits { get; set; } = "Skip bandage if yellow hits";
        public string BandageGraphicIdTooltip { get; set; } = "Graphic ID of bandages to use (default: 0x0E21). Accepts hex (0x0E21) or decimal (3617)";
        public string BandageGraphicIdLabel { get; set; } = "Bandage graphic ID:";
    }

    public class AutoLootAgentLanguage
    {
        public string EnableAutoLoot { get; set; } = "Enable Auto Loot";
        public string EnableAutoLootTooltip { get; set; } = "Auto Loot allows you to automatically pick up items from corpses based on configured criteria.";
        public string SetGrabBag { get; set; } = "Set Grab Bag";
        public string TargetContainerToGrabItemsInto { get; set; } = "Target container to grab items into";
        public string ChooseContainerToGrabItemsInto { get; set; } = "Choose a container to grab items into";
        public string OptionsHeader { get; set; } = "Options:";
        public string EnableScavenger { get; set; } = "Enable Scavenger";
        public string EnableScavengerTooltip { get; set; } = "Scavenger option allows picking objects from ground.";
        public string EnableProgressBar { get; set; } = "Enable Progress Bar";
        public string EnableProgressBarTooltip { get; set; } = "Shows a progress bar gump.";
        public string AutoLootHumanCorpses { get; set; } = "Auto Loot Human Corpses";
        public string AutoLootHumanCorpsesTooltip { get; set; } = "Auto loots human corpses.";
        public string HueCorpseAfterProcessing { get; set; } = "Hue Corpse After Processing";
        public string HueCorpseAfterProcessingTooltip { get; set; } = "Hue corpses after processing to make it easier to see if autoloot has processed them.";
        public string CorpseRetryDelayMs { get; set; } = "Corpse retry delay (ms):";
        public string CorpseRetryDelayTooltip { get; set; } = "Milliseconds before a failed corpse is retried. Minimum 1000ms.";
        public string EntriesHeader { get; set; } = "Entries:";
        public string NoEntriesConfigured { get; set; } = "No entries configured.";
        public string ColArt { get; set; } = "Art";
        public string ColGraphic { get; set; } = "Graphic";
        public string ColHue { get; set; } = "Hue";
        public string ColRegex { get; set; } = "Regex";
        public string ColPriority { get; set; } = "Priority";
        public string ColDestination { get; set; } = "Destination";
        public string ColOrder { get; set; } = "Order";
        public string ColActions { get; set; } = "Actions";
        public string NameHint { get; set; } = "Name";
        public string NameTooltip { get; set; } = "Display name for this entry.";
        public string GraphicTooltip { get; set; } = "Item graphic ID. Set to -1 to match any graphic.";
        public string EditRegex { get; set; } = "Edit Regex";
        public string EditRegexDialogTitle { get; set; } = "Edit Regex";
        public string RegexTooltip { get; set; } = "Regex to match against item name and properties.";
        public string SerialHexHint { get; set; } = "Serial (hex)";
        public string DestinationTooltip { get; set; } = "Destination container serial (hex). Leave empty to use grab bag.";
        public string TargetButton { get; set; } = "Target";
        public string TargetButtonTooltip { get; set; } = "Target a container to use as the destination for this entry.";
        public string MoveUp { get; set; } = "Move up";
        public string MoveDown { get; set; } = "Move down";
        public string AddNewEntry { get; set; } = "Add New Entry:";
        public string NameLabel { get; set; } = "Name:";
        public string GraphicLabel { get; set; } = "Graphic:";
        public string HueLabel { get; set; } = "Hue:";
        public string RegexLabel { get; set; } = "Regex:";
        public string AddManualEntry { get; set; } = "Add Manual Entry";
        public string AddFromTarget { get; set; } = "Add from Target";
        public string AddFromTargetTooltip { get; set; } = "Target an item to add it to the loot list.";
        public string ImportFromCharacter { get; set; } = "Import from Character";
        public string ImportFromCharacterTooltip { get; set; } = "Import autoloot configuration from another character.";
        public string NoOtherCharacterConfigs { get; set; } = "No other character configurations found.";
        public string SelectCharacterToImportFrom { get; set; } = "Select a character to import from:";
        public string ItemsCount { get; set; } = "{0} ({1} items)";
        public string ImportTooltip { get; set; } = "Import from clipboard (must have a valid export copied).";
        public string ExportTooltip { get; set; } = "Export your list to clipboard.";
        public string ImportedLootList { get; set; } = "Imported loot list!";
        public string ClipboardNoValidExport { get; set; } = "Your clipboard does not have a valid export copied.";
        public string ExportedLootList { get; set; } = "Exported loot list to your clipboard!";
        public string GraphicIdHint { get; set; } = "Graphic ID";
        public string GraphicIdTooltip { get; set; } = "Graphic (-1 = any)";
        public string HueAnyHint { get; set; } = "Hue (-1 = any)";
        public string RegexOptionalHint { get; set; } = "Regex (optional)";
    }

    public class DressAgentLanguage
    {
        public string DressAgentNotLoaded { get; set; } = "Dress Agent not loaded";
        public string NoItemsConfigured { get; set; } = "No items configured.";
        public string ColSerial { get; set; } = "Serial";
        public string ColName { get; set; } = "Name";
        public string ColLayer { get; set; } = "Layer";
        public string ColActions { get; set; } = "Actions";
        public string RemoveThisItem { get; set; } = "Remove this item";
        public string DressConfigurations { get; set; } = "Dress Configurations";
        public string AddConfiguration { get; set; } = "Add Configuration";
        public string ConfigNameFormat { get; set; } = "Config {0}";
        public string ConfigItemsFormat { get; set; } = "{0} ({1} items)";
        public string CharacterTooltipFormat { get; set; } = "Character: {0}";
        public string SelectConfigToViewDetails { get; set; } = "Select a configuration to view details";
        public string NameLabel { get; set; } = "Name:";
        public string Dress { get; set; } = "Dress";
        public string DressingFromConfig { get; set; } = "Dressing from config: {0}";
        public string Undress { get; set; } = "Undress";
        public string UndressingFromConfig { get; set; } = "Undressing from config: {0}";
        public string CreateDressMacro { get; set; } = "Create Dress Macro";
        public string CreatedDressMacro { get; set; } = "Created Dress Macro: {0}";
        public string CreateUndressMacro { get; set; } = "Create Undress Macro";
        public string CreatedUndressMacro { get; set; } = "Created Undress Macro: {0}";
        public string UseKREquipPacket { get; set; } = "Use KR Equip Packet (faster)";
        public string UseKREquipPacketTooltip { get; set; } = "Uses KR equip/unequip packets for faster operation";
        public string UndressBagSettings { get; set; } = "Undress Bag Settings";
        public string SetUndressBag { get; set; } = "Set Undress Bag";
        public string SelectContainerForUndressedItems { get; set; } = "Select container for undressed items";
        public string UndressBagSet { get; set; } = "Undress bag set to {0}";
        public string OnlyItemsCanBeSelected { get; set; } = "Only items can be selected!";
        public string CurrentSerialFormat { get; set; } = "Current: ({0})";
        public string DefaultYourBackpack { get; set; } = "Default: Your backpack";
        public string ItemsToDressUndress { get; set; } = "Items to Dress/Undress";
        public string AddCurrentlyEquipped { get; set; } = "Add Currently Equipped";
        public string AddedCurrentlyEquippedItems { get; set; } = "Added currently equipped items to config";
        public string TargetItemToAdd { get; set; } = "Target Item to Add";
        public string TargetItemToAddPrint { get; set; } = "Target an item to add to this config";
        public string AddedItem { get; set; } = "Added item: {0}";
        public string OnlyItemsCanBeAdded { get; set; } = "Only items can be added!";
        public string ClearAllItems { get; set; } = "Clear All Items";
        public string ClearedAllItems { get; set; } = "Cleared all items from config";

        public string Usage { get; set; } = "Usage: -dressagent <dress|undress> \"<config name>\"";
        public string ConfigNotFound { get; set; } = "Dress config '{0}' not found";
    }

    public class AutoBuyAgentLanguage
    {
        public string ProfileNotLoaded { get; set; } = "Profile not loaded";
        public string EnableAutoBuy { get; set; } = "Enable Auto Buy";
        public string IncludeSubContainers { get; set; } = "Include sub containers?";
        public string IncludeSubContainersTooltip { get; set; } = "This will also count items inside containers in your backpack (Containers that have not been opened yet may not have an accurate count of contents).";
        public string OptionsHeader { get; set; } = "Options:";
        public string MaxTotalItems { get; set; } = "Max total items";
        public string MaxUniqueItems { get; set; } = "Max unique items";
        public string EntriesHeader { get; set; } = "Entries:";
        public string NoEntriesConfigured { get; set; } = "No entries configured.";
        public string ColArt { get; set; } = "Art";
        public string ColGraphic { get; set; } = "Graphic";
        public string ColHue { get; set; } = "Hue";
        public string ColMaxAmount { get; set; } = "Max Amount";
        public string ColRestockUpTo { get; set; } = "Restock Up To";
        public string ColMaxPrice { get; set; } = "Max Price";
        public string ColEnabled { get; set; } = "Enabled";
        public string ColActions { get; set; } = "Actions";
        public string SetToZeroUnlimited { get; set; } = "Set to 0 for unlimited.";
        public string RestockTooltip { get; set; } = "Amount to restock up to when buying (0 = disabled).";
        public string MaxPriceTooltip { get; set; } = "Maximum price per item (0 = no limit).";
        public string GraphicIdHint { get; set; } = "Graphic ID";
        public string HueAnyHint { get; set; } = "Hue (-1=any)";
        public string MaxAmountHint { get; set; } = "Max Amount (0=unlimited)";
        public string RestockUpToHint { get; set; } = "Restock Up To";
        public string MaxPriceHint { get; set; } = "Max Price (0=no limit)";
        public string GraphicLabel { get; set; } = "Graphic:";
        public string HueLabel { get; set; } = "Hue:";
        public string MaxAmountLabel { get; set; } = "Max Amount:";
        public string RestockUpToLabel { get; set; } = "Restock Up To:";
        public string MaxPriceLabel { get; set; } = "Max Price:";
        public string AddNewEntry { get; set; } = "Add New Entry:";
        public string AddManualEntry { get; set; } = "Add Manual Entry";
        public string AddFromTarget { get; set; } = "Add from Target";
        public string TargetItemToAdd { get; set; } = "Target item to add";
        public string AddFromTargetTooltip { get; set; } = "Target an item to add it to the buy list.";
        public string ImportTooltip { get; set; } = "Import from clipboard (must have a valid export copied).";
        public string ExportTooltip { get; set; } = "Export your list to clipboard.";
        public string ImportedBuyList { get; set; } = "Imported buy list!";
        public string ClipboardNoValidExport { get; set; } = "Your clipboard does not have a valid export copied.";
        public string ExportedBuyList { get; set; } = "Exported buy list to your clipboard!";
    }

    public class AutoSellAgentLanguage
    {
        public string ProfileNotLoaded { get; set; } = "Profile not loaded";
        public string EnableAutoSell { get; set; } = "Enable Auto Sell";
        public string OptionsHeader { get; set; } = "Options:";
        public string MaxTotalItems { get; set; } = "Max total items";
        public string MaxUniqueItems { get; set; } = "Max unique items";
        public string EntriesHeader { get; set; } = "Entries:";
        public string NoEntriesConfigured { get; set; } = "No entries configured.";
        public string ColArt { get; set; } = "Art";
        public string ColGraphic { get; set; } = "Graphic";
        public string ColHue { get; set; } = "Hue";
        public string ColMaxAmount { get; set; } = "Max Amount";
        public string ColMinOnHand { get; set; } = "Min on Hand";
        public string ColEnabled { get; set; } = "Enabled";
        public string ColActions { get; set; } = "Actions";
        public string SetToZeroUnlimited { get; set; } = "Set to 0 for unlimited.";
        public string MinOnHandTooltip { get; set; } = "Minimum amount to keep on hand (0 = disabled).";
        public string GraphicIdHint { get; set; } = "Graphic ID";
        public string HueAnyHint { get; set; } = "Hue (-1=any)";
        public string MaxAmountHint { get; set; } = "Max Amount (0=unlimited)";
        public string MinOnHandHint { get; set; } = "Min on Hand (0=disabled)";
        public string GraphicLabel { get; set; } = "Graphic:";
        public string HueLabel { get; set; } = "Hue:";
        public string MaxAmountLabel { get; set; } = "Max Amount:";
        public string MinOnHandLabel { get; set; } = "Min on Hand:";
        public string AddNewEntry { get; set; } = "Add New Entry:";
        public string AddManualEntry { get; set; } = "Add Manual Entry";
        public string AddFromTarget { get; set; } = "Add from Target";
        public string TargetItemToAdd { get; set; } = "Target item to add";
        public string AddFromTargetTooltip { get; set; } = "Target an item to add it to the sell list.";
        public string AddFromContainer { get; set; } = "Add from Container";
        public string TargetContainerToAddAllItems { get; set; } = "Target a container to add all its items";
        public string AddedItemsFromContainer { get; set; } = "Added {0} item(s) from container.";
        public string AddFromContainerTooltip { get; set; } = "Target a container to add all its items to the sell list.";
        public string ClearAll { get; set; } = "Clear All";
        public string ClearAllTooltip { get; set; } = "Remove all entries from the sell list.";
        public string ImportTooltip { get; set; } = "Import from clipboard (must have a valid export copied).";
        public string ExportTooltip { get; set; } = "Export your list to clipboard.";
        public string ImportedSellList { get; set; } = "Imported sell list!";
        public string ClipboardNoValidExport { get; set; } = "Your clipboard does not have a valid export copied.";
        public string ExportedSellList { get; set; } = "Exported sell list to your clipboard!";
    }

    public class OrganizerAgentLanguage
    {
        public string NoItemsConfigured { get; set; } = "No items configured.";
        public string ColArt { get; set; } = "Art";
        public string ColHue { get; set; } = "Hue";
        public string ColAmount { get; set; } = "Amount";
        public string ColDestination { get; set; } = "Destination";
        public string ColEnabled { get; set; } = "Enabled";
        public string ColActions { get; set; } = "Actions";
        public string GraphicTooltip { get; set; } = "Graphic: {0}";
        public string AmountTooltip { get; set; } = "Amount to move. Takes into account items already in destination.\n(0 = move all)";
        public string PerItemDestination { get; set; } = "Per-item destination";
        public string ClearAndUseConfigDestination { get; set; } = "Clear and use config destination";
        public string Config { get; set; } = "Config";
        public string UsingConfigDestination { get; set; } = "Using configuration's destination";
        public string Set { get; set; } = "Set";
        public string SelectDestinationContainerForItem { get; set; } = "Select [DESTINATION] Container for this item";
        public string PerItemDestinationSet { get; set; } = "Per-item destination set to {0}";
        public string OnlyItemsCanBeSelected { get; set; } = "Only items can be selected!";
        public string SetPerItemDestination { get; set; } = "Set per-item destination";
        public string DeleteThisItem { get; set; } = "Delete this item";
        public string AddOrganizer { get; set; } = "Add Organizer";
        public string List { get; set; } = "List";
        public string EnabledItemsCount { get; set; } = "{0} enabled items";
        public string SelectOrganizerToViewDetails { get; set; } = "Select an organizer to view details";
        public string Enabled { get; set; } = "Enabled";
        public string NameLabel { get; set; } = "Name:";
        public string RunOrganizer { get; set; } = "Run Organizer";
        public string Duplicate { get; set; } = "Duplicate";
        public string CreateMacro { get; set; } = "Create Macro";
        public string CreatedOrganizerMacro { get; set; } = "Created Organizer Macro: {0}";
        public string ImportTooltip { get; set; } = "Import from clipboard (must have a valid export copied).";
        public string ExportTooltip { get; set; } = "Export this organizer to clipboard.";
        public string ClipboardNoValidExport { get; set; } = "Your clipboard does not have a valid export copied.";
        public string ExportedOrganizer { get; set; } = "Exported organizer to your clipboard!";
        public string ContainerSettings { get; set; } = "Container Settings:";
        public string SetSourceContainer { get; set; } = "Set Source Container";
        public string SelectSourceContainer { get; set; } = "Select [SOURCE] Container";
        public string SourceContainerSet { get; set; } = "Source container set to 0x{0} ({1})";
        public string SetDestinationContainer { get; set; } = "Set Destination Container";
        public string SelectDestinationContainer { get; set; } = "Select [DESTINATION] Container";
        public string DestinationContainerSet { get; set; } = "Destination container set to 0x{0} ({1})";
        public string SourceLabelFormat { get; set; } = "Source: (0x{0})";
        public string SourceYourBackpack { get; set; } = "Source: Your backpack";
        public string DestinationLabelFormat { get; set; } = "Destination: (0x{0})";
        public string DestinationNotSet { get; set; } = "Destination: Not set";
        public string ItemsToOrganize { get; set; } = "Items to Organize:";
        public string GraphicHexHint { get; set; } = "Graphic (hex, e.g. 0EED)";
        public string HueAnyHint { get; set; } = "Hue (-1 = any)";
        public string TargetItemToAdd { get; set; } = "Target Item to Add";
        public string AddedItemGraphic { get; set; } = "Added item: Graphic {0}, Hue {1}";
        public string OnlyItemsCanBeAdded { get; set; } = "Only items can be added!";
        public string AddItemManually { get; set; } = "Add Item Manually";
        public string GraphicLabel { get; set; } = "Graphic:";
        public string GraphicLabelTooltip { get; set; } = "Hex value, e.g. 0EED.";
        public string HueLabel { get; set; } = "Hue:";
        public string HueLabelTooltip { get; set; } = "Set to -1 to match any hue.";
        public string ManualEntry { get; set; } = "Manual Entry:";

        public string OrganizerNotFound { get; set; } = "Organizer '{0}' not found.";
        public string TargetSourceContainer { get; set; } = "Target the source container for organizer '{0}'.";
        public string SourceContainerSetPrint { get; set; } = "Source container for organizer '{0}' set.";
        public string NotAValidContainer { get; set; } = "That doesn't appear to be a valid container.";
        public string NoOrganizersConfigured { get; set; } = "No organizers configured.";
        public string AvailableOrganizers { get; set; } = "Available organizers ({0}):";
        public string OrganizerListItem { get; set; } = "  {0}: '{1}' ({2}, {3} item types, destination: {4:X})";
        public string EnabledStatus { get; set; } = "enabled";
        public string DisabledStatus { get; set; } = "disabled";
        public string CannotFindBackpack { get; set; } = "Cannot find player backpack.";
        public string CannotFindSourceContainer { get; set; } = "Cannot find source container for organizer '{0}'.";
        public string CannotFindDestContainerUsingBackpack { get; set; } = "Cannot find destination container for organizer '{0}'. Using backpack as default.";
        public string CannotFindDestContainerSerial { get; set; } = "Cannot find destination container {0:X}. Using backpack as default.";
        public string NoItemsWereOrganized { get; set; } = "No items were organized.";
        public string OrganizerIndexOutOfRange { get; set; } = "Organizer index {0} is out of range. Available organizers: 0-{1}";
        public string OrganizingItems { get; set; } = "Organizing {0} items from '{1}'...";
        public string OrganizerDisabled { get; set; } = "Organizer '{0}' is disabled.";
        public string CannotFindDestContainerWithSerial { get; set; } = "Cannot find destination container for organizer '{0}' (Serial: {1:X})";
        public string NoItemsOrganizedBy { get; set; } = "No items were organized by '{0}'.";
        public string ImportedOrganizer { get; set; } = "Imported organizer '{0}' with {1} items!";
    }

    public class GraphicReplacementLanguage
    {
        public string HeaderDescription { get; set; } = "Replace graphics with other graphics. Mobile = animations, Land = terrain tiles, Static = items/statics.";
        public string NoReplacements { get; set; } = "No replacements configured.";
        public string ColOriginal { get; set; } = "Original";
        public string ColType { get; set; } = "Type";
        public string ColReplacement { get; set; } = "Replacement";
        public string ColPreview { get; set; } = "Preview";
        public string ColNewHue { get; set; } = "New Hue";
        public string ColActions { get; set; } = "Actions";
        public string CycleTypeTooltip { get; set; } = "Click to cycle: Mobile / Land / Static";
        public string DeleteTooltip { get; set; } = "Delete this replacement";
        public string OriginalHint { get; set; } = "Original graphic (e.g. 0x0EED)";
        public string ReplacementHint { get; set; } = "Replacement graphic";
        public string HueHint { get; set; } = "Hue (-1 = unchanged)";
        public string InvalidHue { get; set; } = "Invalid hue: '{0}'. Must be 0-65535, 0x hex, or -1";
        public string OriginalLabel { get; set; } = "Original:";
        public string ReplacementLabel { get; set; } = "Replacement:";
        public string TypeLabel { get; set; } = "Type:";
        public string NewHueLabel { get; set; } = "New Hue:";
        public string NewEntryLabel { get; set; } = "New Entry:";
        public string AddEntry { get; set; } = "Add Entry";
        public string TargetEntity { get; set; } = "Target Entity";
        public string TargetEntityTooltip { get; set; } = "Target an entity to add it to the replacement list";
        public string ApplyAll { get; set; } = "Apply to All Entities";
        public string ApplyAllTooltip { get; set; } = "Reapply graphic replacements to all entities currently in the world";
        public string CurrentReplacements { get; set; } = "Current Graphic Replacements:";
        public string ClipboardInvalid { get; set; } = "Your clipboard does not have a valid export copied.";
        public string Exported { get; set; } = "Exported graphic filters to your clipboard!";
        public string Refreshed { get; set; } = "Refreshed {0} entities with graphic replacements";
    }

    public class JournalFilterLanguage
    {
        public string HeaderDescription { get; set; } = "Journal Filter hides specific messages from the journal. Messages that match exactly will be filtered out.";
        public string NoFilters { get; set; } = "No filters configured.";
        public string ColFilterText { get; set; } = "Filter Text";
        public string ColActions { get; set; } = "Actions";
        public string DeleteTooltip { get; set; } = "Delete this filter";
        public string FilterTextHint { get; set; } = "Filter text (exact match)";
        public string FilterTextLabel { get; set; } = "Filter Text:";
        public string FilterTextTooltip { get; set; } = "Must match the journal entry exactly. Partial matches not supported.";
        public string AddNewFilterLabel { get; set; } = "Add New Filter:";
        public string AddFilterEntry { get; set; } = "Add Filter Entry";
        public string CurrentJournalFilters { get; set; } = "Current Journal Filters:";
        public string ClipboardInvalid { get; set; } = "Your clipboard does not have a valid export copied.";
        public string Exported { get; set; } = "Exported journal filters to your clipboard!";
    }

    public class SoundFilterLanguage
    {
        public string HeaderDescription { get; set; } = "Sound Filter allows you to mute specific in-game sounds by their ID.";
        public string NoSoundsFiltered { get; set; } = "No sounds filtered.";
        public string TotalFiltered { get; set; } = "Total: {0} sound(s) filtered";
        public string ColSoundId { get; set; } = "Sound ID";
        public string ColActions { get; set; } = "Actions";
        public string PlayTooltip { get; set; } = "Test play this sound (bypasses filter)";
        public string DeleteTooltip { get; set; } = "Delete this filter";
        public string RecentlyPlayed { get; set; } = "Recently played:";
        public string SoundIdLabel { get; set; } = "Sound ID: {0} ({1})";
        public string AddFilterTooltip { get; set; } = "Add this sound to the filter list";
        public string PlayAgainTooltip { get; set; } = "Play this sound again";
        public string RefreshTooltip { get; set; } = "Refresh last played sound display";
        public string TipText { get; set; } = "Tip: Play a sound in-game to see its ID above, then click Add Filter.";
        public string NoSoundPlayed { get; set; } = "No sound played yet.";
        public string SoundIdHint { get; set; } = "Sound ID (0-65535)";
        public string TestPlayTooltip { get; set; } = "Test play this sound ID";
        public string SoundIdLabelPlain { get; set; } = "Sound ID:";
        public string SoundIdLabelTooltip { get; set; } = "Enter the numeric ID of the sound to filter (0-65535)";
        public string AddSoundFilterLabel { get; set; } = "Add Sound Filter:";
        public string AddFilterEntry { get; set; } = "Add Filter Entry";
        public string FilteredSoundsLabel { get; set; } = "Filtered Sounds:";
        public string ClipboardEmpty { get; set; } = "Clipboard is empty";
        public string ParseFailed { get; set; } = "Failed to parse clipboard data";
        public string ImportSuccess { get; set; } = "Added {0} sound filter(s) from clipboard";
        public string ImportFailed { get; set; } = "Import failed: {0}";
        public string ExportSuccess { get; set; } = "Exported {0} sound filter(s) to clipboard";
        public string ExportFailed { get; set; } = "Export failed: {0}";
    }

    public class MusicFilterLanguage
    {
        public string HeaderDescription { get; set; } = "Music Filter allows you to mute specific in-game music tracks by their ID.";
        public string NoMusicFiltered { get; set; } = "No music filtered.";
        public string TotalFiltered { get; set; } = "Total: {0} track(s) filtered";
        public string ColMusicId { get; set; } = "Music ID";
        public string ColActions { get; set; } = "Actions";
        public string PlayTooltip { get; set; } = "Test play this track (bypasses filter)";
        public string DeleteTooltip { get; set; } = "Delete this filter";
        public string RecentlyPlayed { get; set; } = "Recently played:";
        public string MusicIdLabel { get; set; } = "Music ID: {0} ({1})";
        public string AddFilterTooltip { get; set; } = "Add this track to the filter list";
        public string PlayAgainTooltip { get; set; } = "Play this track again";
        public string RefreshTooltip { get; set; } = "Refresh last played music display";
        public string TipText { get; set; } = "Tip: Let music play in-game to see its ID above, then click Add Filter.";
        public string NoMusicPlayed { get; set; } = "No music played yet.";
        public string MusicIdHint { get; set; } = "Music ID (0-149)";
        public string TestPlayTooltip { get; set; } = "Test play this music ID";
        public string MusicIdLabelPlain { get; set; } = "Music ID:";
        public string MusicIdLabelTooltip { get; set; } = "Enter the numeric ID of the music track to filter (0-149)";
        public string AddMusicFilterLabel { get; set; } = "Add Music Filter:";
        public string AddFilterEntry { get; set; } = "Add Filter Entry";
        public string FilteredMusicLabel { get; set; } = "Filtered Music:";
        public string ClipboardEmpty { get; set; } = "Clipboard is empty";
        public string ParseFailed { get; set; } = "Failed to parse clipboard data";
        public string ImportSuccess { get; set; } = "Added {0} music filter(s) from clipboard";
        public string ImportFailed { get; set; } = "Import failed: {0}";
        public string ExportSuccess { get; set; } = "Exported {0} music filter(s) to clipboard";
        public string ExportFailed { get; set; } = "Export failed: {0}";
    }

    public class SeasonFilterLanguage
    {
        public string Spring { get; set; } = "Spring";
        public string Summer { get; set; } = "Summer";
        public string Fall { get; set; } = "Fall";
        public string Winter { get; set; } = "Winter";
        public string Desolation { get; set; } = "Desolation";
        public string None { get; set; } = "None";
        public string HeaderDescription { get; set; } = "Override seasons sent by the server. For example, if the server sends Winter, you can display Fall instead.";
        public string ClearAllTooltip { get; set; } = "Remove all season filters and display seasons as sent by the server";
        public string SeasonFiltersLabel { get; set; } = "Season Filters:";
        public string ColWhenServerSends { get; set; } = "When Server Sends";
        public string ColShowAs { get; set; } = "Show As";
        public string CycleTooltip { get; set; } = "Click to cycle season override for {0}";
        public string FooterText { get; set; } = "Click the button to cycle through options. 'None' disables the filter.";
    }

    public class ItemDatabaseLanguage
    {
        public string ProfileNotLoaded { get; set; } = "Profile not loaded";
        public string EnableItemDatabase { get; set; } = "Enable Item Database";
        public string ReadyToSearch { get; set; } = "Ready to search";
        public string NoResultsToDisplay { get; set; } = "No results to display";
        public string ItemDatabaseIsDisabled { get; set; } = "Item Database is disabled.";
        public string NoItemsFound { get; set; } = "No items found";
        public string FoundItemsMaxLimitReached { get; set; } = "Found {0} items (max limit reached)";
        public string FoundItems { get; set; } = "Found {0} items";
        public string SearchOptions { get; set; } = "Search Options:";
        public string ItemNameHint { get; set; } = "Item name (partial match)";
        public string PropertyTextHint { get; set; } = "Property text (partial match)";
        public string GraphicIdTooltip { get; set; } = "Graphic ID to search for (0 = any)";
        public string HueSearchTooltip { get; set; } = "Hue to search for (-1 = any)";
        public string LayerSearchTooltip { get; set; } = "Layer to search for (-1 = any, 0 = on ground)";
        public string ColArt { get; set; } = "Art";
        public string ColName { get; set; } = "Name";
        public string ColHue { get; set; } = "Hue";
        public string ColLayer { get; set; } = "Layer";
        public string ColContainer { get; set; } = "Container";
        public string ColCharacter { get; set; } = "Character";
        public string ColUpdated { get; set; } = "Updated";
        public string ColActions { get; set; } = "Actions";
        public string Details { get; set; } = "Details";
        public string ViewDetailedItemInfo { get; set; } = "View detailed information about this item";
        public string NameLabel { get; set; } = "Name:";
        public string PropertiesLabel { get; set; } = "Properties:";
        public string GraphicIdLabel { get; set; } = "Graphic ID:";
        public string HueLabel { get; set; } = "Hue:";
        public string LayerLabel { get; set; } = "Layer:";
        public string ContainerSerialLabel { get; set; } = "Container Serial:";
        public string OnGroundOnly { get; set; } = "On ground only";
        public string InContainersOnly { get; set; } = "In containers only";
        public string CurrentCharacterOnly { get; set; } = "Current character only";
        public string MaxResults { get; set; } = "Max results";
        public string AdvancedSearch { get; set; } = "Advanced Search";
        public string Search { get; set; } = "Search";
        public string ClearFields { get; set; } = "Clear Fields";
        public string ClearResults { get; set; } = "Clear Results";
        public string SearchCleared { get; set; } = "Search cleared";
        public string ResultsCleared { get; set; } = "Results cleared";
        public string Status { get; set; } = "Status:";
        public string Results { get; set; } = "Results:";
        public string DatabaseMaintenance { get; set; } = "Database Maintenance:";
        public string ClearOldEntriesTooltip { get; set; } = "Delete all database entries older than this many days";
        public string ClearEntriesOlderThan { get; set; } = "Clear entries older than:";
        public string Days { get; set; } = "days";
        public string ClearOldEntries { get; set; } = "Clear Old Entries";
        public string ClearingEntriesOlderThan { get; set; } = "Clearing entries older than {0} days...";
        public string ClearedEntriesOlderThan { get; set; } = "Cleared entries older than {0} days";
        public string ErrorMessage { get; set; } = "Error: {0}";
        public string LocationLabel { get; set; } = "Location";
        public string GroundLabel { get; set; } = "Ground";
        public string ContainerLabel { get; set; } = "Container";
    }

    public class ItemDetailLanguage
    {
        public string WindowTitle { get; set; } = "Item Details \u2014 {0}";
        public string GraphicId { get; set; } = "Graphic ID: {0} (0x{1:X4})";
        public string Hue { get; set; } = "Hue: {0} (0x{1:X4})";
        public string HueDefault { get; set; } = "Hue: Default";
        public string BasicInformation { get; set; } = "Basic Information";
        public string CustomName { get; set; } = "Custom Name: {0}";
        public string Name { get; set; } = "Name: {0} (0x{1:X8})";
        public string Layer { get; set; } = "Layer: {0} ({1})";
        public string LastSeen { get; set; } = "Last seen: {0}";
        public string Character { get; set; } = "Character: {0}";
        public string Server { get; set; } = "Server: {0}";
        public string Location { get; set; } = "Location";
        public string OnGroundAt { get; set; } = "On ground at {0}, {1}";
        public string InContainer { get; set; } = "In container";
        public string Container { get; set; } = "Container: 0x{0:X8}";
        public string RootContainer { get; set; } = "Root Container: 0x{0:X8}";
        public string Properties { get; set; } = "Properties";
        public string NoPropertiesAvailable { get; set; } = "No properties available";
        public string Actions { get; set; } = "Actions";
        public string UseItem { get; set; } = "Use Item";
        public string UseItemTooltip { get; set; } = "Double-click the item to use it";
        public string TakeItem { get; set; } = "Take Item";
        public string TakeItemTooltip { get; set; } = "Move the item to your backpack";
        public string TryToLocate { get; set; } = "Try to Locate";
        public string TryToLocateTooltip { get; set; } = "Create a quest arrow pointing to the item's last known location";
        public string SetCustomName { get; set; } = "Set Custom Name";
        public string ViewContainer { get; set; } = "View Container";
        public string ViewContainerTooltip { get; set; } = "View the container's database entry";
        public string ViewRootContainer { get; set; } = "View Root Container";
        public string ViewRootContainerTooltip { get; set; } = "View the root container's database entry";
        public string Close { get; set; } = "Close";
        public string SetCustomNameTitle { get; set; } = "Set Custom Name";
    }

    public class MacrosLanguage
    {
        public string ProfileNotLoaded { get; set; } = "Profile not loaded";
        public string NoneHotkey { get; set; } = "None";
        public string NoMacros { get; set; } = "No macros.";
        public string ColName { get; set; } = "Name";
        public string ColHotkey { get; set; } = "Hotkey";
        public string ColEdit { get; set; } = "Edit";
        public string SelectMacroToEdit { get; set; } = "Select a macro to edit.";
        public string MacroName { get; set; } = "Macro Name:";
        public string CreateMacroButton { get; set; } = "Create Macro Button";
        public string CreateMacroButtonTooltip { get; set; } = "Create a draggable macro button for this macro";
        public string Actions { get; set; } = "Actions:";
        public string AddAction { get; set; } = "Add Action";
        public string DeleteMacro { get; set; } = "Delete Macro";
        public string DeleteMacroConfirmTitle { get; set; } = "Delete '{0}'?";
        public string DeleteMacroConfirmMessage { get; set; } = "Are you sure you want to delete '{0}'?";
        public string DeleteMacroTooltip { get; set; } = "Permanently delete this macro";
        public string HotkeyLabel { get; set; } = "Hotkey:";
        public string Listening { get; set; } = "Listening...";
        public string Capture { get; set; } = "Capture";
        public string CaptureTooltip { get; set; } = "Click then press a key to assign as hotkey";
        public string Clear { get; set; } = "Clear";
        public string ClearTooltip { get; set; } = "Remove the hotkey from this macro";
        public string RemoveAction { get; set; } = "Remove";
        public string RemoveActionTooltip { get; set; } = "Remove this action";
        public string NoActions { get; set; } = "No actions. Click 'Add Action' to add one.";
        public string Add { get; set; } = "Add";
        public string NewMacro { get; set; } = "New Macro";
        public string MoveUp { get; set; } = "Move Up";
        public string MoveDown { get; set; } = "Move Down";
        public string Import { get; set; } = "Import";
        public string ImportTooltip { get; set; } = "Import macros from clipboard (must have a valid export)";
        public string Export { get; set; } = "Export";
        public string ExportTooltip { get; set; } = "Export all macros to clipboard";
        public string ClipboardNoValidExport { get; set; } = "Your clipboard does not have a valid macro export copied.";
        public string ExportedMacros { get; set; } = "Exported {0} macro(s) to your clipboard!";
        public string HotkeyAlreadyUsed { get; set; } = "Hotkey already used by macro: {0}";
    }

    public class SkillsLanguage
    {
        public string ColUse { get; set; } = "Use";
        public string ColName { get; set; } = "Name";
        public string ColValue { get; set; } = "Value";
        public string ColBase { get; set; } = "Base";
        public string ColCap { get; set; } = "Cap";
        public string ColDelta { get; set; } = "+/-";
        public string ColLock { get; set; } = "Lock";
        public string UseSkillTooltip { get; set; } = "Use {0}";
        public string DoubleClickSkillTooltip { get; set; } = "Double click to create a skill button for {0}";
        public string LockTooltip { get; set; } = "Lock: {0}. Click to cycle.";
        public string AllUp { get; set; } = "All Up";
        public string AllDown { get; set; } = "All Down";
        public string AllLock { get; set; } = "All Lock";
        public string ResetDelta { get; set; } = "Reset +/-";
        public string ResetDeltaTooltip { get; set; } = "Reset the +/- column baseline to current values";
        public string CopyAll { get; set; } = "Copy All";
        public string CopyAllTooltip { get; set; } = "Copy all skills to clipboard as tab-separated text";
        public string ShowGroups { get; set; } = "Show Groups";
        public string Total { get; set; } = "Total: {0:F1} / {1:F1}";
        public string SkillsCopiedToClipboard { get; set; } = "Skills copied to clipboard.";
    }

    public class HudLanguage
    {
        public string HeaderDescription { get; set; } = "Select gump types to toggle visibility when using the Toggle Hud Visible macro.";
        public string SelectAll { get; set; } = "Select All";
        public string DeselectAll { get; set; } = "Deselect All";
        public string ToggleHudNow { get; set; } = "Toggle HUD Now";
        public string ToggleHudNowTooltip { get; set; } = "Immediately toggle the visibility of selected HUD elements";
        public string PaperdollTooltip { get; set; } = "Character paperdoll windows";
        public string WorldMapTooltip { get; set; } = "World map window";
        public string GridContainersTooltip { get; set; } = "Grid-style container windows";
        public string ContainersTooltip { get; set; } = "Traditional container windows";
        public string HealthbarsTooltip { get; set; } = "Health bar windows";
        public string StatusBarTooltip { get; set; } = "Character status windows";
        public string SpellBarTooltip { get; set; } = "Spell bar windows";
        public string JournalTooltip { get; set; } = "Journal/chat windows";
        public string XmlGumpsTooltip { get; set; } = "Server-sent XML gump windows";
        public string NearbyCorpseLootTooltip { get; set; } = "Nearby corpse loot windows";
        public string MacroButtonsTooltip { get; set; } = "Macro button windows";
        public string SkillButtonsTooltip { get; set; } = "Skill button windows";
        public string SkillsMenusTooltip { get; set; } = "Skills menu windows";
        public string TopMenuBarTooltip { get; set; } = "Top menu bar";
        public string DurabilityTrackerTooltip { get; set; } = "Item durability tracker";
        public string BuffBarTooltip { get; set; } = "Buff/debuff status bars";
        public string CounterBarTooltip { get; set; } = "Item counter bars";
        public string InfoBarTooltip { get; set; } = "Information bars";
        public string SpellIconsTooltip { get; set; } = "Spell icon buttons";
        public string NameOverheadGumpTooltip { get; set; } = "Name overhead displays";
        public string ScriptManagerGumpTooltip { get; set; } = "Script manager window";
        public string PlayerCharTooltip { get; set; } = "Player character (your avatar in the game world)";
        public string MouseTooltip { get; set; } = "Mouse cursor";
        public string HealthBarCollectorTooltip { get; set; } = "Health bar collector window";
        public string AbilityButtonsTooltip { get; set; } = "Ability button windows";
        public string DebugGumpTooltip { get; set; } = "Debug information window";
    }

    public class SpellBarLanguage
    {
        public string EnableSpellbar { get; set; } = "Enable spellbar";
        public string EnableSpellbarTooltip { get; set; } = "Enable or disable the spell bar feature";
        public string DisplayHotkeys { get; set; } = "Display hotkeys on spellbar";
        public string DisplayHotkeysTooltip { get; set; } = "Show hotkey assignments on the spell bar buttons";
        public string RowManagement { get; set; } = "Row Management";
        public string AddRow { get; set; } = "Add Row";
        public string AddRowTooltip { get; set; } = "Add a new spell bar row";
        public string RemoveRow { get; set; } = "Remove Row";
        public string RemoveRowTooltip { get; set; } = "Remove the last row. If you have 5 rows, row 5 will be removed.";
        public string PresetManagement { get; set; } = "Preset Management";
        public string PresetNameHint { get; set; } = "Preset name";
        public string SavePreset { get; set; } = "Save Preset...";
        public string SavePresetTooltip { get; set; } = "Save the current spell bar row as a preset";
        public string LoadPreset { get; set; } = "Load Preset...";
        public string LoadPresetTooltip { get; set; } = "Load a saved preset";
        public string NoPresetsAvailable { get; set; } = "No presets available.";
        public string SelectPresetToLoad { get; set; } = "Select a preset to load:";
        public string HotkeyConfiguration { get; set; } = "Hotkey Configuration";
        public string NoneHotkey { get; set; } = "None";
        public string Set { get; set; } = "Set";
        public string Clear { get; set; } = "Clear";
        public string PressAKey { get; set; } = "Press a key...";
        public string NameLabel { get; set; } = "Name:";
        public string ImportPreset { get; set; } = "Import preset";
        public string LockUnlockMovement { get; set; } = "Lock/Unlock spellbar movement";
        public string DeleteRow { get; set; } = "Delete row";
        public string SetRowColor { get; set; } = "Set row color";
        public string MoreOptions { get; set; } = "More options";
        public string RightClickToSetSpell { get; set; } = "Right click to set spell";
        public string SetSpell { get; set; } = "Set spell";
        public string QuickSetSpell { get; set; } = "Quick set spell";
    }

    public class TitleBarLanguage
    {
        public string HeaderDescription { get; set; } = "Configure window title bar to show HP, Mana, and Stamina information.";
        public string EnableTitleBarStats { get; set; } = "Enable title bar stats";
        public string DisplayMode { get; set; } = "Display Mode";
        public string TextModeLabel { get; set; } = "Text  (HP 85/100, MP 42/50, SP 95/100)";
        public string PercentModeLabel { get; set; } = "Percent  (HP 85%, MP 84%, SP 95%)";
        public string ProgressBarModeLabel { get; set; } = "Progress Bar  (HP [||||||    ] MP [||||||    ] SP [||||||    ])";
    }

    public class SpellIndicatorLanguage
    {
        public string ProfileNotLoaded { get; set; } = "Profile not loaded";
        public string SearchSpellsHint { get; set; } = "Search spells...";
        public string NoSpellIndicatorsConfigured { get; set; } = "No spell indicators configured";
        public string AllSpellIndicators { get; set; } = "All Spell Indicators:";
        public string ColId { get; set; } = "ID";
        public string ColName { get; set; } = "Name";
        public string ColPowerWords { get; set; } = "Power Words";
        public string ColCastRange { get; set; } = "Cast Range";
        public string ColCursorSize { get; set; } = "Cursor Size";
        public string ColCastTime { get; set; } = "Cast Time";
        public string SpellConfiguration { get; set; } = "Spell Configuration:";
        public string SpellIdLabel { get; set; } = "Spell ID:";
        public string NameLabel { get; set; } = "Name:";
        public string PowerWordsLabel { get; set; } = "Power Words:";
        public string PowerWordsTooltip { get; set; } = "Power words must be exact, this is the best way we can detect spells.";
        public string CursorSizeLabel { get; set; } = "Cursor Size:";
        public string CursorSizeTooltip { get; set; } = "Area to show around the cursor, for area spells that affect the area near the target.";
        public string CastRangeLabel { get; set; } = "Cast Range:";
        public string CastTimeLabel { get; set; } = "Cast Time:";
        public string MaxDurationLabel { get; set; } = "Max Duration:";
        public string MaxDurationTooltip { get; set; } = "Fallback in case spell detection fails.";
        public string CursorHueLabel { get; set; } = "Cursor Hue:";
        public string RangeHueLabel { get; set; } = "Range Hue:";
        public string IsLinearLabel { get; set; } = "Is Linear:";
        public string IsLinearTooltip { get; set; } = "Used for spells like wall of stone that create a line.";
        public string ShowRangeDuringCastLabel { get; set; } = "Show Range During Cast:";
        public string FreezeWhileCastingLabel { get; set; } = "Freeze While Casting:";
        public string FreezeWhileCastingTooltip { get; set; } = "Prevent yourself from moving and disrupting your spell.";
        public string ExpectTargetCursorLabel { get; set; } = "Expect Target Cursor:";
        public string DeleteSpellConfirmLabel { get; set; } = "Delete '{0}'?";
        public string DeleteSpell { get; set; } = "Delete Spell";
        public string DeleteSpellTooltip { get; set; } = "Delete this spell indicator configuration.";
        public string BackToList { get; set; } = "Back to List";
        public string NewSpellIdHint { get; set; } = "Spell ID (number)";
        public string NewSpellNameHint { get; set; } = "Spell Name";
        public string CreateSpell { get; set; } = "Create Spell";
        public string FillInBothFields { get; set; } = "Please fill in both Spell ID and Name.";
        public string SpellIdMustBeNumber { get; set; } = "Spell ID must be a valid number.";
        public string SpellIdMustBePositive { get; set; } = "Spell ID must be a positive number.";
        public string SpellIdAlreadyExists { get; set; } = "A spell with this ID already exists.";
        public string CreateNewSpellIndicator { get; set; } = "Create a new spell indicator configuration:";
        public string SpellSearchLabel { get; set; } = "Spell search:";
        public string AddNewSpell { get; set; } = "Add New Spell";
        public string EnableSpellIndicators { get; set; } = "Enable Spell Indicators";
        public string EnableSpellIndicatorsTooltip { get; set; } = "Enable visual spell range indicators that show casting range and area of effect for spells.";
    }

    public class FriendsListLanguage
    {
        public string NoFriendsAddedYet { get; set; } = "No friends added yet.";
        public string CurrentFriends { get; set; } = "Current Friends:";
        public string ColSerial { get; set; } = "Serial";
        public string ColName { get; set; } = "Name";
        public string ColDateAdded { get; set; } = "Date Added";
        public string NA { get; set; } = "N/A";
        public string Unknown { get; set; } = "Unknown";
        public string RemovedFromFriendsList { get; set; } = "Removed {0} from friends list";
        public string ManageFriendsList { get; set; } = "Manage your friends list.";
        public string AddByTarget { get; set; } = "Add by Target";
        public string TargetPlayerToAdd { get; set; } = "Target a player to add to friends list";
        public string AddedToFriendsList { get; set; } = "Added {0} to friends list";
        public string CouldNotAddAlreadyInList { get; set; } = "Could not add {0} \u2014 already in friends list";
        public string InvalidTargetMustBePlayer { get; set; } = "Invalid target \u2014 must be a player";
    }

    public class PathfindingLanguage
    {
        public string LongDistancePathfinding { get; set; } = "Long-Distance Pathfinding";
        public string LongDistancePathfindingTooltip { get; set; } = "This is currently in beta.";
        public string PathfindingGenTimeMs { get; set; } = "Pathfinding Gen Time (ms)";
        public string GenTimeTooltip { get; set; } = "Target time in milliseconds for pathfinding cache generation per cycle. Higher values generate cache faster but may cause performance issues.";
        public string CacheProgressNA { get; set; } = "Cache Progress: N/A";
        public string CacheProgressCurrentTotal { get; set; } = "Cache Progress: {0}/{1} chunks ({2:F1}%)";
        public string CacheProgressTooltip { get; set; } = "Current map cache generation progress";
        public string ResetCurrentMapCache { get; set; } = "Reset current map cache";
        public string ResetCurrentMapCacheTooltip { get; set; } = "This will start regeneration of the current map cache.";
        public string PathfindingZLevelDiff { get; set; } = "Pathfinding Z level difference";
        public string ZLevelSliderTooltip { get; set; } = "This is an advanced setting, adjust at your own peril.\nThis adjusts the maximum z level(height) difference between pathfinding nodes.";
    }

    public class TazUOChatLanguage
    {
        public string Title { get; set; } = "TazUO Chat";
        public string NotConnected { get; set; } = "Not connected..";
        public string TryToConnect { get; set; } = "Try to connect";
        public string Channels { get; set; } = "Channels";
        public string ChannelHint { get; set; } = "channel...";
        public string JoinTooltip { get; set; } = "Join or create a channel";
        public string LeaveChannel { get; set; } = "Leave channel";
        public string NoMessagesYet { get; set; } = "No messages yet.";
        public string SelectAChannel { get; set; } = "Select a channel.";
        public string UsersFormat { get; set; } = "Users ({0})";
        public string Users { get; set; } = "Users";
        public string TypeAMessage { get; set; } = "Type a message...";
        public string Send { get; set; } = "Send";
        public string Options { get; set; } = "Options";
        public string AutoConnect { get; set; } = "Auto connect";
    }

    public class JournalGumpLanguage
    {
        public string System { get; set; } = "System";
        public string Objects { get; set; } = "Objects";
        public string Client { get; set; } = "Client";
        public string Guild { get; set; } = "Guild";
    }

    public class WorldMapGumpLanguage
    {
        public string ShowMyCorpse { get; set; } = "Show my Corpse";
        public string OpenWebMap { get; set; } = "Open Web Map (Browser)";
        public string AutoStartWebMap { get; set; } = "Auto start web map";
        public string MakingMapFile { get; set; } = "Please wait, I'm making the map file...";
        public string FailedSaveUserMarkers { get; set; } = "Failed to save user markers";
    }

    public class MapGumpLanguage
    {
        public string Menu { get; set; } = "Menu";
        public string ShowApproxLocation { get; set; } = "Show approximate location on world map";
        public string AddAsMarker { get; set; } = "Add as marker on world map";
        public string CreateArrowToLocation { get; set; } = "Create arrow pointing to location";
        public string TryToPathfind { get; set; } = "Try to pathfind";
        public string Close { get; set; } = "Close";
        public string WrongFacet { get; set; } = "You're on the wrong facet!";
        public string EstimatedLoc { get; set; } = "Estimated loc: {0}, {1}";
    }

    public class GridContainerLanguage
    {
        public string SearchPlaceholder { get; set; } = "Search...";
        public string ClearSearch { get; set; } = "Clear search";
        public string SetLootBag { get; set; } = "Set loot bag";
        public string SetLootBagTooltip { get; set; } = "For double click looting only";
        public string DropHereQuickLootCorpse { get; set; } = "Drop an item here to send it to your backpack.<br><br>Click this icon to enable/disable single-click looting for corpses.<br>   Currently {0}";
        public string DropHereQuickLootContainer { get; set; } = "Drop an item here to send it to your backpack.<br><br>Click this icon to enable/disable single-click loot for this container while it remains open.<br>   Currently {0}";
        public string SortTooltip { get; set; } = "Sort this container.<br>Left click to show sort options<br>Alt + Click to enable auto sort<br>Current sort: {0}<br>Auto sort currently {1}";
        public string SortName { get; set; } = "Name";
        public string SortGraphicHue { get; set; } = "Graphic + Hue";
        public string InfoBarTooltip { get; set; } = "Ctrl + Click to lock an item in place\nAlt + Click to toggle selection for multi-move\nAlt + Double Click to select all similar items\nShift + Click to add an item to your auto loot list\nSort and single click looting can be enabled with the icons on the right side";
        public string OpenOriginalView { get; set; } = "Open Original View";
        public string OpenNewContainersOrigView { get; set; } = "Open New Containers in the Original View";
        public string StackSimilarItems { get; set; } = "Stack Similar Items in the Original View";
        public string OpenHighlightSettings { get; set; } = "Open Grid View Highlight Settings";
        public string AutolootContainer { get; set; } = "Autoloot this container";
        public string RefreshHighlights { get; set; } = "Refresh item highlights";
        public string RenameContainer { get; set; } = "Rename container";
        public string RenameContainerTitle { get; set; } = "Rename Container";
        public string RenameContainerPrompt { get; set; } = "Type in a custom name for this container.";
        public string Save { get; set; } = "Save";
        public string Reset { get; set; } = "Reset";
        public string SortByGraphicHue { get; set; } = "Sort by Graphic + Hue";
        public string SortByName { get; set; } = "Sort by Name";
        public string AddedToAutoLoot { get; set; } = "Added this item to auto loot.";
        public string Enabled { get; set; } = "Enabled";
        public string Disabled { get; set; } = "Disabled";
    }

    public class CounterBarLanguage
    {
        public string SetSpell { get; set; } = "Set spell";
        public string QuickSetSpell { get; set; } = "Quick set spell";
    }
}
