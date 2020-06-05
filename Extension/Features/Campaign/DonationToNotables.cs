using Extension.Config;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Extension.Features.Campaign
{
    class DonationToNotables : CampaignBehaviorExt
    {
        protected override void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialogs(campaignGameStarter);
        }

        void AddDialogs(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddPlayerLine("gift",
                "hero_main_options", "give_gift",
                "Dear {?NPC.GENDER}madam{?}sir{\\?}, I would like to give you a small gift as a token of my appreciation!",
                delegate
                {
                    return Hero.OneToOneConversationHero.IsNotable && Hero.MainHero.Gold > 1000;
                }, null, 100, null);
            campaignGameStarter.AddDialogLine("gift_answer",
                "give_gift", "gift_options",
                "A gift? That is wonderful! What is it?[if:convo_delighted, ib:act_start_pleased_conversation]",
                null, null, 100, null);
            campaignGameStarter.AddPlayerLine("gift_option_1",
                "gift_options", "gift_accept_1",
                "Kindly accept this tiny bag of {SMALL_GIFT}{GOLD_ICON}!",
                delegate
                {
                    MBTextManager.SetTextVariable("SMALL_GIFT", 1000, false);
                    return Hero.MainHero.Gold > 1000;
                }, delegate
                {
                    GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, 10);
                }, 100, null);
            campaignGameStarter.AddPlayerLine("gift_option_2",
                "gift_options", "gift_accept_2",
                "Kindly accept this bag of {MEDIUM_GIFT}{GOLD_ICON}!",
                delegate
                {
                    MBTextManager.SetTextVariable("MEDIUM_GIFT", 5000, false);
                    return Hero.MainHero.Gold > 5000;
                }, delegate
                {
                    GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, 5000);
                }, 100, null);
            campaignGameStarter.AddPlayerLine("gift_option_3",
                "gift_options", "gift_accept_3",
                "Kindly accept this heavy bag of {LARGE_GIFT}{GOLD_ICON}!",
                delegate
                {
                    MBTextManager.SetTextVariable("LARGE_GIFT", 10000, false);
                    return Hero.MainHero.Gold > 10000;
                }, delegate
                {
                    GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, 10000);
                }, 100, null);
            campaignGameStarter.AddPlayerLine("gift_options_exit",
                "gift_options", "lord_pretalk",
                "Actually, never mind.",
                null, null, 100, null);
            campaignGameStarter.AddDialogLine("gift_accept_1",
                "gift_accept_1", "gift_done",
                "Thank you![rf:happy][rb:trivial]",
                null,
                delegate
                {
                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, Hero.OneToOneConversationHero, 2, false);
                    string name = Hero.OneToOneConversationHero.Name.ToString();
                    string relation = ((int)Hero.OneToOneConversationHero.GetRelationWithPlayer()).ToString();
                    InformationManager.DisplayMessage(new InformationMessage($"Your relation with {name} is now {relation}"));
                }, 100, null);
            campaignGameStarter.AddDialogLine("gift_accept_2",
                "gift_accept_2", "gift_done",
                "Thank very much![rf:happy][rb:positive]",
                null,
                delegate
                {
                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, Hero.OneToOneConversationHero, 10, false);
                    string name = Hero.OneToOneConversationHero.Name.ToString();
                    string relation = ((int)Hero.OneToOneConversationHero.GetRelationWithPlayer()).ToString();
                    InformationManager.DisplayMessage(new InformationMessage($"Your relation with {name} is now {relation}"));
                }, 100, null);
            campaignGameStarter.AddDialogLine("gift_accept_3",
                "gift_accept_3", "gift_done",
                "I am very grateful, thank you very much![rf:happy][rb:very_positive]",
                null,
                delegate
                {
                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, Hero.OneToOneConversationHero, 20, false);
                    string name = Hero.OneToOneConversationHero.Name.ToString();
                    string relation = ((int)Hero.OneToOneConversationHero.GetRelationWithPlayer()).ToString();
                    InformationManager.DisplayMessage(new InformationMessage($"Your relation with {name} is now {relation}"));
                }, 100, null);
            campaignGameStarter.AddPlayerLine("gift_done",
                "gift_done", "lord_pretalk",
                "You are welcome {?NPC.GENDER}madam{?}sir{\\?}!",
                null, null, 100, null);
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.Persuasion.Group.Enabled = false;
            Options.Campaign.DonationToNotables.Group.Classes.Add(typeof(DonationToNotables));
        }
    }
}
