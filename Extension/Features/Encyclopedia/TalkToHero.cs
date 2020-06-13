using Extension.Config;
using Extension.Utils;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Extension.Features.Ecyclopedia
{
    class TalkToHero : CampaignBehaviorExt
    {
        readonly List<Hero> TalkQueue = new List<Hero>();

        public override void RegisterEvents()
        {
            base.RegisterEvents();
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(OnHeroKilled));
        }

        void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
        {
            if (TalkQueue.Contains(victim))
            {
                TalkQueue.Remove(victim);
            }
        }

        public bool CanTalkTo(Hero hero)
        {
            return hero.IsAlive
                   && hero != Hero.MainHero
                   && !hero.IsDisabled
                   && !hero.IsPrisoner;
        }

        public void InitiateTalk(Hero hero)
        {
            if (CanTalkTo(hero))
            {
                TalkQueue.Add(hero);
                Helper.DisplayMessage($"{hero.Name} will talk to you soon");
            }
            else
            {
                Helper.DisplayMessage($"{hero.Name} is unavailable");
            }
        }

        public bool QueuedToTalk(Hero hero)
        {
            return TalkQueue.Contains(hero);
        }

        protected override void OnHourlyTick()
        {
            if (Hero.MainHero.IsOccupiedByAnEvent())
            {
                return;
            }
            foreach (Hero hero in TalkQueue)
            {
                if (CanTalkTo(hero) && !hero.IsOccupiedByAnEvent())
                {
                    TalkQueue.Remove(hero);
                    StartMeeting(hero);
                    break;
                }
            }
        }

        void StartMeeting(Hero hero)
        {
            Helper.TheCampaign.SetTimeSpeed(0);
            if (hero.PartyBelongedTo == null)
            {
                CampaignMission.OpenConversationMission(
                    new ConversationCharacterData(CharacterObject.PlayerCharacter, MobileParty.MainParty.Party),
                    new ConversationCharacterData(hero.CharacterObject, null));
            }
            else
            {
                Helper.TheCampaign.HandlePartyEncounter(MobileParty.MainParty.Party, hero.PartyBelongedTo.Party);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Encyclopedia.TalkToHero.Group.Classes.Add(typeof(TalkToHero));
        }
    }
}
