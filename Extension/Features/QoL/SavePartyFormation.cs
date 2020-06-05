using Extension.Config;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Extension.Features.QoL
{
    class SavePartyFormation : CampaignBehaviorExt
    {
        Dictionary<string, int> MainPartyFormations = new Dictionary<string, int>();

        public override void SyncData(IDataStore dataStore)
        {
            base.SyncData(dataStore);
            if (dataStore.IsSaving)
            {
                MainPartyFormations.Clear();
                foreach (CharacterObject characterObject in MobileParty.MainParty.MemberRoster.Troops)
                {
                    MainPartyFormations.Add(characterObject.StringId, (int)characterObject.CurrentFormationClass);
                }
            }
            dataStore.SyncData("MainPartyFormations", ref MainPartyFormations);
        }

        protected override void OnGameLoaded(CampaignGameStarter starter)
        {
            foreach (CharacterObject character in MobileParty.MainParty.MemberRoster.Troops)
            {
                if (MainPartyFormations.ContainsKey(character.StringId))
                {
                    character.CurrentFormationClass = (FormationClass)MainPartyFormations[character.StringId];
                }
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.QoL.SavePartyFormation.Group.Classes.Add(typeof(SavePartyFormation));
        }
    }
}
