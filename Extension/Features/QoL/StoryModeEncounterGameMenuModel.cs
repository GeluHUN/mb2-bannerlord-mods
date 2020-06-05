using Extension.Config;
using StoryMode.GameModels;
using TaleWorlds.CampaignSystem;

namespace Extension.Features.QoL
{
    public class StoryModeEncounterGameMenuModelExt : StoryModeEncounterGameMenuModel
    {
        public override string GetEncounterMenu(PartyBase attackerParty, PartyBase defenderParty, out bool startBattle, out bool joinBattle)
        {
            string result = base.GetEncounterMenu(attackerParty, defenderParty, out startBattle, out joinBattle);
            if (result == "encounter_meeting"
                && PlayerEncounter.Current != null
                && PlayerEncounter.EncounteredParty != null
                && PlayerEncounter.EncounteredParty.MobileParty.IsBandit)
            {
                return "encounter";
            }
            else
            {
                return result;
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.QoL.InstantBattleWithBandits.Group.Classes.Add(typeof(StoryModeEncounterGameMenuModelExt));
        }
    }
}
