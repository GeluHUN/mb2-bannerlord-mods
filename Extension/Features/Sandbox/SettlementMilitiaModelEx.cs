using Extension.Config;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace Extension.Features.Sandbox
{
    class SettlementMilitiaModelEx : DefaultSettlementMilitiaModel
    {
        float EliteTroopRateIncrease => Options.Sandbox.Militia.SettlementMilitiaModel.EliteTroopRateIncrease.Value;
        float WartimeRecruitmentRate => Options.Sandbox.Militia.SettlementMilitiaModel.WartimeRecruitmentRate.Value;

        public override void CalculateMilitiaSpawnRate(Settlement settlement, out float meleeTroopRate, out float rangedTroopRate, out float meleeEliteTroopRate, out float rangedEliteTroopRate)
        {
            base.CalculateMilitiaSpawnRate(settlement, out meleeTroopRate, out rangedTroopRate, out float baseMeleeEliteTroopRate, out float baseRangedEliteTroopRate);
            meleeEliteTroopRate = Math.Min(baseMeleeEliteTroopRate + EliteTroopRateIncrease, 1f);
            rangedEliteTroopRate = Math.Min(baseRangedEliteTroopRate + EliteTroopRateIncrease, 1f);
        }

        public override float CalculateMilitiaChange(Settlement settlement, StatExplainer explanation)
        {
            float result = base.CalculateMilitiaChange(settlement, explanation);
            ExplainedNumber explainedNumber = new ExplainedNumber(result, explanation, null);
            float extraRate = WartimeRecruitmentRate;
            if (extraRate > 1 && FactionManager.Instance.FindCampaignWarsOfFaction(settlement.OwnerClan).Any())
            {
                explainedNumber.Add(result > 0 ? result * extraRate : -result / extraRate, new TextObject("Wartime recruitment", null));
            }
            return explainedNumber.ResultNumber;
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.Militia.SettlementMilitiaModel.EliteTroopRateIncrease.Set(
                value: 0.1f,
                defaultValue: 0.1f,
                min: 0,
                max: 1f);
            Options.Sandbox.Militia.SettlementMilitiaModel.WartimeRecruitmentRate.Set(
                value: 2f,
                defaultValue: 2f,
                min: 1f,
                max: 5f);
            Options.Sandbox.Militia.Group.Classes.Add(typeof(SettlementMilitiaModelEx));
        }
    }
}
