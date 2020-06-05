using Extension.Resources;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    class OptionsScreen : ScreenBase
    {
        GauntletLayer Layer;
        GauntletMovie Movie;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ResourceManager.Instance.Refresh();
            Layer = new GauntletLayer(4000, "GauntletLayer")
            {
                IsFocusLayer = true
            };
            Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            Layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            AddLayer(Layer);
            ScreenManager.TrySetFocus(Layer);
            Movie = Layer.LoadMovie("OptionsScreen", new ExtensionOptionsVM());
            Utilities.SetForceVsync(true);
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            RemoveLayer(Layer);
            Layer.ReleaseMovie(Movie);
            Layer = null;
            Movie = null;
            Utilities.SetForceVsync(false);
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
