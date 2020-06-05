using Extension.Config;
using Extension.Utils;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Extension.Features.Sandbox
{
    class DonateFoodToLords : CampaignBehaviorExt
    {
        bool EnableDonateFoodToLords => Options.Sandbox.BoostAI.DonateFoodToLords.EnableDonateFoodToLords.Value;

        protected override void OnDailyTick()
        {
            if (!EnableDonateFoodToLords)
            {
                return;
            }
            foreach (MobileParty party in from party in MobileParty.All
                                          where party.IsLordParty
                                                && party != MobileParty.MainParty
                                                && party.GetNumDaysForFoodToLast() <= 0
                                                && party.Leader.HeroObject.Gold <= 10
                                          select party)
            {
                float consumption = Helper.TheCampaign.Models.MobilePartyFoodConsumptionModel.CalculateDailyFoodConsumptionf(party, null);
                party.ItemRoster.Add(new ItemRosterElement(DefaultItems.Grain, (int)(-consumption * 5)));
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.BoostAI.DonateFoodToLords.EnableDonateFoodToLords.Set(
                value: true,
                defaultValue: true);
            Options.Sandbox.BoostAI.Group.Classes.Add(typeof(DonateFoodToLords));
        }
    }
}
