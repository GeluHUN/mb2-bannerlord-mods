using Extension.Config;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Encyclopedia.Pages;
using TaleWorlds.Localization;

namespace Extension.Features.Ecyclopedia
{
    [EncyclopediaModel(new Type[] { typeof(Settlement) })]
    class EncyclopediaSettlementPage : DefaultEncyclopediaSettlementPage
    {
        public EncyclopediaSettlementPage()
            : base()
        {
            List<EncyclopediaFilterGroup> filters = (List<EncyclopediaFilterGroup>)_filters;
            EncyclopediaFilterGroup group = filters.Find(g => g.Name.ToString() == "State");
            if (group != null)
            {
                filters.Remove(group);
            }
            filters.Insert(0, CreateStateFilterGroup());
            filters.Add(CreateWorkshopFilterGroup());
            filters.Add(CreateKingdomFilterGroup());
        }

        EncyclopediaFilterGroup CreateStateFilterGroup()
        {
            List<EncyclopediaFilterItem> items = new List<EncyclopediaFilterItem>
            {
                new EncyclopediaFilterItem(new TextObject("Has Visited", null), (object s) => ((Settlement)s).HasVisited),
                new EncyclopediaFilterItem(new TextObject("Sieged", null), (object s) => ((Settlement)s).IsUnderSiege),
                new EncyclopediaFilterItem(new TextObject("Under raid", null), (object s) => ((Settlement)s).IsUnderRaid),
                new EncyclopediaFilterItem(new TextObject("Looted", null), (object s) => ((Settlement)s).IsRaided),
                new EncyclopediaFilterItem(new TextObject("Starving", null), (object s) => ((Settlement)s).IsStarving),
                new EncyclopediaFilterItem(new TextObject("Rebelling", null), (object s) => ((Settlement)s).IsRebelling),
                new EncyclopediaFilterItem(new TextObject("Booming", null), (object s) => ((Settlement)s).IsBooming)
            };
            return new EncyclopediaFilterGroup(items, new TextObject("State", null));
        }

        EncyclopediaFilterGroup CreateWorkshopFilterGroup()
        {
            List<EncyclopediaFilterItem> items = new List<EncyclopediaFilterItem>();
            foreach (WorkshopType workshopType in WorkshopType.All)
            {
                items.Add(new EncyclopediaFilterItem(workshopType.Name, (object obj) =>
                {
                    Settlement settlement = obj as Settlement;
                    if (settlement.IsTown)
                    {
                        return settlement.Town.Workshops.Any(w => w.WorkshopType == workshopType);
                    }
                    return false;
                }));
            }
            return new EncyclopediaFilterGroup(items, new TextObject("Workshops", null));
        }

        EncyclopediaFilterGroup CreateKingdomFilterGroup()
        {
            List<EncyclopediaFilterItem> items = new List<EncyclopediaFilterItem>();
            foreach (Kingdom kingdom in Kingdom.All)
            {
                items.Add(new EncyclopediaFilterItem(kingdom.Name, (object obj) =>
                {
                    return (obj as Settlement).OwnerClan.MapFaction == kingdom;
                }));
            }
            return new EncyclopediaFilterGroup(items, new TextObject("Kingdoms", null));
        }

        static internal void Initialize_Configuration()
        {
            Options.Encyclopedia.Settlement.Group.Enabled = true;
        }
    }

    [HarmonyPatch(typeof(EncyclopediaManager))]
    class EncyclopediaManagerPatch
    {
        [HarmonyPatch("CreateEncyclopediaPages")]
        [HarmonyPostfix]
        public static void CreateEncyclopediaPages_Postfix(EncyclopediaManager __instance)
        {
            Dictionary<Type, EncyclopediaPage> pages = Traverse.Create(__instance)
                .Field("_pages")
                .GetValue<Dictionary<Type, EncyclopediaPage>>();
            pages[typeof(Settlement)] = Activator.CreateInstance(typeof(EncyclopediaSettlementPage)) as EncyclopediaPage;
        }

        public static bool Prepare()
        {
            return Options.Encyclopedia.Settlement.Group.Enabled;
        }
    }
}
