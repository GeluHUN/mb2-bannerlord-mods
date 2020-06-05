using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    class EditValueScreen : ScreenBase
    {
        GauntletLayer Layer;
        GauntletMovie Movie;
        readonly ViewModel ViewModel;

        public EditValueScreen(ViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Layer = new GauntletLayer(4100, "GauntletLayer")
            {
                IsFocusLayer = true
            };
            Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            Layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            AddLayer(Layer);
            ScreenManager.TrySetFocus(Layer);
            Movie = Layer.LoadMovie("EditValueScreen", ViewModel);
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            RemoveLayer(Layer);
            Layer.ReleaseMovie(Movie);
            Layer = null;
            Movie = null;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            if (Layer.Input.IsHotKeyReleased("Exit"))
            {
                ScreenManager.PopScreen();
            }
        }
    }
}
