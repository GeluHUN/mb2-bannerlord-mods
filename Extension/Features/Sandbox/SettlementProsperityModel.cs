using Extension.Config;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace Extension.Features.Sandbox
{
    class ProsperityChange
    {
        public enum ChangeEffect
        {
            EffectsBaseChange,
            EffectsProsperity
        }

        readonly string OptionId;
        protected readonly string Text;
        protected readonly int FoodStockFrom;
        protected readonly int FoodStockTo;
        protected readonly ChangeEffect Effect;
        protected float Change => (Options.Sandbox.Prosperity.Group[OptionId] as FloatOption).Value;

        protected ProsperityChange(string id, string text, int foodStockFrom, int foodStockTo, ChangeEffect effect)
        {
            OptionId = id;
            Text = text;
            FoodStockFrom = foodStockFrom;
            FoodStockTo = foodStockTo;
            Effect = effect;
        }

        public virtual bool IsValid(Town fortification)
        {
            return fortification.FoodStocks >= FoodStockFrom && fortification.FoodStocks < FoodStockTo;
        }

        public float GetChange(Town fortification, StatExplainer explanation, float baseChange)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(baseChange, explanation, null);
            float value = GetChangeValue(fortification.Prosperity, baseChange);
            explainedNumber.Add(value, new TextObject(Text, null));
            return explainedNumber.ResultNumber;
        }

        protected virtual float GetChangeValue(float currentProsperity, float baseChange)
        {
            if (Effect == ChangeEffect.EffectsProsperity)
            {
                return currentProsperity * Change;
            }
            else
            {
                return baseChange * Change;
            }
        }

        protected void AddOption(string name, string hint, float effect)
        {
            FloatOption.Create(OptionId, Options.Sandbox.SimulatedBattle.Group,
                name: name,
                hint: hint,
                value: effect,
                defaultValue: effect,
                min: 0,
                max: 2);
        }
    }

    class StarvingProsperityChange : ProsperityChange
    {
        public StarvingProsperityChange(float effect)
            : base("starving", "Starving", 0, 0, ChangeEffect.EffectsProsperity)
        {
            AddOption("Starvation", "How much prosperity changes each day when the setlement is starving.", effect);
        }

        public override bool IsValid(Town fortification)
        {
            return fortification.Settlement.IsStarving;
        }
    }

    class UnderSiegeProsperityChange : ProsperityChange
    {
        public UnderSiegeProsperityChange(float effect)
            : base("sieged", "Sieged", 0, 0, ChangeEffect.EffectsProsperity)
        {
            AddOption("Under siege", "How much prosperity changes each day when the setlement is under siege.", effect);
        }

        public override bool IsValid(Town fortification)
        {
            return fortification.IsUnderSiege;
        }
    }

    class FoodShortageProsperityChange : ProsperityChange
    {
        public FoodShortageProsperityChange(float effect)
            : base("foodshortage", "Food shortage", 0, 0, ChangeEffect.EffectsBaseChange)
        {
            AddOption("Food shortage", "How much prosperity changes each day when the setlement is loosing food.", effect);
        }

        public override bool IsValid(Town fortification)
        {
            return fortification.FoodChange < 0;
        }
    }

    class FoodExcessProsperityChange : ProsperityChange
    {
        public FoodExcessProsperityChange(float effect)
            : base("foodexcess", "Food excess", 0, 0, ChangeEffect.EffectsProsperity)
        {
            AddOption("Food excess", "How much prosperity changes each day when the setlement has some food.", effect);
        }

        public override bool IsValid(Town fortification)
        {
            return fortification.FoodChange > 0 && fortification.FoodStocks > 100;
        }
    }

    class FoodAbundanceProsperityChange : ProsperityChange
    {
        public FoodAbundanceProsperityChange(float effect)
            : base("foodabundance", "Food abundance", 0, 0, ChangeEffect.EffectsProsperity)
        {
            AddOption("Food abundance", "How much prosperity changes each day when the setlement has lots of food to spare.", effect);
        }

        public override bool IsValid(Town fortification)
        {
            return fortification.FoodChange > 0 && fortification.FoodStocks > 200;
        }
    }

    class SettlementProsperityModelExt : DefaultSettlementProsperityModel
    {
        static readonly List<ProsperityChange> ProsperityChanges = new List<ProsperityChange>();

        public override float CalculateProsperityChange(Town fortification, StatExplainer explanation)
        {
            float result = base.CalculateProsperityChange(fortification, explanation);
            foreach (ProsperityChange change in ProsperityChanges)
            {
                if (change.IsValid(fortification))
                {
                    result += change.GetChange(fortification, explanation, result);
                }
            }
            return result;
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.Prosperity.Group.Classes.Add(typeof(SettlementProsperityModelExt));
            ProsperityChanges.Add(new StarvingProsperityChange(-0.1f));
            ProsperityChanges.Add(new UnderSiegeProsperityChange(-0.05f));
            ProsperityChanges.Add(new FoodShortageProsperityChange(-0.5f));
            ProsperityChanges.Add(new FoodExcessProsperityChange(0.1f));
            ProsperityChanges.Add(new FoodAbundanceProsperityChange(0.25f));
        }
    }
}
