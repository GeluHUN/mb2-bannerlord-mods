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
using System.Text;

namespace Extension
{
    class Module : MBSubModuleBase
    {
        public readonly static string ModuleId = "Extension";

        public static Module Instance { get; private set; }

        public ApplicationVersion Version => ModuleInfo.GetModules().First(m => m.Id == ModuleId).Version;

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
        Exception Error;
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
                }
                catch (Exception e)
                {
                    HarmonyModule = null;
                    Instance = null;
                    Error = e;
                }
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (Error != null)
            {
                Helper.DisplayMessage($"Error {Error.Message} during load of {ModuleId}", Colors.Red);
            }
            else if (AreOtherModsInstalled(out string otherMods))
            {
                Helper.DisplayMessage($"Warning, other mods ({otherMods}) are installed, there could be compatibility issues", Color.ConvertStringToColor("#FF6A00FF"));
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
            if (Loaded)
            {
                ResourceManager.Instance.Unload();
                HarmonyModule.UnpatchAll();
                HarmonyModule = null;
                Loaded = false;
                Instance = null;
                Helper.DisplayMessage($"{ModuleId} unloaded");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            if (!Loaded)
            {
                return;
            }
            if (game.GameType is Campaign && starter is CampaignGameStarter campaignGameStarter)
            {
                PatchAll();
                AddCampaignBehaviors(campaignGameStarter);
                AddCampaignModels(campaignGameStarter);
            }
            GameStartEventHandler handler = GameStartEvent;
            handler?.Invoke(game, starter);
        }

        public override void OnGameEnd(Game game)
        {
            if (!Loaded)
            {
                return;
            }
            GameEndEventHandler handler = GameEndEvent;
            handler?.Invoke(game);
            HarmonyModule.UnpatchAll();
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
            if (Loaded == false
                || Helper.TheGame == null
                || Helper.TheGame.CurrentState != Game.State.Running)
            {
                return;
            }
            ApplicationTickEventHandler handler = ApplicationTickEvent;
            handler?.Invoke(dt);
        }

        void PatchAll()
        {
            HarmonyModule.PatchAll();
            foreach (MethodBase m in HarmonyModule.GetPatchedMethods())
            {
                Helper.DisplayMessage($"Patched: {m.DeclaringType.Name}.{m.Name}");
            }
        }

        void AddCampaignModels(CampaignGameStarter campaignGameStarter)
        {
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
                Helper.DisplayMessage($"Behavior {type.Name} loaded");
            }
        }

        void AddCampaignBehaviors(CampaignGameStarter campaignGameStarter)
        {
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
                Helper.DisplayMessage($"Model {type.Name} loaded");
            }
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

        bool AreOtherModsInstalled(out string otherMods)
        {
            bool result = false;
            StringBuilder sb = new StringBuilder();
            foreach (ModuleInfo mod in from m in ModuleInfo.GetModules()
                                       where m.Id != ModuleId
                                             && m.Id != "Native"
                                             && m.Id != "Sandbox"
                                             && m.Id != "SandBoxCore"
                                             && m.Id != "StoryMode"
                                             && m.Id != "CustomBattle"
                                       select m)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(mod.Name);
                result = true;
            }
            otherMods = sb.ToString();
            return result;
        }

        bool CheckVersionCompatibility()
        {
            return (from e in ExpectedModules
                    from m in ModuleInfo.GetModules()
                    where m.Id == e.Key && m.Version == e.Value
                    select m)
                    .Count() == ExpectedModules.Count;
        }
    }
}
