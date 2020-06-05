using Extension.Config;
using Extension.Utils;
using StoryMode;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Extension.Features.Campaign
{
    class InitialSkillLevel
    {
        public readonly SkillObject Skill;
        public int InitialValue => (Options.Campaign.StrongerHeroAtStart.Group[OptionId] as IntOption).Value;

        readonly string OptionId;

        InitialSkillLevel(SkillObject skill)
        {
            Skill = skill;
            OptionId = skill.StringId;
        }

        void AddOption(int initialSkillValue)
        {
            FloatOption.Create(OptionId, Options.Campaign.StrongerHeroAtStart.Group,
                name: Skill.Name.ToString(),
                hint: $"{Skill.Name} initial value to set at the game start.",
                value: initialSkillValue,
                defaultValue: initialSkillValue,
                min: 0,
                max: 250);
        }

        public static InitialSkillLevel Create(SkillObject skill, int initialSkillValue)
        {
            InitialSkillLevel result = new InitialSkillLevel(skill);
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

        private void SetSkillLevel(SkillObject skill, int level)
        {
            if (level > 0)
            {
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
                SetSkillLevel(initialSkillLevel.Skill, initialSkillLevel.InitialValue);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.StrongerHeroAtStart.Group.Classes.Add(typeof(StrongerHeroAtStart));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Steward, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Trade, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Leadership, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Charm, 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Roguery, 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Scouting, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Tactics, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Crafting, 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Athletics, 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Riding, 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Throwing, 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Crossbow, 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Bow, 50));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Polearm, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.TwoHanded, 0));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.OneHanded, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Medicine, 25));
            InitialSkillLevels.Add(InitialSkillLevel.Create(DefaultSkills.Engineering, 0));
        }
    }
}
