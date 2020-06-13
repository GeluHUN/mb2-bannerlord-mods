using Extension.Config;
using Extension.Utils;
using HarmonyLib;
using SandBox.GauntletUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.CampaignSystem.SettlementComponent;
using static TaleWorlds.CampaignSystem.WorkshopType;

namespace Extension.Features.Ecyclopedia
{
    [EncyclopediaViewModel(typeof(Settlement))]
    class EncyclopediaSettlementPageVMExt : EncyclopediaSettlementPageVM
    {
        string _workshopsText;
        string _localProductionText;
        string _boundProductionText;
        string _economyStatusText;

        public EncyclopediaSettlementPageVMExt(EncyclopediaPageArgs args)
            : base(args)
        {
        }

        public void ExecuteTrack()
        {
            if (!IsVisualTrackerSelected)
            {
                Helper.TheCampaign.VisualTrackerManager.RegisterObject(Obj as Settlement);
                IsVisualTrackerSelected = true;
            }
            else
            {
                Helper.TheCampaign.VisualTrackerManager.RemoveTrackedObject(Obj as Settlement);
                IsVisualTrackerSelected = false;
            }
            Game.Current.EventManager.TriggerEvent(new PlayerToggleTrackSettlementFromEncyclopediaEvent(Obj as Settlement, IsVisualTrackerSelected));
        }

        public override void Refresh()
        {
            Settlement settlement = Obj as Settlement;
            OnPropertyChanged("HasEconomy");
            OnPropertyChanged("EconomyText");
            EconomyStatusText = GetEconomyStatusInfo(settlement);
            WorkshopsText = GetWorkshopInfo(settlement);
            LocalProductionText = GetProductionInfo(settlement);
            BoundProductionText = GetBoundInfo(settlement);
            base.Refresh();
        }

        string GetBoundInfo(Settlement settlement)
        {
            if (!settlement.IsCastle && !settlement.IsTown)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            MBReadOnlyList<Village> boundVillages = settlement.BoundVillages;
            if (boundVillages.Count > 0)
            {
                sb.Append("Bound village productions:\n");
                for (int i = 0; i < boundVillages.Count; i++)
                {
                    Village village = boundVillages[i];
                    if (i > 0)
                    {
                        sb.Append("\n");
                    }
                    float dailyAmount = village.VillageType.GetProductionPerDay(village.VillageType.PrimaryProduction);
                    string itemName = village.VillageType.PrimaryProduction.Name.ToString();
                    sb.Append($"    {village.Name} produces {dailyAmount} {itemName} daily");
                }
            }
            else
            {
                sb.Append("No bound villages");
            }
            return sb.ToString();
        }

        string GetProductionInfo(Settlement settlement)
        {
            if (!settlement.IsVillage)
            {
                return "";
            }
            VillageType villageType = settlement.Village.VillageType;
            StringBuilder sb = new StringBuilder();
            ItemObject primaryProduction = villageType.PrimaryProduction;
            if (primaryProduction != null)
            {
                float dailyAmount = villageType.GetProductionPerDay(villageType.PrimaryProduction);
                string itemName = villageType.PrimaryProduction.Name.ToString();
                sb.Append($"Primary production is {dailyAmount} {itemName} daily\n");
            }
            else
            {
                sb.Append("Has no primary production\n");
            }
            IReadOnlyList<(ItemObject, float)> productions = villageType.Productions;
            if (productions.Count > 0)
            {
                sb.Append("    Produces daily:\n");
                for (int i = 0; i < productions.Count; i++)
                {
                    (ItemObject, float) item = productions[i];
                    if (i > 0)
                    {
                        sb.Append("\n");
                    }
                    float dailyAmount = villageType.GetProductionPerDay(item.Item1);
                    string itemName = item.Item1.Name.ToString();
                    sb.Append($"        {dailyAmount} {itemName}");
                }
            }
            else
            {
                sb.Append("    Has no production");
            }
            return sb.ToString();
        }

        string GetEconomyStatusInfo(Settlement settlement)
        {
            StringBuilder sb = new StringBuilder();
            if (settlement.IsBooming)
            {
                sb.Append("Economy is booming\n");
            }
            else
            {
                sb.Append("Economy is normal\n");
            }
            ProsperityLevel prosperityLevel = ProsperityLevel.NumberOfLevels;
            if (settlement.IsTown || settlement.IsCastle)
            {
                prosperityLevel = settlement.Town.GetProsperityLevel();
            }
            else if (settlement.IsVillage)
            {
                prosperityLevel = settlement.Village.GetProsperityLevel();
            }
            sb.Append("Prosperity level is ");
            switch (prosperityLevel)
            {
                case ProsperityLevel.Low:
                    sb.Append("low");
                    break;
                case ProsperityLevel.Mid:
                    sb.Append("medium");
                    break;
                case ProsperityLevel.High:
                    sb.Append("high");
                    break;
                default:
                    sb.Append("unknown");
                    break;
            }
            return sb.ToString();
        }

