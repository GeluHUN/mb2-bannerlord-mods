using Extension.Config;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;

namespace Extension.Features.Campaign
{
    [HarmonyPatch(typeof(Persuasion))]
    class PersuasionAlwaysSuccesful
    {
        [HarmonyPatch("GetResult")]
        [HarmonyPrefix]
        public static bool Prefix(ref PersuasionOptionResult __result)
        {
            __result = PersuasionOptionResult.Success;
            return false;
        }

        static internal bool Prepare()
        {
            return Options.Campaign.Persuasion.Group.Enabled;
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.Persuasion.Group.Enabled = false;
        }
    }
}
