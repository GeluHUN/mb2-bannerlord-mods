using System;
using TaleWorlds.CampaignSystem;

namespace Extension.Features
{
    class CampaignBehaviorExt : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(OnDailyTick));
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(OnTick));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(OnHourlyTick));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnGameLoaded));
            CampaignEvents.OnNewGameCreatedEvent9.AddNonSerializedListener(this, new Action(OnNewGameCreated));
        }

        protected virtual void OnNewGameCreated()
        {
        }

        protected virtual void OnGameLoaded(CampaignGameStarter starter)
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        protected virtual void OnDailyTick()
        {
        }

        protected virtual void OnHourlyTick()
        {
        }

        protected virtual void OnTick(float dt)
        {
        }

        protected virtual void OnSessionLaunched(CampaignGameStarter starter)
        {
        }
    }
}
