using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;

namespace Extension.Features.QoL.UI
{
    class AlertsScreen : GlobalLayer
    {
        readonly AlertsVM DataSource;
        readonly GauntletMovie Movie;
        bool Visible = true;

        public AlertsScreen(AlertsVM alertsVM)
        {
            DataSource = alertsVM;
            GauntletLayer layer = new GauntletLayer(290, "GauntletLayer");
            layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.Mouse);
            Movie = layer.LoadMovie("AlertsScreen", DataSource);
            Layer = layer;
            ScreenManager.AddGlobalLayer(this, true);
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

        public void ToggleVisible()
        {
            if (MapScreen.Instance != null && MapScreen.Instance.IsActive)
            {
                Visible = !Visible;
            }
        }
    }
}
