using Extension.Config;
using Extension.Utils;
using HarmonyLib;
using SandBox.View.Map;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace Extension.Features.QoL
{
    class FasterForwardHotKeyCategory : GameKeyContext
    {
        public static string CategoryId => "FasterForwardHotKeyCategory";
        public static bool Initialized { get; private set; } = false;

        static string HotKeyId => "FasterForward";

        FasterForwardHotKeyCategory()
            : base(CategoryId, 0)
        {
            for (int i = 0; i < Options.QoL.FasterForward.NumberOfSettings; i++)
            {
                RegisterHotKey(new HotKey($"{HotKeyId}{i + 1}", CategoryId, InputKey.D1 + i, HotKey.Modifiers.Control, HotKey.Modifiers.None), true);
            }
        }

        public static void Remove()
        {
            Initialized = false;
        }

        static void Initialize()
        {
            Dictionary<string, GameKeyContext> categories = (Dictionary<string, GameKeyContext>)
                Traverse.Create(typeof(HotKeyManager))
                        .Field("_categories")
                        .GetValue();
            if (categories.TryGetValue(CategoryId, out GameKeyContext category) == false)
            {
                categories.Add(CategoryId, category = new FasterForwardHotKeyCategory());
            }
            InputContext context = (InputContext)MapScreen.Instance?.Input;
            if (context != null)
            {
                if (context.IsCategoryRegistered(category) == false)
                {
                    context.RegisterHotKeyCategory(category);
                    Initialized = true;
                }
            }
        }

        public static bool IsHotKeyPressed(out int fasterForwardValue)
        {
            fasterForwardValue = -1;
            if (Initialized == false)
            {
                Initialize();
                return false;
            }
            for (int i = 0; i < Options.QoL.FasterForward.NumberOfSettings; i++)
            {
                if (MapScreen.Instance.Input.IsHotKeyPressed($"{HotKeyId}{i + 1}"))
                {
                    fasterForwardValue = (Options.QoL.FasterForward.Group[$"{Options.QoL.FasterForward.Group.Id}{i + 1}"] as IntOption).Value;
                    return true;
                }
            }
            return false;
        }
    }

    class FasterForward : CampaignBehaviorExt
    {
        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            base.OnSessionLaunched(starter);
            FasterForwardHotKeyCategory.Remove();
            Module.Instance.ApplicationTickEvent += OnApplicationTick;
            Module.Instance.GameEndEvent += OnGameEnd;
        }

        void OnGameEnd(Game game)
        {
            FasterForwardHotKeyCategory.Remove();
            Module.Instance.ApplicationTickEvent -= OnApplicationTick;
            Module.Instance.GameEndEvent -= OnGameEnd;
        }

        void OnApplicationTick(float dt)
        {
            if (MapScreen.Instance == null)
            {
                return;
            }
            if (FasterForwardHotKeyCategory.IsHotKeyPressed(out int fasterForwardValue))
            {
                if (Helper.TheCampaign.SpeedUpMultiplier != fasterForwardValue)
                {
                    Helper.TheCampaign.SpeedUpMultiplier = fasterForwardValue;
                    Helper.TheCampaign.TimeControlMode = CampaignTimeControlMode.Stop;
                    Helper.DisplayMessage($"Fast forward is now {fasterForwardValue}x time", Colors.Cyan);
                }
            }
        }

        static internal void Initialize_Configuration()
        {
            for (int i = 0; i < Options.QoL.FasterForward.NumberOfSettings; i++)
            {
                (Options.QoL.FasterForward.Group[$"{Options.QoL.FasterForward.Group.Id}{i + 1}"] as IntOption).Set(
                    value: (int)Math.Pow(2, i + 2),
                    defaultValue: (int)Math.Pow(2, i + 2),
                    min: 4,
                    max: 100);
            }
            Options.QoL.FasterForward.Group.Classes.Add(typeof(FasterForward));
        }
    }
}
