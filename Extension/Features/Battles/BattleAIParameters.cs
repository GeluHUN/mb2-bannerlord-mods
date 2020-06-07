using HarmonyLib;
using Extension.Config;
using TaleWorlds.MountAndBlade;
using System;

namespace Extension.Features.Battles
{
    [HarmonyPatch(typeof(Formation))]
    class GreaterDefensiveness
    {
        static float Defensiveness => Options.Battles.BattleParams.BattleAIParameters.Defensiveness.Value;

        [HarmonyPatch("UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness")]
        [HarmonyPrefix]
        public static bool UpdateAgentDrivenPropertiesBasedOnOrderDefensiveness_Prefix(Formation __instance)
        {
            int formationFactor = (int)Traverse.Create(__instance)
                                               .Field("_formationOrderDefensivenessFactor")
                                               .GetValue();
            __instance.ApplyActionOnEachUnit(delegate (Agent agent)
            {
                agent.Defensiveness = formationFactor + Defensiveness;
            });
            return false;
        }

        static internal bool Prepare()
        {
            return Options.Battles.BattleParams.Group.Enabled;
        }

        static internal void Initialize_Configuration()
        {
            Options.Battles.BattleParams.BattleAIParameters.Defensiveness.Set(
                value: 2f,
                defaultValue: 2f,
                min: 0f,
                max: 4f);
        }
    }

    [HarmonyPatch(typeof(ArrangementOrder))]
    class HoldTheShieldHigh
    {
        static bool EnableHoldTheShieldHigh => Options.Battles.BattleParams.BattleAIParameters.EnableHoldTheShieldHigh.Value;

        [HarmonyPatch("GetShieldDirectionOfUnit")]
        [HarmonyPostfix]
        public static void GetShieldDirectionOfUnit_Postfix(ref Agent.UsageDirection __result, Formation formation, Agent unit, ArrangementOrder.ArrangementOrderEnum orderEnum)
        {
            bool isUnitDetached = (bool)Traverse.Create(formation)
                                                .Method("IsUnitDetached", new Type[] { typeof(Agent) }, new object[] { unit })
                                                .GetValue();
            if (isUnitDetached)
            {
                __result = Agent.UsageDirection.DefendAny;
            }
            else if (__result == Agent.UsageDirection.None
                     && (orderEnum == ArrangementOrder.ArrangementOrderEnum.Loose
                         || orderEnum == ArrangementOrder.ArrangementOrderEnum.Scatter))
            {
                __result = Agent.UsageDirection.DefendAny;
            }
            else if (orderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall
                     && ((IFormationUnit)unit).FormationRankIndex > 0)
            {
                __result = Agent.UsageDirection.DefendUp;
            }
        }

        static internal bool Prepare()
        {
            return Options.Battles.BattleParams.Group.Enabled && EnableHoldTheShieldHigh;
        }

        static internal void Initialize_Configuration()
        {
            Options.Battles.BattleParams.BattleAIParameters.EnableHoldTheShieldHigh.Set(
                value: true,
                defaultValue: true);
        }
    }
}
