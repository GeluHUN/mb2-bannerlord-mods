using Extension.Config;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace Extension.Features.Campaign
{
    class TroopCountLimitModelExt : DefaultTroopCountLimitModel
    {
        int HideoutBattlePlayerMaxTroopCount => Options.Campaign.Bandits.TroopCountLimit.HideoutBattlePlayerMaxTroopCount.Value;

        public override int GetHideoutBattlePlayerMaxTroopCount()
        {
            return HideoutBattlePlayerMaxTroopCount;
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.Bandits.TroopCountLimit.HideoutBattlePlayerMaxTroopCount.Set(
                value: 20,
                defaultValue: 20,
                min: 1,
                max: 50);
            Options.Campaign.Bandits.Group.Classes.Add(typeof(TroopCountLimitModelExt));
        }
    }
}
