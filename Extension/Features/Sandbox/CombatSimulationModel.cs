using Extension.Config;
using Extension.Utils;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Extension.Features.Sandbox
{
    class FactionAdvantage
    {
        readonly string FactionId;
        readonly string FactionName;
        readonly string AttackStrengthId;
        readonly string DefenseStrengthId;

        public float DefenderAdvantage => (Options.Sandbox.SimulatedBattle.Group[DefenseStrengthId] as FloatOption).Value;
        public float AttackerAdvantage => (Options.Sandbox.SimulatedBattle.Group[AttackStrengthId] as FloatOption).Value;

        public static FactionAdvantage Create(string factionId, string factionName, float attackStrength, float defenseStrength)
        {
            FactionAdvantage result = new FactionAdvantage(factionId, factionName);
            result.AddOption(attackStrength, defenseStrength);
            return result;
        }

        public bool IsSameFaction(IFaction faction)
        {
            return faction.StringId == FactionId;
        }

        FactionAdvantage(string factionId, string factionName)
        {
            FactionId = factionId;
            FactionName = factionName;
            AttackStrengthId = $"{FactionId}_attackerstrength";
            DefenseStrengthId = $"{FactionId}_defensestrength";
        }

        void AddOption(float attackStrength, float defenseStrength)
        {
            FloatOption.Create(AttackStrengthId, Options.Sandbox.SimulatedBattle.Group,
                name: $"{FactionName} attack strength",
                hint: new RichtextBuilder()
                    .Append($"{FactionName} attack strength in simulated battles. The game value is multiplied by this number.")
                    .AppendDoubleLine()
                    .Append("Original value is 1.")
                    .ToString(),
                value: attackStrength,
                defaultValue: attackStrength,
                min: 0,
                max: 2);
            FloatOption.Create(DefenseStrengthId, Options.Sandbox.SimulatedBattle.Group,
                name: $"{FactionName} defense strength",
                hint: new RichtextBuilder()
                    .Append($"{FactionName} defense strength in simulated battles. The game value is multiplied by this number.")
                    .AppendDoubleLine()
                    .Append("Original value is 1.")
                    .ToString(),
                value: defenseStrength,
                defaultValue: defenseStrength,
                min: -1,
                max: 2);
        }
    }

    class CombatSimulationModelEx : DefaultCombatSimulationModel
    {
        static readonly List<FactionAdvantage> FactionAdvantages = new List<FactionAdvantage>();

        bool MountsDontCountInSiege => Options.Sandbox.SimulatedBattle.CombatSimulationModel.MountsDontCountInSiege.Value;
        bool UnbreakableDefenses => Options.Sandbox.SimulatedBattle.CombatSimulationModel.UnbreakableDefenses.Value;

        public override int SimulateHit(CharacterObject strikerTroop, CharacterObject strikedTroop, PartyBase strikerParty, PartyBase strikedParty, float strikerAdvantage, MapEvent battle)
        {
            if (battle.IsSiegeAssault && MountsDontCountInSiege)
            {
                return (int)((0.5f + 0.5f * MBRandom.RandomFloat) * (40f * MathF.Pow(GetPower(strikerTroop) / GetPower(strikedTroop), 0.7f) * strikerAdvantage));
            }
            else
            {
                return (int)((0.5f + 0.5f * MBRandom.RandomFloat) * (40f * MathF.Pow(strikerTroop.GetPower() / strikedTroop.GetPower(), 0.7f) * strikerAdvantage));
            }
        }

        float GetPower(CharacterObject strikerTroop)
        {
            int num = strikerTroop.IsHero ? (strikerTroop.HeroObject.Level / 4 + 1) : strikerTroop.Tier;
            return (2 + num) * (10 + num) * 0.02f * (strikerTroop.IsHero ? 1.5f : 1f);
        }

        public override (float defenderAdvantage, float attackerAdvantage) GetBattleAdvantage(PartyBase defenderParty, PartyBase attackerParty, MapEvent.BattleTypes mapEventType, Settlement settlement)
        {
            if (UnbreakableDefenses
                && (mapEventType == MapEvent.BattleTypes.Siege
                    || mapEventType == MapEvent.BattleTypes.Raid)
                && (attackerParty != PartyBase.MainParty
                    || defenderParty != PartyBase.MainParty))
            {
                return (1.0f, 0.0f);
            }
            (float defenderAdvantage, float attackerAdvantage) = base.GetBattleAdvantage(defenderParty, attackerParty, mapEventType, settlement);
            foreach (FactionAdvantage factionAdvantage in FactionAdvantages)
            {
                if (factionAdvantage.IsSameFaction(defenderParty.MapFaction))
                {
                    defenderAdvantage *= factionAdvantage.DefenderAdvantage;
                }
                if (factionAdvantage.IsSameFaction(attackerParty.MapFaction))
                {
                    attackerAdvantage *= factionAdvantage.AttackerAdvantage;
                }
            }
            return (defenderAdvantage, attackerAdvantage);
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.SimulatedBattle.CombatSimulationModel.MountsDontCountInSiege.Set(
                value: true,
                defaultValue: true);
            Options.Sandbox.SimulatedBattle.CombatSimulationModel.UnbreakableDefenses.Set(
                value: false,
                defaultValue: false);
            Options.Sandbox.SimulatedBattle.Group.Classes.Add(typeof(CombatSimulationModelEx));
            FactionAdvantages.Add(FactionAdvantage.Create("player_faction", "Player", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("empire", "Northern Empire", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("empire_w", "Western Empire", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("empire_s", "Southern Empire", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("sturgia", "Sturgia", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("aserai", "Aserai", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("vlandia", "Vlandia", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("battania", "Battania", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("khuzait", "Khuzait", 1.0f, 1.0f));
            FactionAdvantages.Add(FactionAdvantage.Create("looters", "Looters", 0.5f, 0.5f));
            FactionAdvantages.Add(FactionAdvantage.Create("sea_raiders", "Sea Raiders", 0.8f, 0.8f));
            FactionAdvantages.Add(FactionAdvantage.Create("mountain_bandits", "Mountain Bandits", 0.8f, 0.8f));
            FactionAdvantages.Add(FactionAdvantage.Create("forest_bandits", "Forest Bandits", 0.8f, 0.8f));
            FactionAdvantages.Add(FactionAdvantage.Create("desert_bandits", "Desert Bandits", 0.8f, 0.8f));
            FactionAdvantages.Add(FactionAdvantage.Create("steppe_bandits", "Steppe Bandits", 0.8f, 0.8f));
        }
    }
}
