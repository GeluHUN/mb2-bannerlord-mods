using Extension.Utils;
using System.Linq;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    class OptionCategoryVM : ViewModel
    {
        public OptionCategoryVM(Category category)
            : base()
        {
            Id = category.Id;
            Name = category.Name;
            HintText = category.Hint;
            AddChilds(category);
        }

        public void ResetOptions()
        {
            Options.ForEach(o => o.ResetOption());
            OptionGroups.ForEach(g => g.ResetOptions());
        }

        public void SaveOptions()
        {
            Options.ForEach(o => o.SaveOption());
            OptionGroups.ForEach(g => g.SaveOptions());
        }

        void AddChilds(Category category)
        {
            foreach (ChildElement child in category)
            {
                if (child is Group group)
                {
                    if (group.Count() > 0)
                    {
                        OptionGroups.Add(new OptionGroupVM(group));
                    }
                    else
                    {
                        Options.Add(new OptionGroupSingleVM(group));
                    }
                }
                else
                {
                    Options.Add(new OptionVM(child as Option));
                }
            }
        }

        [DataSourceProperty]
        public string Id { get; }

        [DataSourceProperty]
        public string Name { get; }

        [DataSourceProperty]
        public string HintText { get; }

        [DataSourceProperty]
        public MBBindingList<OptionGroupVM> OptionGroups { get; } = new MBBindingList<OptionGroupVM>();

        [DataSourceProperty]
        public MBBindingList<OptionVM> Options { get; } = new MBBindingList<OptionVM>();
    }
}
