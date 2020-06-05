using Extension.Config;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace Extension.Features.Campaign
{
    class EnableRebellions : CampaignBehaviorExt
    {
        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            Traverse.Create(typeof(RebellionsCampaignBehavior))
                    .Field("_rebellionEnabled")
                    .SetValue(true);
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.Rebellions.Group.Classes.Add(typeof(EnableRebellions));
        }
    }
}
