using Extension.Config;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Extension.Features.Sandbox
{
    class DonateGoldToTowns : CampaignBehaviorExt
    {
        protected override void OnDailyTick()
        {
            foreach (Settlement settlement in from settlement in Settlement.All
                                              where settlement.IsTown && settlement.Town.Gold < 5000
                                              select settlement)
            {
                settlement.Town.ChangeGold(5000 - settlement.Town.Gold);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.BoostAI.DonateGoldToTowns.EnableDonateDonateGoldToTowns.Set(
                value: true,
                defaultValue: true);
            Options.Sandbox.BoostAI.Group.Classes.Add(typeof(DonateGoldToTowns));
        }
    }
}
