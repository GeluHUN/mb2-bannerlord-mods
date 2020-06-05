using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    abstract class ValueOptionVM<ValueType, OptionType> : ViewModel
        where ValueType : struct
        where OptionType : TypedOption<ValueType>
    {
        protected OptionType Option { get; }

        ValueType _value;
        readonly ValueType OriginalValue;

        public ValueOptionVM(OptionType option)
            : base()
        {
            IsValid = option != null;
            if (IsValid)
            {
                Option = option;
                OriginalValue = Value = option.Value;
            }
        }

        public void SaveValue()
        {
            if (IsValid)
            {
                Option.Value = _value;
            }
        }

        public void SetDefault()
        {
            if (IsValid)
            {
                Value = Option.Default;
            }
        }

        public void ResetOptions()
        {
            if (IsValid)
            {
                Value = OriginalValue;
            }
        }

        public void OnValueClick()
        {
            if (IsValid)
            {
                ViewModel viewModel = (EditValueVM<ValueType, OptionType>)
                    Activator.CreateInstance(
                        typeof(EditValueVM<ValueType, OptionType>),
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new object[] { Option, Value },
                        null);
                ScreenManager.PushScreen(new EditValueScreen(viewModel));
            }
        }

        [DataSourceProperty]
        public bool IsValid { get; }

        [DataSourceProperty]
        public virtual string AsText => _value.ToString();

        [DataSourceProperty]
        public ValueType Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value))
                {
                    _value = value;
                    OnPropertyChanged("Value");
                    OnPropertyChanged("AsText");
                }
            }
        }
    }

    class FloatOptionVM : ValueOptionVM<float, FloatOption>
    {
        public FloatOptionVM(FloatOption option)
            : base(option)
        {
        }

        [DataSourceProperty]
        public float Min => IsValid ? Option.Min : 0f;

        [DataSourceProperty]
        public float Max => IsValid ? Option.Max : 0f;
    }

    class IntOptionVM : ValueOptionVM<int, IntOption>
    {
        public IntOptionVM(IntOption option)
            : base(option)
        {
        }

        [DataSourceProperty]
        public int Min => IsValid ? Option.Min : 0;

        [DataSourceProperty]
        public int Max => IsValid ? Option.Max : 0;
    }

    class BoolOptionVM : ValueOptionVM<bool, BoolOption>
    {
        public BoolOptionVM(BoolOption option)
            : base(option)
        {
        }
    }

    class EnumOptionVM : ValueOptionVM<int, EnumOption>
    {
        public EnumOptionVM(EnumOption option)
            : base(option)
        {
            if (IsValid)
            {
                List<string> items = new List<string>();
                Array.ForEach(option.EnumType.GetEnumNames(), text => items.Add(text));
                EnumValues = new SelectorVM<SelectorItemVM>(items, Value, selected => Value = selected.SelectedIndex);
            }
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> EnumValues { get; }
    }

    class OptionVM : ViewModel
    {
        public OptionVM(Option option)
            : base()
        {
            Id = option.Id;
            Name = option.Name;
            HintText = option.Hint;
            BoolOption = new BoolOptionVM(option as BoolOption);
            FloatOption = new FloatOptionVM(option as FloatOption);
            IntOption = new IntOptionVM(option as IntOption);
            EnumOption = new EnumOptionVM(option as EnumOption);
            RefreshValues();
        }

        public virtual void SaveOption()
        {
            BoolOption.SaveValue();
            FloatOption.SaveValue();
            IntOption.SaveValue();
            EnumOption.SaveValue();
        }

        public void ResetOption()
        {
            BoolOption.SetDefault();
            FloatOption.SetDefault();
            IntOption.SetDefault();
            EnumOption.SetDefault();
        }

        [DataSourceProperty]
        public string Id { get; }

        [DataSourceProperty]
        public string Name { get; }

        [DataSourceProperty]
        public string HintText { get; }

        [DataSourceProperty]
        public BoolOptionVM BoolOption { get; }

        [DataSourceProperty]
        public FloatOptionVM FloatOption { get; }

        [DataSourceProperty]
        public IntOptionVM IntOption { get; }

        [DataSourceProperty]
        public EnumOptionVM EnumOption { get; }
    }

    class OptionGroupSingleVM : OptionVM
    {
        readonly Group Group;

        public OptionGroupSingleVM(Group group)
            : base(Config.BoolOption.Create(group.Id, null, group.Name, group.Hint, group.Enabled, group.Enabled))
        {
            Group = group;
        }

        public override void SaveOption()
        {
            Group.Enabled = BoolOption.Value;
        }
    }
}
