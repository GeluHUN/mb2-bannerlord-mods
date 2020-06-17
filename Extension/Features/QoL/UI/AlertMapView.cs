using SandBox.GauntletUI.Map;
using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;

namespace Extension.Features.QoL.UI
{
    class AlertMapView : MapView
    {
        AlertsVM DataSource;
        GauntletMovie Movie;
        GauntletLayer Layer;
        bool Visible = true;

        public AlertMapView()
        {
        }

        public AlertMapView(AlertsVM alertsVM)
        {
            DataSource = alertsVM;
        }

        protected override void CreateLayout()
        {
            base.CreateLayout();
            GauntletMapBasicView mapView = MapScreen.GetMapView<GauntletMapBasicView>();
            Layer = mapView.GauntletLayer;
            Movie = Layer.LoadMovie("AlertsScreen", DataSource);
        }

        protected override void OnFinalize()
        {
            Layer.ReleaseMovie(Movie);
            Movie = null;
            DataSource = null;
            Layer = null;
            base.OnFinalize();
        }

        public void ToggleVisible()
        {
            Visible = !Visible;
            Movie.Context.Root.IsVisible = Visible;
        }

        public static AlertMapView AddAlertView(AlertsVM alertsVM)
        {
            return MapScreen.Instance.AddMapView<AlertMapView>(new object[] { alertsVM }) as AlertMapView;
        }
    }
}
