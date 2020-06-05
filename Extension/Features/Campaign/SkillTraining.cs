using Extension.Config;
using Extension.Utils;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Extension.Features.Campaign
{
    class SkillTraining : CampaignBehaviorExt
    {
        int TrainSessionLength => Options.Campaign.SkillTraining.TrainSessionLength.Value;
        int RestLength => Options.Campaign.SkillTraining.RestLength.Value;
        int SkillLevelGoldCost => Options.Campaign.SkillTraining.SkillLevelGoldCost.Value;
        int HighestTrainableSkillLevel => Options.Campaign.SkillTraining.HighestTrainableSkillLevel.Value;
        int ExperienceIncreasePerHour => Options.Campaign.SkillTraining.ExperienceIncreasePerHour.Value;

        int RestHoursRemain;
        bool IsTraining;
        Town Town;
        int TrainedHours;
        SkillObject SkillToTrain;
        float TrainedPercentage => (float)TrainedHours / TrainSessionLength;

        public override void RegisterEvents()
        {
            base.RegisterEvents();
            CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(OnSiegeEventStarted));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsLoading)
            {
                RestHoursRemain = 0;
                IsTraining = false;
                Town = null;
                TrainedHours = 0;
                SkillToTrain = null;
            }
            dataStore.SyncData("SkillTraining_RestHoursRemain", ref RestHoursRemain);
            dataStore.SyncData("SkillTraining_TrainedHours", ref TrainedHours);
            dataStore.SyncData("SkillTraining_IsTraining", ref IsTraining);
            dataStore.SyncData("SkillTraining_Town", ref Town);
            dataStore.SyncData("SkillTraining_SkillToTrain", ref SkillToTrain);
        }

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            AddMenus(starter);
            if (IsTraining)
            {
                Settlement currentSettlement = Settlement.CurrentSettlement;
                if (currentSettlement.IsTown && currentSettlement.Town == Town)
                {
                    GameMenu.SwitchToMenu("town_train_wait");
                }
            }
        }

        int GetTrainCost(SkillObject skill)
        {
            return Hero.MainHero.GetSkillValue(skill) * SkillLevelGoldCost;
        }

        void AddBeginSkillTrainingMenu(CampaignGameStarter starter, SkillObject skill)
        {
            starter.AddGameMenuOption(
                "town_train_options",
                $"town_train_{skill.Id}",
                new RichtextBuilder()
                    .Append($"Train your {skill.Name} for {{SKILL_TRAINING_COST_{skill.Id}}}")
                    .AppendImg("Icons\\Coin@2x")
                    .ToString(),
                delegate (MenuCallbackArgs menu)
                {
                    int cost = GetTrainCost(skill);
                    menu.IsEnabled = Hero.MainHero.GetSkillValue(skill) < HighestTrainableSkillLevel && Hero.MainHero.Gold >= cost;
                    menu.optionLeaveType = GameMenuOption.LeaveType.Wait;
                    MBTextManager.SetTextVariable($"SKILL_TRAINING_COST_{skill.Id}", $"{cost}");
                    return true;
                },
                delegate (MenuCallbackArgs menu)
                {
                    BeginTraining(skill);
                });
        }

        void AddMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption(
                "town",
                "town_train",
                "Skill trainers",
                delegate (MenuCallbackArgs menu)
                {
                    menu.IsEnabled = RestHoursRemain == 0;
                    if (RestHoursRemain > 0)
                    {
                        menu.Tooltip = new TextObject($"Trainers are unavailable for another {RestHoursRemain} {(RestHoursRemain > 1 ? "hours" : "hour")}");
                    }
                    else
                    {
                        menu.Tooltip = TextObject.Empty;
                    }
                    menu.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return true;
                },
                delegate (MenuCallbackArgs menu)
                {
                    GameMenu.SwitchToMenu("town_train_options");
                },
                false,
                4);
            starter.AddGameMenu(
                "town_train_options",
                "Train in the town for a day",
                null,
                GameOverlays.MenuOverlayType.SettlementWithCharacters);
            foreach (SkillObject skill in DefaultSkills.GetAllSkills())
            {
                AddBeginSkillTrainingMenu(starter, skill);
            }
            starter.AddGameMenuOption(
                "town_train_options",
                "town_train_back",
                "Back to town center",
                delegate (MenuCallbackArgs menu)
                {
                    menu.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                delegate (MenuCallbackArgs menu)
                {
                    GameMenu.SwitchToMenu("town");
                },
                true);
            starter.AddWaitGameMenu(
                "town_train_wait",
                "{TOWN_TRAIN_WAIT}",
                null,
                delegate (MenuCallbackArgs menu)
                {
                    MBTextManager.SetTextVariable("TOWN_TRAIN_WAIT", new TextObject("You've trained for {TOWN_TRAIN_HOURS_TRAINED}.", null), false);
                    menu.MenuContext.GameMenu.AllowWaitingAutomatically();
                    return true;
                },
                delegate (MenuCallbackArgs menu)
                {
                    StopTraining();
                },
                delegate (MenuCallbackArgs menu, CampaignTime dt)
                {
                    MBTextManager.SetTextVariable("TOWN_TRAIN_HOURS_TRAINED", $"{TrainedHours} {(TrainedHours == 1 ? "hour" : "hours")}");
                    menu.MenuContext.GameMenu.SetProgressOfWaitingInMenu(TrainedPercentage);
                },
                GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption,
                GameOverlays.MenuOverlayType.SettlementWithBoth,
                TrainSessionLength);
            starter.AddGameMenuOption(
                "town_train_wait",
                "town_train_wait_stop",
                "Stop training",
                delegate (MenuCallbackArgs menu)
                {
                    menu.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    return true;
                },
                delegate (MenuCallbackArgs menu)
                {
                    StopTraining();
                },
                true);
        }

        void BeginTraining(SkillObject skill)
        {
            GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, GetTrainCost(skill));
            SkillToTrain = skill;
            IsTraining = true;
            TrainedHours = 0;
            Town = Settlement.CurrentSettlement.Town;
            Helper.DisplayMessage($"You start training your {SkillToTrain.Name} skill in {Town.Name}");
            GameMenu.SwitchToMenu("town_train_wait");
        }

        void StopTraining()
        {
            Helper.DisplayMessage($"You finished training your {SkillToTrain.Name} skill in {Town.Name}");
            IsTraining = false;
            RestHoursRemain = RestLength;
            TrainedHours = 0;
            SkillToTrain = null;
            Town = null;
            GameMenu.SwitchToMenu("town");
        }

        void OnSiegeEventStarted(SiegeEvent siegeEvent)
        {
            if (IsTraining
                && siegeEvent.BesiegedSettlement.IsTown
                && Town == siegeEvent.BesiegedSettlement.Town)
            {
                StopTraining();
            }
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
                Hero.MainHero.AddSkillXp(SkillToTrain, ExperienceIncreasePerHour);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.SkillTraining.TrainSessionLength.Set(
                value: 24,
                defaultValue: 24,
                min: 1,
                max: 48);
            Options.Campaign.SkillTraining.RestLength.Set(
                value: 24,
                defaultValue: 24,
                min: 1,
                max: 48);
            Options.Campaign.SkillTraining.HighestTrainableSkillLevel.Set(
                value: 150,
                defaultValue: 150,
                min: 0,
                max: 250);
            Options.Campaign.SkillTraining.SkillLevelGoldCost.Set(
                value: 100,
                defaultValue: 100,
                min: 0,
                max: 1000);
            Options.Campaign.SkillTraining.ExperienceIncreasePerHour.Set(
                value: 10,
                defaultValue: 10,
                min: 1,
                max: 100);
            Options.Campaign.SkillTraining.Group.Classes.Add(typeof(SkillTraining));
        }
    }
}
