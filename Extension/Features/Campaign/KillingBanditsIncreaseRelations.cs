using Extension.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Extension.Features.Campaign
{
    class KillingBanditsIncreaseRelations : CampaignBehaviorExt
    {
        int SettlementDistance => Options.Campaign.Bandits.KillingBanditsIncreaseRelations.SettlementDistance.Value;
        float LooterIncreaseRelation => Options.Campaign.Bandits.KillingBanditsIncreaseRelations.LooterIncreaseRelation.Value;
        float BanditIncreaseRelation => Options.Campaign.Bandits.KillingBanditsIncreaseRelations.BanditIncreaseRelation.Value;

        public override void RegisterEvents()
        {
            base.RegisterEvents();
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(OnPlayerBattleEndEvent));
        }

        void OnPlayerBattleEndEvent(MapEvent mapEvent)
        {
            if (mapEvent.HasWinner)
            {
                MapEventSide looserSide = mapEvent.DefeatedSide == BattleSideEnum.Attacker ? mapEvent.AttackerSide : mapEvent.DefenderSide;
                if (looserSide.LeaderParty.MapFaction.IsBanditFaction)
                {
                    foreach (Settlement settlement in from settlement in Settlement.All
                                                      where (settlement.IsVillage
                                                             || settlement.IsTown)
                                                            && settlement.Position2D.DistanceSquared(mapEvent.Position) <= SettlementDistance
                                                      select settlement)
                    {
                        int relationChange = GetRelationIncrease(looserSide.LeaderParty.Culture, looserSide.Casualties);
                        IncreaseRelationWithSettlement(settlement, mapEvent.Winner.Parties, relationChange);
                    }
                }
            }
        }

        int GetRelationIncrease(CultureObject banditCulture, int casualties)
        {
            if (banditCulture.StringId == "Looter")
            {
                return MathF.Round(casualties * LooterIncreaseRelation);
            }
            else
            {
                return MathF.Round(casualties * BanditIncreaseRelation);
            }
        }

        void IncreaseRelationWithSettlement(Settlement settlement, IEnumerable<PartyBase> parties, int relationChange)
        {
            if (relationChange > 0)
            {
                foreach ((Hero notable, Hero leader) in from notable in settlement.Notables
                                                        where notable.IsActive
                                                              && notable.IsAlive
                                                              && !notable.IsGangLeader
                                                        from party in parties
                                                        where party.LeaderHero != null
                                                        select (notable, party.LeaderHero))
                {
                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(leader, notable, relationChange, false);
                }
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.Bandits.KillingBanditsIncreaseRelations.SettlementDistance.Set(
                value: 500,
                defaultValue: 500,
                min: 0,
                max: 1000);
            Options.Campaign.Bandits.KillingBanditsIncreaseRelations.LooterIncreaseRelation.Set(
                value: 0.1f,
                defaultValue: 0.1f,
                min: 0,
                max: 1);
            Options.Campaign.Bandits.KillingBanditsIncreaseRelations.BanditIncreaseRelation.Set(
                value: 0.1f,
                defaultValue: 0.2f,
                min: 0,
                max: 1);
            Options.Campaign.Bandits.Group.Classes.Add(typeof(KillingBanditsIncreaseRelations));
        }
    }
}
