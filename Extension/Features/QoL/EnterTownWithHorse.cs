using HarmonyLib;
using TaleWorlds.Core;
using SandBox.Source.Missions;
using SandBox;
using TaleWorlds.MountAndBlade;
using Extension.Config;

namespace Extension.Features.QoL
{
    [HarmonyPatch(typeof(TownCenterMissionController))]
    class TownCenterMissionControllerPatch
    {
        [HarmonyPatch("AfterStart")]
        [HarmonyPrefix]
        public static bool TownCenterMissionController_AfterStart(TownCenterMissionController __instance)
        {
            AfterStart(__instance);
            return false;
        }

        static void AfterStart(TownCenterMissionController townCenterMissionController)
        {
            if (!GameNetwork.IsClientOrReplay)
            {
                bool isNight = TaleWorlds.CampaignSystem.Campaign.Current.IsNight;
                townCenterMissionController.Mission.SetMissionMode(MissionMode.StartUp, true);
                townCenterMissionController.Mission.IsInventoryAccessible = !TaleWorlds.CampaignSystem.Campaign.Current.IsMainHeroDisguised;
                townCenterMissionController.Mission.IsQuestScreenAccessible = true;
                townCenterMissionController.Mission.DoesMissionRequireCivilianEquipment = true;
                MissionAgentHandler missionBehaviour = townCenterMissionController.Mission.GetMissionBehaviour<MissionAgentHandler>();
                missionBehaviour.SpawnPlayer(townCenterMissionController.Mission.DoesMissionRequireCivilianEquipment, false, false, false, false, false, "");
                missionBehaviour.SpawnLocationCharacters(null, isNight);
                MissionAgentHandler.SpawnHorses();
                MissionAgentHandler.SpawnCats();
                MissionAgentHandler.SpawnDogs();
                if (!isNight)
                {
                    MissionAgentHandler.SpawnSheeps();
                    MissionAgentHandler.SpawnCows();
                    MissionAgentHandler.SpawnHogs();
                    MissionAgentHandler.SpawnGeese();
                    MissionAgentHandler.SpawnChicken();
                }
            }
        }

        static internal bool Prepare()
        {
            return Options.QoL.EnterTownWithHorse.Group.Enabled;
        }
    }
}
