using Extension.Config;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace Extension.Features.Sandbox
{
    class PrisonerRecruitmentCalculationModelEx : DefaultPrisonerRecruitmentCalculationModel
    {
        int ConformityChangePerHour => Options.Sandbox.Prisoners.PrisonerRecruitment.ConformityChangePerHour.Value;
        float DailyRecruitedPrisoners => Options.Sandbox.Prisoners.PrisonerRecruitment.DailyRecruitedPrisoners.Value;

        public override int GetConformityChangePerHour(PartyBase party)
        {
            return ConformityChangePerHour;
        }

        public override float[] GetDailyRecruitedPrisoners(MobileParty mainParty)
        {
            float[] result = base.GetDailyRecruitedPrisoners(mainParty);
            float multiplier = DailyRecruitedPrisoners;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] *= multiplier;
            }
            return result;
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.Prisoners.PrisonerRecruitment.ConformityChangePerHour.Set(
                value: 2,
                defaultValue: 2,
                min: 1,
                max: 10);
            Options.Sandbox.Prisoners.PrisonerRecruitment.DailyRecruitedPrisoners.Set(
                value: 2,
                defaultValue: 2,
                min: 1,
                max: 5);
            Options.Sandbox.Clan.Group.Classes.Add(typeof(PrisonerRecruitmentCalculationModelEx));
        }
    }
}
