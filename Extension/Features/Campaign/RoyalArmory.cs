using Extension.Config;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Extension.Features.Campaign
{
    class RoyalArmory : CampaignBehaviorExt
    {
        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption(
                "town_keep",
                "armoury",
                "Royal armoury",
                delegate (MenuCallbackArgs menu)
                {
                    menu.optionLeaveType = GameMenuOption.LeaveType.Trade;
                    string culture = Settlement.CurrentSettlement.OwnerClan.Kingdom.Culture.StringId;
                    return culture == "battania"
                           || culture == "aserai"
                           || culture == "sturgia"
                           || culture == "vlandia"
                           || culture == "empire"
                           || culture == "khuzait";
                },
                delegate
                {
                    ItemRoster itemRoster = new ItemRoster();
                    if (Settlement.CurrentSettlement.Culture.StringId == "aserai")
                    {
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("southern_noble_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("southern_lord_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("aserai_lord_helmet_a"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("desert_scale_shoulders"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("leopard_pelt"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("desert_scale_armor"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgia_cavalry_armor"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("desert_lamellar"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("stitched_leather_over_mail"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mail_and_plate_barding"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("half_mail_and_plate_barding"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("eastern_javelin_3_t4"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("steel_round_shield"), 10, true);
                    }
                    if (Settlement.CurrentSettlement.Culture.StringId == "battania")
                    {
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battanian_crowned_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battanian_plated_noble_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battanian_noble_helmet_with_feather"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_warlord_pauldrons"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("armored_bearskin"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("bearskin"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("rough_bearskin"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_warlord_armor"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_mercenary_armor"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("kilt_over_plated_leather"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_warlord_bracers"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_noble_bracers"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_warlord_boots"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_horse_harness_scaled"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("battania_horse_harness_halfscaled"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("bodkin_arrows_a"), 10, true);
                    }
                    if (Settlement.CurrentSettlement.Culture.StringId == "empire")
                    {
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("empire_guarded_lord_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("empire_jewelled_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("empire_lord_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("empire_helmet_with_metal_strips"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("imperial_goggled_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("imperial_lamellar_shoulders"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("pauldron_cape_a"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("studded_imperial_neckguard"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("imperial_scale_armor"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lamellar_with_scale_skirt"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("imperial_lamellar"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("legionary_mail"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lamellar_plate_gauntlets"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("decorated_imperial_gauntlets"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lordly_padded_mitten"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lamellar_plate_boots"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("decorated_imperial_boots"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("half_scale_barding"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("imperial_scale_barding"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("arrow_emp_1_a"), 10, true);
                    }
                    if (Settlement.CurrentSettlement.Culture.StringId == "khuzait")
                    {
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("khuzait_noble_helmet_with_neckguard"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("spiked_helmet_with_facemask"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("khuzait_noble_helmet_with_fur"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("eastern_vendel_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("khuzait_noble_helmet_with_feathers"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("brass_lamellar_shoulder"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lamellar_shoulders"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("brass_lamellar_over_mail"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("eastern_plated_leather"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("eastern_plated_leather_vambraces"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("studded_steppe_barding"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("steppe_half_barding"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("heavy_steppe_arrows"), 10, true);
                    }
                    if (Settlement.CurrentSettlement.Culture.StringId == "sturgia")
                    {
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgian_lord_helmet_c"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lendman_helmet_over_full_mail"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lendman_helmet_over_mail"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("northern_warlord_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgian_lord_helmet_b"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgian_helmet_b_close"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgian_lord_helmet_a"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("imperial_goggled_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("decorated_goggled_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("brass_lamellar_shoulder_white"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("brass_scale_shoulders"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("armored_bearskin"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("bearskin"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("rough_bearskin"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgian_fortified_armor"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgian_lamellar_base"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sturgian_lamellar_fortified"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("northern_coat_of_plates"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("northern_brass_lamellar_over_mail"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("northern_plated_gloves"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("northern_plated_boots"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("northern_ring_barding"), 10, true);
                    }
                    if (Settlement.CurrentSettlement.Culture.StringId == "vlandia")
                    {
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("western_crowned_plated_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("western_crowned_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("full_helm_over_mail_coif"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("western_plated_helmet"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mail_shoulders"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("coat_of_plates_over_mail"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("plated_leather_coat"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lordly_mail_mitten"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("reinforced_mail_mitten"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("lordly_padded_mitten"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("strapped_mail_chausses"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("mail_chausses"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("chain_barding"), 10, true);
                        itemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("halfchain_barding"), 10, true);
                    }
                    InventoryManager.OpenScreenAsTrade(itemRoster, Settlement.CurrentSettlement.Town, InventoryManager.InventoryCategoryType.None, null);
                },
                false,
                -1,
                false);
        }

        static internal void Initialize_Configuration()
        {
            Options.Campaign.RoyalArmory.Group.Classes.Add(typeof(RoyalArmory));
        }
    }
}