        string GetWorkshopInfo(Settlement settlement)
        {
            if (!settlement.IsTown)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            List<Workshop> workshops = new List<Workshop>(settlement.Town.Workshops);
            for (int j = 0; j < workshops.Count; j++)
            {
                Workshop workshop = workshops[j];
                if (j > 0)
                {
                    sb.Append("\n");
                }
                sb.Append($"{workshop.WorkshopType.Name} workshop (Level: {workshop.Level})\n");
                sb.Append($"    Owned by {workshop.Owner.Name} and made {workshop.ProfitMade} profit last day");
                if (!workshop.WorkshopType.StringId.Equals("artisans"))
                {
                    IReadOnlyList<Production> productions = workshop.WorkshopType.Productions;
                    if (productions.Count > 0)
                    {
                        sb.Append("\n    Produces daily:\n");
                        for (int i = 0; i < productions.Count; i++)
                        {
                            Production production = productions[i];
                            if (i > 0)
                            {
                                sb.Append("\n");
                            }
                            sb.Append("        ");
                            IReadOnlyList<(ItemCategory, int)> outputs = production.Outputs;
                            for (int x = 0; x < outputs.Count; x++)
                            {
                                (ItemCategory, int) item = outputs[x];
                                if (x > 0)
                                {
                                    sb.Append(", ");
                                }
                                sb.Append($"{item.Item1.GetName()} {item.Item2}");
                            }
                            sb.Append(" from ");
                            IReadOnlyList<(ItemCategory, int)> inputs = production.Inputs;
                            for (int x = 0; x < inputs.Count; x++)
                            {
                                (ItemCategory, int) item = inputs[x];
                                if (x > 0)
                                {
                                    sb.Append(", ");
                                }
                                sb.Append($"{item.Item1.GetName()} {item.Item2}");
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine("    Has no production");
                    }
                }
            }
            return sb.ToString();
        }

        [DataSourceProperty]
        public bool HasEconomy => Obj is Settlement;

        [DataSourceProperty]
        public string EconomyText => $"{(Obj as Settlement).Name}'s economy";

        [DataSourceProperty]
        public string WorkshopsText
        {
            get => _workshopsText;
            set
            {
                if (value != _workshopsText)
                {
                    _workshopsText = value;
                    OnPropertyChanged("WorkshopsText");
                }
            }
        }

        [DataSourceProperty]
        public string LocalProductionText
        {
            get => _localProductionText;
            set
            {
                if (value != _localProductionText)
                {
                    _localProductionText = value;
                    OnPropertyChanged("LocalProductionText");
                }
            }
        }

        [DataSourceProperty]
        public string BoundProductionText
        {
            get => _boundProductionText;
            set
            {
                if (value != _boundProductionText)
                {
                    _boundProductionText = value;
                    OnPropertyChanged("BoundProductionText");
                }
            }
        }

        [DataSourceProperty]
        public string EconomyStatusText
        {
            get => _economyStatusText;
            set
            {
                if (value != _economyStatusText)
                {
                    _economyStatusText = value;
                    OnPropertyChanged("EconomyStatusText");
                }
            }
        }
    }

    [HarmonyPatch(typeof(EncyclopediaData))]
    class EncyclopediaDataPatch
    {
        [HarmonyPatch("GetEncyclopediaPageInstance")]
        [HarmonyPrefix]
        public static bool GetEncyclopediaPageInstance_Prefix(object o, ref EncyclopediaPageVM __result, EncyclopediaData __instance)
        {
            if (o is Settlement)
            {
                MethodInfo showInMap = AccessTools.Method(typeof(EncyclopediaData), "ShowInMap");
                Action<Vec2> action = (Action<Vec2>)Delegate.CreateDelegate(typeof(Action<Vec2>), __instance, showInMap);
                __result = new EncyclopediaSettlementPageVMExt(new EncyclopediaPageArgs(o, action));
                return false;
            }
            else
            {
                return true;
            }
        }

        static internal bool Prepare()
        {
            return Options.Encyclopedia.Settlement.Group.Enabled;
        }
    }
}
