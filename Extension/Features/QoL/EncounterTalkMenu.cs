using Extension.Config;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;

namespace Extension.Features.QoL
{
    class EncounterTalkMenu : CampaignBehaviorExt
    {
        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption(
                "encounter",
                "encounter_talk",
                "Talk",
                delegate (MenuCallbackArgs menu)
                {
                    menu.optionLeaveType = GameMenuOption.LeaveType.Conversation;
                    return PlayerEncounter.Current != null
                           && PlayerEncounter.EncounteredParty != null
                           && PlayerEncounter.EncounteredParty.MobileParty.IsBandit;
                },
                delegate (MenuCallbackArgs menu)
                {
                    if (PlayerEncounter.Current == null || ((PlayerEncounter.Battle == null || PlayerEncounter.Battle.AttackerSide.LeaderParty == PartyBase.MainParty || PlayerEncounter.Battle.DefenderSide.LeaderParty == PartyBase.MainParty) && !PlayerEncounter.MeetingDone))
                    {
                        PlayerEncounter.DoMeeting();
                    }
                    else if (PlayerEncounter.LeaveEncounter)
                    {
                        PlayerEncounter.Finish(true);
                    }
                    else if (PlayerEncounter.Battle == null)
                    {
                        PlayerEncounter.StartBattle();
                    }
                },
                false,
                1);
        }

        static internal void Initialize_Configuration()
        {
            Options.QoL.InstantBattleWithBandits.Group.Classes.Add(typeof(EncounterTalkMenu));
        }
    }
}
