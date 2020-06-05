using TaleWorlds.MountAndBlade;
using Extension.Config;
using TaleWorlds.CampaignSystem;

namespace Extension.Features.Battles
{
    class BiggerBattleSize : CampaignBehaviorExt
    {
        int MaximumBattleSize => Options.Battles.BattleParams.BiggerBattleSize.MaximumBattleSize.Value;

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            Module.Instance.MissionBehaviourInitializeEvent += OnMissionBehaviourInitialize;
        }

        void OnMissionBehaviourInitialize(Mission mission)
        {
            if (mission.GetMissionBehaviour<FieldBattleController>() != null
                || mission.GetMissionBehaviour<SiegeMissionController>() != null)
            {
                BannerlordConfig.BattleSize = MaximumBattleSize;
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Battles.BattleParams.BiggerBattleSize.MaximumBattleSize.Set(
                value: 2000,
                defaultValue: 1000,
                min: 100,
                max: 4000);
            Options.Battles.BattleParams.Group.Classes.Add(typeof(BiggerBattleSize));
        }
    }
}
