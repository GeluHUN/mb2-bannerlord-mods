using Extension.Config;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Extension.Features.Sandbox
{
    class BoostLordTroops : CampaignBehaviorExt
    {
        int MaximumTierToBoost => Options.Sandbox.BoostAI.BoostLordTroops.MaximumTierToBoost.Value;

        protected override void OnDailyTick()
        {
            int maxTier = MaximumTierToBoost;
            foreach ((MobileParty party, CharacterObject troop) in from party in MobileParty.All
                                                                   where party.IsLordParty
                                                                         && !party.IsMainParty
                                                                         && party.MapEvent == null
                                                                         && !party.IsBandit
                                                                         && !party.IsBanditBossParty
                                                                   from troop in party.MemberRoster.Troops
                                                                   where troop.IsRegular
                                                                         && troop.Tier < maxTier
                                                                   select (party, troop))
            {
                party.MemberRoster.AddXpToTroop(troop.UpgradeXpCost * party.MemberRoster.GetTroopCount(troop), troop);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.BoostAI.BoostLordTroops.MaximumTierToBoost.Set(
                value: 2,
                defaultValue: 2,
                min: 0,
                max: 10);
            Options.Sandbox.BoostAI.Group.Classes.Add(typeof(BoostLordTroops));
        }
    }
}
