using Extension.Utils;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    class EditValueVM<ValueType, OptionType> : ViewModel
        where ValueType : struct
        where OptionType : TypedOption<ValueType>
    {
        OptionType Option { get; }
        string Text;
        readonly ValueType OriginalValue;

        public EditValueVM(OptionType option, ValueType value)
            : base()
        {
            Option = option;
            Name = option.Name;
            HintText = option.Hint;
            OriginalValue = value;
            Text = value.ToString();
        }

        public void ExecuteSave()
        {
            if (IsValid)
            {
                Option.SetValue(Option.MakeValid(Option.FromString(AsText)));
                ScreenManager.PopScreen();
            }
        }

        public void ExecuteCancel()
        {
            ScreenManager.PopScreen();
        }

        public void ExecuteReset()
        {
            AsText = OriginalValue.ToString();
        }

        [DataSourceProperty]
        public string Name { get; }

        [DataSourceProperty]
        public string HintText { get; }

        [DataSourceProperty]
        public string AsText
        {
            get => Text;
            set
            {
                if (value != Text)
                {
                    Text = value;
                    OnPropertyChanged("AsText");
                    OnPropertyChanged("IsValid");
                    OnPropertyChanged("ErrorMessage");
                }
            }
        }

        [DataSourceProperty]
        public bool IsValid
        {
            get
            {
                object value = Option.FromString(AsText);
                if (value != null)
                {
                    return Option.IsValid(value);
                }
                return false;
            }
        }

        [DataSourceProperty]
        public string ErrorMessage
        {
            get
            {
                object value = Option.FromString(AsText);
                if (value != null)
                {
                    if (Option.IsValid(value))
                    {
                        return $"Value is valid for {Option.Name}";
                    }
                    else
                    {
                        return new RichtextBuilder()
                                   .StartStyle("Red")
                                   .Append($"Value is not within valid range for {Option.Name}")
                                   .EndStyle()
                                   .ToString();
                    }
                }
                else
                {
                    return new RichtextBuilder()
                               .StartStyle("Red")
                               .Append($"Not a valid text for {typeof(ValueType).Name}")
                               .EndStyle()
                               .ToString();
                }
            }
        }
    }
}
