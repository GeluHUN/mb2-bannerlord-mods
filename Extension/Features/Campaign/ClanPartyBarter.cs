using Extension.Config;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Barterables;

namespace Extension.Features.Campaign
{
    class ClanPartyBarter : CampaignBehaviorExt
    {
        public override void RegisterEvents()
        {
            base.RegisterEvents();
            CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(CheckForBarters));
        }

        protected override void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }

        void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine(
                "clan_member_barter",
                "hero_main_options",
                "lord_pretalk",
                "Let me trade with you.",
                delegate
                {
                    Hero hero = Hero.OneToOneConversationHero;
                    return hero != null
                           && hero.Clan == Clan.PlayerClan
                           && hero.PartyBelongedTo != null
                           && hero.PartyBelongedTo.LeaderHero == hero
                           && !hero.PartyBelongedTo.IsCaravan
                           && !hero.PartyBelongedTo.IsMilitia
                           && !hero.PartyBelongedTo.IsVillager;
                },
                delegate
                {
                    BarterManager.Instance.StartBarterOffer(
                        Hero.MainHero,
                        Hero.OneToOneConversationHero,
                        PartyBase.MainParty,
                        Hero.OneToOneConversationHero?.PartyBelongedTo?.Party,
                        Hero.OneToOneConversationHero,
                        null,
                        0,
                        false,
                        null);
                },
                110,
                null,
                null);
        }

        void CheckForBarters(BarterData barter)
        {
            if ((barter.OffererHero != null && barter.OtherHero != null)
                || (barter.OffererHero == null && barter.OffererParty != null)
                || (barter.OtherHero == null && barter.OtherParty != null))
            {
                barter.AddBarterable<GoldBarterGroup>(
                    new GoldBarterable(
                        barter.OffererHero,
                        barter.OtherHero,
                        barter.OffererParty,
                        barter.OtherParty,
                        (barter.OffererHero != null) ? barter.OffererHero.Gold : barter.OffererParty.MobileParty.PartyTradeGold),
                    false);
                barter.AddBarterable<GoldBarterGroup>(
                    new GoldBarterable(
                        barter.OtherHero,
                        barter.OffererHero,
                        barter.OtherParty,
                        barter.OffererParty,
                        (barter.OtherHero != null) ? barter.OtherHero.Gold : barter.OtherParty.MobileParty.PartyTradeGold),
                    false);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.PartyOrders.Group.Classes.Add(typeof(ClanPartyBarter));
        }
    }

    [HarmonyPatch(typeof(BarterManager))]
    class BarterWithOwnClanIsAlwaysOk
    {
        [HarmonyPatch("IsOfferAcceptable", new Type[] { typeof(BarterData), typeof(Hero), typeof(PartyBase) })]
        [HarmonyPrefix]
        public static bool IsOfferAcceptable_Prefix(BarterData args, ref bool __result)
        {
            if (args.OffererHero == Hero.MainHero && args.OtherHero?.Clan == Clan.PlayerClan)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch("ApplyAndFinalizePlayerBarter", new Type[] { typeof(Hero), typeof(Hero), typeof(BarterData) })]
        [HarmonyPostfix]
        public static void ApplyAndFinalizePlayerBarter_Postfix(Hero offererHero, Hero otherHero)
        {
            if (offererHero == Hero.MainHero && otherHero?.Clan == Clan.PlayerClan)
            {
                Dictionary<Hero, CampaignTime> barteredHeroes = (Dictionary<Hero, CampaignTime>)Traverse.Create(BarterManager.Instance)
                    .Field("_barteredHeroes")
                    .GetValue();
                barteredHeroes.Remove(otherHero);
            }
        }

        static internal bool Prepare()
        {
            return Options.Campaign.PartyOrders.Group.Enabled;
        }
    }
}
