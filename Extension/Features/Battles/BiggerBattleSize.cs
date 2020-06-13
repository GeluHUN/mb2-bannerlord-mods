using TaleWorlds.MountAndBlade;
using Extension.Config;
using HarmonyLib;
using System;
using TaleWorlds.Core;

namespace Extension.Features.Battles
{
    [HarmonyPatch(typeof(MissionAgentSpawnLogic))]
    class BiggerBattleSize
    {
        static int OriginalValue;
        static int MaximumBattleSize => Options.Battles.BattleParams.BiggerBattleSize.MaximumBattleSize.Value;

        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(IMissionTroopSupplier[]), typeof(BattleSideEnum) })]
        [HarmonyPrefix]
        static public void MissionAgentSpawnLogic_Prefix()
        {
            OriginalValue = BannerlordConfig.BattleSize;
            BannerlordConfig.BattleSize = MaximumBattleSize;
        }

        [HarmonyPatch(MethodType.Constructor, new Type[] { typeof(IMissionTroopSupplier[]), typeof(BattleSideEnum) })]
        [HarmonyPostfix]
        static public void MissionAgentSpawnLogic_Postfix()
        {
            BannerlordConfig.BattleSize = OriginalValue;
        }

        static internal bool Prepare()
        {
            return Options.Battles.BattleParams.Group.Enabled;
        }

        static internal void Initialize_Configuration()
        {
            Options.Battles.BattleParams.BiggerBattleSize.MaximumBattleSize.Set(
                value: 1000,
                defaultValue: 1000,
                min: 100,
                max: 1000);
            Options.Battles.BattleParams.Group.Classes.Add(typeof(BiggerBattleSize));
        }
    }
}
