using Extension.Config;
using Extension.Utils;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace Extension.Features.Sandbox
{
    class WorkshopModelExt : DefaultWorkshopModel
    {
        int WorkshopLimitPerTier => Options.Sandbox.Clan.ClanTierModel.PartyLimitPerTier.Value;

        public override int GetMaxWorkshopCountForPlayer()
        {
            return (TaleWorlds.CampaignSystem.Clan.PlayerClan.Tier + 1) * WorkshopLimitPerTier;
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.Clan.WorkshopModel.WorkshopLimitPerTier.Set(
                value: 2,
                defaultValue: 2,
                min: 1,
                max: 5); ;
            Options.Sandbox.Clan.Group.Classes.Add(typeof(WorkshopModelExt));
        }
    }
}
