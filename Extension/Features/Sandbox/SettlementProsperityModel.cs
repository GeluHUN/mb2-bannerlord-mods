﻿using Extension.Config;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace Extension.Features.Sandbox
{
    abstract class TownProsperityChange
    {
        protected abstract string Text { get; }

        public abstract bool IsValid(Town town);

        public float GetChange(Town town, StatExplainer explanation, float baseChange)
        {
            float value = GetChangeValue(town, baseChange);
            if (value != 0)
            {
                ExplainedNumber explainedNumber = new ExplainedNumber(0, explanation, null);
                explainedNumber.Add(value, new TextObject(Text, null));
                return explainedNumber.ResultNumber;
            }
            else
            {
                return 0;
            }
        }

        protected abstract float GetChangeValue(Town town, float baseChange);
    }

    class PausedProsperityChange : TownProsperityChange
    {
        protected override string Text => $"Stopped development (reason: {Reason})";
        string Reason { get; set; }

        public override bool IsValid(Town town)
        {
            if (town.Settlement.IsStarving)
            {
                Reason = "starving";
            }
            else if (town.Settlement.IsUnderSiege)
            {
                Reason = "under siege";
            }
            else if (town.FoodChange < 0)
            {
                Reason = "food loss";
            }
            else
            {
                Reason = "unknown";
            }
            return town.Settlement.IsStarving
                   || town.Settlement.IsUnderSiege
                   || town.FoodChange < 0;
        }

        protected override float GetChangeValue(Town town, float baseChange)
        {
            return baseChange > 0 ? -baseChange : 0;
        }
    }

    class StarvingProsperityChange : TownProsperityChange
    {
        protected override string Text => "Starving";

        static float StarvingBase => Options.Sandbox.Prosperity.SettlementProsperityModel.StarvingBase.Value;

        public override bool IsValid(Town town)
        {
            return town.Settlement.IsStarving;
        }

        protected override float GetChangeValue(Town town, float baseChange)
        {
            return -StarvingBase * ((int)town.GetProsperityLevel() + 1);
        }
    }

    class UnderSiegeProsperityChange : TownProsperityChange
    {
        protected override string Text => "Under siege";

        static float UnderSiegeBase => Options.Sandbox.Prosperity.SettlementProsperityModel.UnderSiegeBase.Value;

        public override bool IsValid(Town town)
        {
            return town.Settlement.IsUnderSiege;
        }

        protected override float GetChangeValue(Town town, float baseChange)
        {
            return -UnderSiegeBase * ((int)town.GetProsperityLevel() + 1);
        }
    }

    class LowFoodProsperityChange : TownProsperityChange
    {
        protected override string Text => "Low on food";

        static float LowFoodBase => Options.Sandbox.Prosperity.SettlementProsperityModel.LowFoodBase.Value;

        public override bool IsValid(Town town)
        {
            return town.FoodChange < 0 && town.FoodStocks < 200 && town.FoodStocks > 0;
        }

        protected override float GetChangeValue(Town town, float baseChange)
        {
            return LowFoodBase * (-(int)town.GetProsperityLevel() + town.FoodChange / 5);
        }
    }

    class FoodShortageProsperityChange : TownProsperityChange
    {
        protected override string Text => "Food shortage";

        static float FoodShortageBase => Options.Sandbox.Prosperity.SettlementProsperityModel.FoodShortageBase.Value;

        public override bool IsValid(Town town)
        {
            return town.FoodChange < 0 && town.FoodStocks > 200;
        }

        protected override float GetChangeValue(Town town, float baseChange)
        {
            return FoodShortageBase * (-(int)town.GetProsperityLevel() + town.FoodChange / 10);
        }
    }

    class FoodExcessProsperityChange : TownProsperityChange
    {
        protected override string Text => "Food excess";

        static float FoodExcessBase => Options.Sandbox.Prosperity.SettlementProsperityModel.FoodExcessBase.Value;

        public override bool IsValid(Town town)
        {
            return town.FoodChange > 10 && town.FoodStocks > 400;
        }

        protected override float GetChangeValue(Town town, float baseChange)
        {
            return FoodExcessBase * ((int)town.GetProsperityLevel() + town.FoodChange / 10);
        }
    }

    class FoodAbundanceProsperityChange : TownProsperityChange
    {
        protected override string Text => "Food abundance";

        static float FoodAbundanceBase => Options.Sandbox.Prosperity.SettlementProsperityModel.FoodAbundanceBase.Value;

        public override bool IsValid(Town town)
        {
            return town.FoodChange > 20 && town.FoodStocks > 600;
        }

        protected override float GetChangeValue(Town town, float baseChange)
        {
            return FoodAbundanceBase * ((int)town.GetProsperityLevel() + town.FoodChange / 5);
        }
    }

    class SettlementProsperityModelExt : DefaultSettlementProsperityModel
    {
        static readonly List<TownProsperityChange> ProsperityChanges = new List<TownProsperityChange>();
        static int VillageImmigrationRange => Options.Sandbox.Prosperity.SettlementProsperityModel.VillageImmigrationRange.Value;

        public override float CalculateProsperityChange(Town fortification, StatExplainer explanation)
        {
            float result = base.CalculateProsperityChange(fortification, explanation);
            foreach (TownProsperityChange change in ProsperityChanges)
            {
                if (change.IsValid(fortification))
                {
                    result += change.GetChange(fortification, explanation, result);
                }
            }
            return result;
        }

        public override float CalculateHearthChange(Village village, StatExplainer explanation)
        {
            float result = base.CalculateHearthChange(village, explanation);
            float immigration = 0;
            StringBuilder fromList = new StringBuilder();
            foreach (Settlement settlement in from settlement in Settlement.All
                                              where (settlement.IsVillage
                                                     || settlement.IsTown
                                                     || settlement.IsCastle)
                                                    && settlement.Village != village
                                                    && settlement.Position2D.DistanceSquared(village.Settlement.Position2D) <= VillageImmigrationRange
                                              select settlement)
            {
                float value = 0;
                if (settlement.IsVillage)
                {
                    value = settlement.Village.Hearth / 1000;
                }
                else if (settlement.IsTown)
                {
                    value = settlement.Town.Prosperity / 10000;
                }
                else if (settlement.IsCastle)
                {
                    value = settlement.Militia / 500;
                }
                immigration += value;
                if (fromList.Length > 0)
                {
                    fromList.Append(", ");
                }
                fromList.Append($"{settlement.Name}: {value:N01}");
            }
            if (immigration > 0.001f)
            {
                ExplainedNumber explainedNumber = new ExplainedNumber(0, explanation, null);
                explainedNumber.Add(immigration, new TextObject($"Immigration ({fromList})", null));
                result += explainedNumber.ResultNumber;
            }
            return result;
        }

        static internal void Initialize_Configuration()
        {
            ProsperityChanges.Add(new PausedProsperityChange());
            ProsperityChanges.Add(new StarvingProsperityChange());
            ProsperityChanges.Add(new UnderSiegeProsperityChange());
            ProsperityChanges.Add(new LowFoodProsperityChange());
            ProsperityChanges.Add(new FoodShortageProsperityChange());
            ProsperityChanges.Add(new FoodExcessProsperityChange());
            ProsperityChanges.Add(new FoodAbundanceProsperityChange());
            Options.Sandbox.Prosperity.Group.Classes.Add(typeof(SettlementProsperityModelExt));
            Options.Sandbox.Prosperity.SettlementProsperityModel.VillageImmigrationRange.Set(
                value: 500,
                defaultValue: 500,
                min: 0,
                max: 2000);
            Options.Sandbox.Prosperity.SettlementProsperityModel.StarvingBase.Set(
                value: 2.0f,
                defaultValue: 2.0f,
                min: 0,
                max: 5);
            Options.Sandbox.Prosperity.SettlementProsperityModel.UnderSiegeBase.Set(
                value: 1.0f,
                defaultValue: 1.0f,
                min: 0,
                max: 5);
            Options.Sandbox.Prosperity.SettlementProsperityModel.LowFoodBase.Set(
                value: 1.5f,
                defaultValue: 1.5f,
                min: 0,
                max: 5);
            Options.Sandbox.Prosperity.SettlementProsperityModel.FoodShortageBase.Set(
                value: 1.0f,
                defaultValue: 1.0f,
                min: 0,
                max: 5);
            Options.Sandbox.Prosperity.SettlementProsperityModel.FoodExcessBase.Set(
                value: 1,
                defaultValue: 1,
                min: 0,
                max: 5);
            Options.Sandbox.Prosperity.SettlementProsperityModel.FoodAbundanceBase.Set(
                value: 1.5f,
                defaultValue: 1.5f,
                min: 0,
                max: 5);
        }
    }
}
