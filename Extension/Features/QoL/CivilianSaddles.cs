using HarmonyLib;
using TaleWorlds.Core;
using System.Linq;
using TaleWorlds.CampaignSystem;
using Extension.Config;

namespace Extension.Features.QoL
{
    class CivilianSaddles : CampaignBehaviorExt
    {
        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            foreach (ItemObject item in from item in ItemObject.All
                                        where item.ItemType == ItemObject.ItemTypeEnum.HorseHarness
                                              && item.HasArmorComponent
                                              && (item.ArmorComponent.MaterialType == ArmorComponent.ArmorMaterialTypes.Cloth
                                                  || item.ArmorComponent.MaterialType == ArmorComponent.ArmorMaterialTypes.Leather)
                                        select item)
            {
                Traverse.Create(item)
                        .Property("ItemFlags")
                        .SetValue(item.ItemFlags | ItemFlags.Civilian);
            };
        }

        static internal void Initialize_Configuration()
        {
            Options.QoL.CivilianSaddles.Group.Classes.Add(typeof(CivilianSaddles));
        }
    }
}
