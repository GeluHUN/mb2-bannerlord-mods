using System.Linq;
using TaleWorlds.CampaignSystem;
using Extension.Config;
using TaleWorlds.Core;

namespace Extension.Features.Campaign
{
    class WanderersAreNice : CampaignBehaviorExt
    {
        protected override void OnDailyTick()
        {
            MakeWanderersNice();
        }

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            MakeWanderersNice();
        }

        void FixWandererTrait(Hero hero, TraitObject trait)
        {
            if (hero.GetHeroTraits().HasProperty(trait))
            {
                int value = hero.GetHeroTraits().GetPropertyValue(trait);
                if (value < 0)
                {
                    hero.GetHeroTraits().SetPropertyValue(trait, -value);
                }
            }
        }

        void FixWandererAttribute(Hero hero, CharacterAttributesEnum attrib)
        {
            if (hero.GetAttributeValue(attrib) < 3)
            {
                hero.SetAttributeValue(attrib, 3);
            }
        }

        void MakeWanderersNice()
        {
            foreach (Hero hero in from hero in Hero.All
                                  where hero.IsWanderer
                                        && hero.IsActive
                                        && !hero.IsTemplate
                                  select hero)
            {
                FixWandererTrait(hero, DefaultTraits.Mercy);
                FixWandererTrait(hero, DefaultTraits.Valor);
                FixWandererTrait(hero, DefaultTraits.Honor);
                FixWandererTrait(hero, DefaultTraits.Generosity);
                FixWandererTrait(hero, DefaultTraits.Calculating);
                FixWandererAttribute(hero, CharacterAttributesEnum.Control);
                FixWandererAttribute(hero, CharacterAttributesEnum.Cunning);
                FixWandererAttribute(hero, CharacterAttributesEnum.Endurance);
                FixWandererAttribute(hero, CharacterAttributesEnum.Intelligence);
                FixWandererAttribute(hero, CharacterAttributesEnum.Social);
                FixWandererAttribute(hero, CharacterAttributesEnum.Vigor);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.WanderersAreNice.Group.Classes.Add(typeof(WanderersAreNice));
        }
    }
}
