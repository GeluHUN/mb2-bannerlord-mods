using Extension.Config;
using Extension.Utils;
using HarmonyLib;
using StoryMode.Behaviors;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Extension.Features.Campaign
{
    class TroopTraining : CampaignBehaviorExt
    {
        int TrainSessionLength => Options.Campaign.TroopTraining.TrainSessionLength.Value;
        int RestLength => Options.Campaign.TroopTraining.RestLength.Value;
        int HighestTrainableTroopTier => Options.Campaign.TroopTraining.HighestTrainableTroopTier.Value;
        int TroopTierGoldCost => Options.Campaign.TroopTraining.TroopTierGoldCost.Value;
        float FieldUsageFeePercantage => Options.Campaign.TroopTraining.FieldUsageFeePercantage.Value;
        int ExperienceIncreasePerHour => Options.Campaign.TroopTraining.ExperienceIncreasePerHour.Value;
        float MaxWoundChance => Options.Campaign.TroopTraining.MaxWoundChance.Value;

        int RestHoursRemain;
        bool IsTraining;
        int TrainedHours;
        float TrainedPercentage => (float)TrainedHours / TrainSessionLength;

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                RestHoursRemain = 0;
                IsTraining = false;
                TrainedHours = 0;
            }
            dataStore.SyncData("TroopTraining_RestHoursRemain", ref RestHoursRemain);
            dataStore.SyncData("TroopTraining_TrainedHours", ref TrainedHours);
            dataStore.SyncData("TroopTraining_IsTraining", ref IsTraining);
        }

        internal void OnSessionLaunchedPatch(CampaignGameStarter starter)
        {
            AddMenus(starter);
            if (IsTraining)
            {
                GameMenu.SwitchToMenu("troop_traning_wait");
            }
        }

        bool IsTroopValidForTrain(FlattenedTroopRosterElement troop)
        {
            return troop.IsWounded == false
                   && troop.IsKilled == false
                   && troop.Troop.IsHero == false
                   && troop.Troop.IsRegular
                   && troop.Troop.Tier <= HighestTrainableTroopTier;
        }

        int GetTrainCost()
        {
            float trainCost = 0f;
            foreach (CharacterObject troop in from troop in MobileParty.MainParty.MemberRoster.ToFlattenedRoster()
                                              where IsTroopValidForTrain(troop)
                                              select troop.Troop)
            {
                trainCost += troop.Tier * TroopTierGoldCost;
            }
            return (int)(trainCost * (1 + FieldUsageFeePercantage));
        }

        void AddMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption(
                "training_field_menu",
                "training_field_traintroops",
                new RichtextBuilder()
                    .Append("Train your troops for {TROOP_TRAINING_COST}")
                    .AppendImg("Icons\\Coin@2x")
                    .ToString(),
                delegate (MenuCallbackArgs menu)
                {
                    TroopRoster troops = MobileParty.MainParty.MemberRoster;
                    menu.IsEnabled = RestHoursRemain == 0 && (troops.TotalRegulars - troops.TotalWoundedRegulars) > 0;
                    if ((troops.TotalRegulars - troops.TotalWoundedRegulars) <= 0)
                    {
                        menu.Tooltip = new TextObject($"You don't have any troops able to train");
                    }
                    else if (RestHoursRemain > 0)
                    {
                        menu.Tooltip = new TextObject($"The trainign field is unavailable for another {RestHoursRemain} {(RestHoursRemain > 1 ? "hours" : "hour")}");
                    }
                    else
                    {
                        menu.Tooltip = TextObject.Empty;
                    }
                    menu.optionLeaveType = GameMenuOption.LeaveType.Wait;
                    MBTextManager.SetTextVariable("TROOP_TRAINING_COST", $"{GetTrainCost()}");
                    return true;
                },
                delegate (MenuCallbackArgs menu)
                {
                    BeginTraining();
                },
                false,
                1);
            starter.AddWaitGameMenu(
                "troop_traning_wait",
                "{TROOP_TRAINING_WAIT}",
                null,
                delegate (MenuCallbackArgs menu)
                {
                    MBTextManager.SetTextVariable("TROOP_TRAINING_WAIT", $"Your troops have trained for {TrainedHours} {(TrainedHours == 1 ? "hour" : "hours")}");
                    menu.MenuContext.GameMenu.AllowWaitingAutomatically();
                    return true;
                },
                delegate (MenuCallbackArgs menu)
                {
                    StopTraining();
                },
                delegate (MenuCallbackArgs menu, CampaignTime dt)
                {
                    MBTextManager.SetTextVariable("TROOP_TRAINING_HOURS_TRAINED", string.Format("{0} {1}", TrainedHours, TrainedHours == 1 ? "hour" : "hours"), false);
                    menu.MenuContext.GameMenu.SetProgressOfWaitingInMenu(TrainedPercentage);
                },
                GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption,
                GameOverlays.MenuOverlayType.None, TrainSessionLength);
            starter.AddGameMenuOption(
                "troop_traning_wait",
                "troop_traning_stop",
                "Stop the training",
                delegate (MenuCallbackArgs menu)
                {
                    menu.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                delegate (MenuCallbackArgs args)
                {
                    StopTraining();
                },
                true);
        }

        void BeginTraining()
        {
            GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, GetTrainCost());
            IsTraining = true;
            TrainedHours = 0;
            Helper.DisplayMessage("Your troops start training");
            GameMenu.SwitchToMenu("troop_traning_wait");
        }

        void StopTraining()
        {
            Helper.DisplayMessage("Your troops finished training");
            IsTraining = false;
            TrainedHours = 0;
            RestHoursRemain = RestLength;
            GameMenu.SwitchToMenu("training_field_menu");
        }

        protected override void OnHourlyTick()
        {
            if (!IsTraining && RestHoursRemain > 0)
            {
                RestHoursRemain--;
            }
            if (IsTraining)
            {
                TrainedHours++;
                GiveExperienceToTroops();
            }
        }

        void GiveExperienceToTroops()
        {
            TroopRoster troopRoster = MobileParty.MainParty.MemberRoster;
            int troopsGainedExp = 0;
            int troopsWounded = 0;
            foreach (CharacterObject troop in from troop in MobileParty.MainParty.MemberRoster.ToFlattenedRoster()
                                              where IsTroopValidForTrain(troop)
                                              select troop.Troop)
            {
                troopsGainedExp++;
                troopRoster.AddXpToTroop(ExperienceIncreasePerHour, troop);
                float woundChance = MaxWoundChance / (troop.Tier + 1);
                if (MBRandom.RandomFloat < woundChance)
                {
                    troopsWounded++;
                    troopRoster.WoundTroop(troop);
                }
            }
            Helper.DisplayMessage($"{troopsGainedExp} troops gained experiance and {troopsWounded} troops wounded in the last hour");
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.TroopTraining.TrainSessionLength.Set(
                value: 24,
                defaultValue: 24,
                min: 1,
                max: 48);
            Options.Campaign.TroopTraining.RestLength.Set(
                value: 24,
                defaultValue: 24,
                min: 1,
                max: 48);
            Options.Campaign.TroopTraining.HighestTrainableTroopTier.Set(
                value: 4,
                defaultValue: 4,
                min: 1,
                max: 10);
            Options.Campaign.TroopTraining.TroopTierGoldCost.Set(
                value: 10,
                defaultValue: 10,
                min: 1,
                max: 100);
            Options.Campaign.TroopTraining.FieldUsageFeePercantage.Set(
                value: 0.2f,
                defaultValue: 0.2f,
                min: 0,
                max: 1);
            Options.Campaign.TroopTraining.ExperienceIncreasePerHour.Set(
                value: 25,
                defaultValue: 25,
                min: 1,
                max: 1000);
            Options.Campaign.TroopTraining.MaxWoundChance.Set(
                value: 0.05f,
                defaultValue: 0.05f);
            Options.Campaign.TroopTraining.Group.Classes.Add(typeof(TroopTraining));
        }
    }

    [HarmonyPatch(typeof(TrainingFieldCampaignBehavior))]
    class TrainingFieldMenuPatch
    {
        [HarmonyPatch("OnSessionLaunched")]
        [HarmonyPostfix]
        public static void OnSessionLaunched_Postfix(CampaignGameStarter campaignGameStarter)
        {
            Helper.GetCampaignBehavior<TroopTraining>().OnSessionLaunchedPatch(campaignGameStarter);
        }

        static internal bool Prepare()
        {
            return Options.Campaign.TroopTraining.Group.Enabled;
        }
    }
}
