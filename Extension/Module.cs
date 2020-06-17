using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
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
        List<InformationMessage> LoadMessages = new List<InformationMessage>();

        protected override void OnSubModuleLoad()
        {
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            try
            {
                if (!CompatibilityHelper.Instance.CheckGameVersion())
                {
                    LoadMessages.Add(new InformationMessage($"{ModuleId} can not load, expected game version is {CompatibilityHelper.GameVersion}", Colors.Red));
                    AddOptionsMenu(false);
                    return;
                }
                if (!HarmonyHelper.Instance.LoadAssembly())
                {
                    LoadMessages.Add(new InformationMessage($"{ModuleId} can not load, expected Harmony version is {HarmonyHelper.LibVersion}", Colors.Red));
                    AddOptionsMenu(false);
                    return;
                }
                if (!CompatibilityHelper.Instance.CheckOtherModules(ModuleId))
                {
                    LoadMessages.Add(new InformationMessage($"{ModuleId} may encounter problems, unkown mods found", Color.ConvertStringToColor("#FF6A00FF")));
                }
                Instance = this;
                HarmonyHelper.Instance.Initialize(ModuleId);
                Configuration.Instance.Initialize();
                Configuration.Instance.Load(Version);
                ResourceManager.Instance.Load();
                AddOptionsMenu(Loaded = true);
                LoadMessages.Add(new InformationMessage($"{ModuleId} {Version} loaded"));
                Helper.LogFunctionEnd(MethodBase.GetCurrentMethod());
            }
            catch (Exception e)
            {
                HarmonyHelper.Instance.Release();
                Instance = null;
                AddOptionsMenu(false);
                Helper.LogException(MethodBase.GetCurrentMethod(), e);
                LoadMessages.Add(new InformationMessage($"{ModuleId} load failed, please check the logs for more information", Colors.Red));
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            foreach (InformationMessage msg in LoadMessages)
            {
                Helper.DisplayMessage(msg);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            Helper.LogFunctionStart(MethodBase.GetCurrentMethod());
            if (Loaded)
            {
                ResourceManager.Instance.Unload();
                HarmonyHelper.Instance.Module.UnpatchAll();
                HarmonyHelper.Instance.Release();
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
            HarmonyHelper.Instance.Module.UnpatchAll();
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
            HarmonyHelper.Instance.Module.PatchAll();
            foreach (MethodBase m in HarmonyHelper.Instance.Module.GetPatchedMethods())
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

        void AddOptionsMenu(bool enabled)
        {
            if (enabled)
            {
                TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(
                    new InitialStateOption(
                        ModuleId,
                        new TextObject($"{ModuleId}", null),
                        7000,
                        () => ScreenManager.PushScreen(new OptionsScreen()),
                        false));
            }
            else
            {
                TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(
                    new InitialStateOption(
                        ModuleId,
                        new TextObject($"{ModuleId} (not loaded)", null),
                        7000,
                        () => { },
                        true));
            }
        }
    }
}
