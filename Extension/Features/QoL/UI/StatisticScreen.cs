using SandBox.View.Map;
using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;

namespace Extension.Features.QoL.UI
{
    class StatisticScreen : GlobalLayer
    {
        readonly StatisticVM DataSource;
        readonly GauntletMovie Movie;
        bool Visible = false;

        public StatisticScreen()
        {
            DataSource = new StatisticVM();
            GauntletLayer layer = new GauntletLayer(5000, "GauntletLayer");
            Movie = layer.LoadMovie("StatisticsScreen", DataSource);
            Layer = layer;
            ScreenManager.AddGlobalLayer(this, false);
        }

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);
            Movie.Context.Root.IsVisible = Visible && MapScreen.Instance != null && MapScreen.Instance.IsActive;
        }

        public void OnFinalize()
        {
            ScreenManager.RemoveGlobalLayer(this);
        }

        public void UpdateData(StatData data)
        {
            DataSource.UpdateData(data);
        }

        public void ToggleVisible()
        {
            if (MapScreen.Instance != null && MapScreen.Instance.IsActive)
            {
                Visible = !Visible;
            }
        }
    }
}
