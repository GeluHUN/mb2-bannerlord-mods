using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using Extension.Config;

namespace Extension.Features.Battles
{
    class HumanSize : CampaignBehaviorExt
    {
        float BodyCapsuleRadius => Options.Battles.BattleParams.HumanSize.BodyCapsuleRadius.Value;

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            new Traverse(Game.Current.HumanMonster)
                .Property("BodyCapsuleRadius")
                .SetValue(BodyCapsuleRadius);
        }

        static internal void Initialize_Configuration()
        {
            Options.Battles.BattleParams.HumanSize.BodyCapsuleRadius.Set(
                value: 0.45f,
                defaultValue: 0.45f,
                min: 0.1f,
                max: 1f);
            Options.Battles.BattleParams.Group.Classes.Add(typeof(HumanSize));
        }
    }
}
