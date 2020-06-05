using Extension.Config;
using Extension.Utils;
using StoryMode.GameModels;
using TaleWorlds.CampaignSystem;

namespace Extension.Models
{
    class StoryModeGenericXpModelExt : StoryModeGenericXpModel
    {
        public override float GetXpMultiplier(Hero hero)
        {
            if (hero?.CurrentSettlement != null
                && StoryMode.Extensions.IsTrainingField(hero.CurrentSettlement))
            {
                return 1f;
            }
            else
            {
                return base.GetXpMultiplier(hero);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.StoryModeGenericXpModel.Group.Classes.Add(typeof(StoryModeGenericXpModelExt));
        }
    }
}
