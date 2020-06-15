using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;
using Extension.Config;
using Extension.Config.UI;
using Extension.Utils;
using Extension.Resources;

namespace Extension
{
    class Module : MBSubModuleBase
    {
        public readonly static string ModuleId = "Extension";
        public static Module Instance { get; private set; }
        public static ApplicationVersion Version => ModuleInfo.GetModules().First(m => m.Id == ModuleId).Version;

        public event MissionBehaviourInitializeEventHandler MissionBehaviourInitializeEvent;
        public event ApplicationTickEventHandler ApplicationTickEvent;
        public event GameStartEventHandler GameStartEvent;
        public event GameEndEventHandler GameEndEvent;

        public delegate void MissionBehaviourInitializeEventHandler(Mission mission);
        public delegate void ApplicationTickEventHandler(float dt);
        public delegate void GameStartEventHandler(Game game, IGameStarter starter);
        public delegate void GameEndEventHandler(Game game);

        bool Loaded;
        Harmony HarmonyModule;
        bool ErrorDuringLoad;
        readonly Dictionary<string, ApplicationVersion> ExpectedModules =
            new Dictionary<string, ApplicationVersion>()
            {
                { "Native", ApplicationVersion.FromString("e1.4.2", ApplicationVersionGameType.Singleplayer)},
                { "Sandbox", ApplicationVersion.FromString("e1.4.2", ApplicationVersionGameType.Singleplayer)},
                { "SandBoxCore", ApplicationVersion.FromString("e1.4.2", ApplicationVersionGameType.Singleplayer)},
                { "StoryMode", ApplicationVersion.FromString("e1.4.2", ApplicationVersionGameType.Singleplayer)}
            };

        protected override void OnSubModuleLoad()
        {
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            ErrorDuringLoad = false;
            if (CheckVersionCompatibility())
            {
                try
                {
                    Instance = this;
                    HarmonyModule = new Harmony(ModuleId);
                    Configuration.Instance.Initialize();
                    Configuration.Instance.Load(Version);
                    ResourceManager.Instance.Load();
                    AddOptionsScreen();
                    Loaded = true;
                    Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
                }
                catch (Exception e)
                {
                    HarmonyModule = null;
                    Instance = null;
                    ErrorDuringLoad = true;
                    Helper.LogException(MethodBase.GetCurrentMethod(), e);
                }
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (ErrorDuringLoad)
            {
                Helper.DisplayMessage($"There was an error during the load of {ModuleId}", Colors.Red);
            }
            else if (CheckOtherModsAreInstalled())
            {
                Helper.DisplayMessage($"Warning, other mods are installed, there could be compatibility issues", Color.ConvertStringToColor("#FF6A00FF"));
            }
            else if (Loaded)
            {
                Helper.DisplayMessage($"{ModuleId} {Version} loaded");
            }
            else
            {
                Helper.DisplayMessage($"{ModuleId} NOT loaded, game version is incompatible", Colors.Red);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            if (Loaded)
            {
                ResourceManager.Instance.Unload();
                HarmonyModule.UnpatchAll();
                HarmonyModule = null;
                Loaded = false;
                Instance = null;
                Helper.DisplayMessage($"{ModuleId} unloaded");
            }
            Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
        }

        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            if (!Loaded)
            {
                return;
            }
            if (game.GameType is Campaign && starter is CampaignGameStarter campaignGameStarter)
            {
                Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
                try
                {
                    PatchAll();
                    AddCampaignBehaviors(campaignGameStarter);
                    AddCampaignModels(campaignGameStarter);
                    GameStartEventHandler handler = GameStartEvent;
                    handler?.Invoke(game, starter);
                    Helper.DisplayMessage($"{ModuleId} started");
                    Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
                }
                catch (Exception e)
                {
                    Helper.LogException(MethodBase.GetCurrentMethod(), e);
                }
            }
        }

        public override void OnGameEnd(Game game)
        {
            if (!Loaded)
            {
                return;
            }
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            GameEndEventHandler handler = GameEndEvent;
            handler?.Invoke(game);
            HarmonyModule.UnpatchAll();
            Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            if (!Loaded)
            {
                return;
            }
            MissionBehaviourInitializeEventHandler handler = MissionBehaviourInitializeEvent;
            handler?.Invoke(mission);
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (Loaded == false || Helper.TheGame == null || Helper.TheGame.CurrentState != Game.State.Running)
            {
                return;
            }
            ApplicationTickEventHandler handler = ApplicationTickEvent;
            handler?.Invoke(dt);
        }

        void PatchAll()
        {
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            HarmonyModule.PatchAll();
            foreach (MethodBase m in HarmonyModule.GetPatchedMethods())
            {
                Helper.LogMessage($"Patched: {m.DeclaringType.Name}.{m.Name}");
            }
            Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
        }

        void AddCampaignModels(CampaignGameStarter campaignGameStarter)
        {
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            foreach (Type type in from categroy in Configuration.Instance
                                  from child in categroy
                                  where child is Group
                                  let grp = child as Group
                                  where grp.Enabled == true
                                  from cls in grp.Classes
                                  where cls.IsSubclassOf(typeof(CampaignBehaviorBase))
                                  select cls)
            {
                campaignGameStarter.AddBehavior((CampaignBehaviorBase)Activator.CreateInstance(type));
                Helper.LogMessage($"Behavior {type.Name} loaded");
            }
            Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
        }

        void AddCampaignBehaviors(CampaignGameStarter campaignGameStarter)
        {
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            foreach (Type type in from categroy in Configuration.Instance
                                  from child in categroy
                                  where child is Group
                                  let grp = child as Group
                                  where grp.Enabled == true
                                  from cls in grp.Classes
                                  where cls.IsSubclassOf(typeof(GameModel))
                                  select cls)
            {
                campaignGameStarter.AddModel((GameModel)Activator.CreateInstance(type));
                Helper.LogMessage($"Model {type.Name} loaded");
            }
            Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
        }

        void AddOptionsScreen()
        {
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(
                new InitialStateOption(
                    ModuleId,
                    new TextObject($"{ModuleId}", null),
                    7000,
                    () => ScreenManager.PushScreen(new OptionsScreen()),
                    false));
        }

        bool CheckOtherModsAreInstalled()
        {
            bool result = false;
            foreach (ModuleInfo mod in from m in ModuleInfo.GetModules()
                                       where m.Id != ModuleId
                                             && m.IsSelected
                                             && m.Id != "Native"
                                             && m.Id != "Sandbox"
                                             && m.Id != "SandBoxCore"
                                             && m.Id != "StoryMode"
                                             && m.Id != "CustomBattle"
                                       select m)
            {
                result = true;
                Helper.LogMessage($"Other mod found: {mod.Name} {mod.Version}");
            }
            return result;
        }

        bool CheckVersionCompatibility()
        {
            bool result = (from e in ExpectedModules
                           from m in ModuleInfo.GetModules()
                           where m.Id == e.Key && m.Version == e.Value
                           select m).Count() == ExpectedModules.Count;
            if (result)
            {
                Helper.LogMessage($"{ModuleId} can load, game version is compatible");
            }
            else
            {
                Helper.LogMessage($"{ModuleId} can not load, game version is not compatible");
            }
            return result;
        }
    }
}
