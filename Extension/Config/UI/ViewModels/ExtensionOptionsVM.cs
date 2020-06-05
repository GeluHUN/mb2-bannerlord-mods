using Extension.Config;
using Extension.Utils;
using System.Linq;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    class ExtensionOptionsVM : ViewModel
    {
        public ExtensionOptionsVM()
            : base()
        {
            Configuration.Instance.ForEach(c => OptionCategories.Add(new OptionCategoryVM(c)));
        }

        public void ExecuteCancel()
        {
            ScreenManager.PopScreen();
        }

        public void ExecuteReset()
        {
            OptionCategories.ForEach(c => c.ResetOptions());
        }

        public void ExecuteSave()
        {
            OptionCategories.ForEach(c => c.SaveOptions());
            Configuration.Instance.Save(Module.Instance.Version);
            ScreenManager.PopScreen();
        }

        [DataSourceProperty]
        public MBBindingList<OptionCategoryVM> OptionCategories { get; set; } = new MBBindingList<OptionCategoryVM>();
    }
}
