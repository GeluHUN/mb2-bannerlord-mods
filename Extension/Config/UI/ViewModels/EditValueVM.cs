using Extension.Utils;
using System;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace Extension.Config.UI
{
    class EditValueVM<ValueType, OptionType> : ViewModel
        where ValueType : struct
        where OptionType : TypedOption<ValueType>
    {
        protected OptionType Option { get; }
        
        readonly ValueType OriginalValue;
        readonly Action<ValueType> OnSave;

        string Text;

        public EditValueVM(OptionType option, ValueType value, Action<ValueType> onSave)
            : base()
        {
            OnSave = onSave;
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
                OnSave((ValueType)Option.MakeValid(Option.FromString(AsText)));
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

    class EditPercentValueVM : EditValueVM<int, IntOption>
    {
        public EditPercentValueVM(PercentOption percentOption, int value, Action<int> onSave)
            : base(
                  IntOption.Create(
                      percentOption.Id,
                      null,
                      percentOption.Name,
                      percentOption.Hint),
                  value,
                  onSave)
        {
            Option.Set((int)percentOption.Value * 100, (int)percentOption.Default * 100, 0, 100);
        }
    }
}
