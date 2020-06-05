using Extension.Utils;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    class OptionGroupVM : ViewModel
    {
        readonly bool OriginalEnabled;
        readonly Group Group;
        bool _enabled;

        public OptionGroupVM(Group group)
            : base()
        {
            Group = group;
            Id = group.Id;
            Name = group.Name;
            HintText = group.Hint;
            Enabled = OriginalEnabled = group.Enabled;
            group.ForEach(o => Options.Add(new OptionVM(o as Option)));
        }

        public void ResetOptions()
        {
            Enabled = OriginalEnabled;
            Options.ForEach(o => o.ResetOption());
        }

        public void SaveOptions()
        {
            Group.Enabled = Enabled;
            Options.ForEach(o => o.SaveOption());
        }

        [DataSourceProperty]
        public string Id { get; }

        [DataSourceProperty]
        public string Name { get; }

        [DataSourceProperty]
        public string HintText { get; }

        [DataSourceProperty]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged("Enabled");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<OptionVM> Options { get; } = new MBBindingList<OptionVM>();
    }
}
