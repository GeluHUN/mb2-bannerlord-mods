using HarmonyLib;
using TaleWorlds.Core;
using System.Linq;
using TaleWorlds.CampaignSystem;
using Extension.Config;

namespace Extension.Features.Battles
{
    class OverheadShields : CampaignBehaviorExt
    {
        bool EnableOverheadShields => Options.Battles.BattleParams.BattleAIParameters.EnableOverheadShields.Value;

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            if (!EnableOverheadShields)
            {
                return;
            }
            foreach (WeaponComponentData weapon in from item in ItemObject.All
                                                   where item.ItemType == ItemObject.ItemTypeEnum.Shield
                                                   from weapon in item.Weapons
                                                   where weapon.IsShield
                                                         && weapon.ItemUsage.Equals("shield")
                                                   select weapon)
            {
                Traverse.Create(weapon)
                        .Property("ItemUsage")
                        .SetValue("hand_shield");
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Battles.BattleParams.BattleAIParameters.EnableOverheadShields.Set(
                 value: true,
                 defaultValue: true);
            Options.Battles.BattleParams.Group.Classes.Add(typeof(OverheadShields));
        }
    }
}
