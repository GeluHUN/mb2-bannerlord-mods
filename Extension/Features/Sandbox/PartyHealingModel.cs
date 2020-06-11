using Extension.Config;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;

namespace Extension.Features.Sandbox
{
    class PartyHealingModelExt : DefaultPartyHealingModel
    {
        bool LootersCantKill => Options.Sandbox.SurvivalChances.PartyHealingModel.LootersCantKill.Value;
        bool PlayerTroopsCantDie => Options.Sandbox.SurvivalChances.PartyHealingModel.PlayerTroopsCantDie.Value;

        public override float GetSurvivalChance(PartyBase party, CharacterObject character, DamageTypes damageType, PartyBase enemyParty = null)
        {
            if (PlayerTroopsCantDie
                && (PartyBase.MainParty == party || party.Owner.Clan == Hero.MainHero.Clan))
            {
                return 1f;
            }
            if (LootersCantKill
                && enemyParty?.Culture != null
                && enemyParty.Culture.StringId == "looters")
            {
                return 1f;
            }
            return base.GetSurvivalChance(party, character, damageType, enemyParty);
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.SurvivalChances.PartyHealingModel.LootersCantKill.Set(
                value: true,
                defaultValue: true);
            Options.Sandbox.SurvivalChances.PartyHealingModel.PlayerTroopsCantDie.Set(
                value: false,
                defaultValue: false);
            Options.Sandbox.Clan.Group.Classes.Add(typeof(PartyHealingModelExt));
        }
    }
}
