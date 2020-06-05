using Extension.Config;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Extension.Features.Sandbox
{
    class MoreMilitiaAtStart : CampaignBehaviorExt
    {
        int StartingMilitiaMultiplier => Options.Sandbox.Militia.MoreMilitiaAtStart.StartingMilitiaMultiplier.Value;

        public override void RegisterEvents()
        {
            base.RegisterEvents();
            CampaignEvents.OnNewGameCreatedEvent9.AddNonSerializedListener(this, new Action(OnAfterNewGameCreated));
        }

        void OnAfterNewGameCreated()
        {
            foreach (Settlement settlement in from settlement in Settlement.All
                                              where settlement.IsFortification
                                                    || settlement.IsVillage
                                              select settlement)
            {
                settlement.Militia *= StartingMilitiaMultiplier;
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.Sandbox.Militia.MoreMilitiaAtStart.StartingMilitiaMultiplier.Set(
                value: 2,
                defaultValue: 2,
                min: 1,
                max: 10);
            Options.Sandbox.Militia.Group.Classes.Add(typeof(MoreMilitiaAtStart));
        }
    }
}
