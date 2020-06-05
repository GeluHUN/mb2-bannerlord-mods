using Extension.Config;
using Extension.Utils;
using StoryMode;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Extension.Features.Campaign
{
    class InitialSkillLevel
    {
        public string SkillId { get; private set; }
        public int InitialValue => (Options.Campaign.StrongerHeroAtStart.Group[SkillId] as IntOption).Value;

        readonly string SkillName;

        InitialSkillLevel(string skillId, string skillName)
        {
            SkillId = skillId;
            SkillName = skillName;
        }

        void AddOption(int initialSkillValue)
        {
            FloatOption.Create(SkillId, Options.Campaign.StrongerHeroAtStart.Group,
                name: SkillName,
                hint: $"{SkillName} initial value to set at the game start.",
                value: initialSkillValue,
                defaultValue: initialSkillValue,
                min: 0,
                max: 250);
        }

        public static InitialSkillLevel Create(string skillId, int initialSkillValue)
        {
            InitialSkillLevel result = new InitialSkillLevel(skillId, skillId);
            result.AddOption(initialSkillValue);
            return result;
        }
    }

    class StrongerHeroAtStart : CampaignBehaviorExt
    {
        static readonly List<InitialSkillLevel> InitialSkillLevels = new List<InitialSkillLevel>();

        public override void RegisterEvents()
        {
            base.RegisterEvents();
            StoryModeEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(OnCharacterCreationIsOverEvent));
        }

        private void SetSkillLevel(string skillId, int level)
        {
            if (level > 0)
            {
                SkillObject skill = (from s in SkillObject.All where s.StringId == skillId select s).First();
                int current = Hero.MainHero.GetSkillValue(skill);
                int xpNeeded = Helper.TheCampaign.Models.CharacterDevelopmentModel.GetXpAmountForSkillLevelChange(Hero.MainHero, skill, level - current);
                float rate = Helper.TheCampaign.Models.CharacterDevelopmentModel.CalculateLearningRate(Hero.MainHero, skill);
                float mult = Helper.TheCampaign.Models.GenericXpModel.GetXpMultiplier(Hero.MainHero);
                Hero.MainHero.AddSkillXp(skill, (int)(xpNeeded / mult / rate));
            }
        }

        void OnCharacterCreationIsOverEvent()
        {
            foreach (InitialSkillLevel initialSkillLevel in InitialSkillLevels)
            {
                SetSkillLevel(initialSkillLevel.SkillId, initialSkillLevel.InitialValue);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.StrongerHeroAtStart.Group.Classes.Add(typeof(StrongerHeroAtStart));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Steward", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Trade", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Leadership", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Charm", 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Roguery", 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Scouting", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Tactics", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Crafting", 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Athletics", 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Riding", 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Throwing", 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Crossbow", 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Bow", 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Polearm", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("TwoHanded", 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create("OneHanded", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Medicine", 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create("Engineering", 0));
        }
    }
}
