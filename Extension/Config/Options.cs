using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Extension.Config
{
    public abstract class NamedElement
    {
        public string Id { get; }
        public string Name { get; set; }
        public string Hint { get; set; }

        protected NamedElement(string id)
        {
            Id = id;
        }
    }

    public abstract class ChildElement : NamedElement
    {
        public ParentElement Parent { get; }

        protected ChildElement(string id, ParentElement parent)
            : base(id)
        {
            Parent = parent;
            if (Parent != null)
            {
                Parent.Add(this);
            }
        }
    }

    public abstract class ParentElement : ChildElement, IEnumerable<ChildElement>, IEnumerable
    {
        protected Dictionary<string, ChildElement> Childs { get; }

        public ChildElement this[string key] => Childs[key];

        protected ParentElement(string id, ParentElement parent = null)
            : base(id, parent)
        {
            Childs = new Dictionary<string, ChildElement>();
        }

        public void Add(ChildElement child)
        {
            Childs.Add(child.Id, child);
        }

        public void ForEach(Action<ChildElement> action)
        {
            foreach (ChildElement element in this)
            {
                action(element);
            }
        }

        public IEnumerator<ChildElement> GetEnumerator()
        {
            return Childs.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Childs.Values.GetEnumerator();
        }
    }

    public class Category : ParentElement
    {
        protected Category(string id)
            : base(id, null)
        {
            Configuration.Instance.Add(this);
        }

        public static Category Create(string id, string name, string hint)
        {
            return new Category(id)
            {
                Name = name,
                Hint = hint
            };
        }
    }

    public class Group : ParentElement
    {
        public List<Type> Classes { get; } = new List<Type>();
        public bool Enabled { get; set; } = true;
        public Category Category => Parent as Category;

        protected Group(string id, Category category)
            : base(id, category)
        {
        }

        public static Group Create(string id, Category category, string name, string hint, bool enabled = true)
        {
            return new Group(id, category)
            {
                Name = name,
                Hint = hint,
                Enabled = enabled
            };
        }
    }

    public abstract class Option : ChildElement
    {
        protected Option(string id, ParentElement parent)
            : base(id, parent)
        {
        }

        public abstract object GetValue();
        public abstract void SetValue(object value);
        public abstract object FromString(string strValue);
        public abstract bool IsValid(object value);
        public abstract object MakeValid(object value);
    }

    public abstract class TypedOption<T> : Option
    {
        public T Default { get; set; }

        T _value;
        public T Value
        {
            get
            {
                return (T)GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        protected TypedOption(string id, ParentElement parent)
            : base(id, parent)
        {
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            _value = (T)value;
        }

        public override object MakeValid(object value)
        {
            return value;
        }
    }

    public abstract class NumberOption<T> : TypedOption<T>
        where T : struct
    {
        public T Min { get; set; }
        public T Max { get; set; }

        protected NumberOption(string id, ParentElement parent)
            : base(id, parent)
        {
        }
    }

    public class IntOption : NumberOption<int>
    {
        protected IntOption(string id, ParentElement parent)
            : base(id, parent)
        {
        }

        public override object FromString(string strValue)
        {
            if (int.TryParse(strValue, out int result))
            {
                return result;
            }
            return null;
        }

        public override bool IsValid(object value)
        {
            if (value != null && value is int)
            {
                return (int)value >= Min && (int)value <= Max;
            }
            return false;
        }

        public override object MakeValid(object value)
        {
            return Math.Min(Math.Max((int)value, Min), Max);
        }

        public static IntOption Create(string id, ParentElement parent, string name, string hint)
        {
            return new IntOption(id, parent)
            {
                Name = name,
                Hint = hint
            };
        }

        public void Set(int value, int defaultValue = default, int min = default, int max = default)
        {
            Value = value;
            Default = defaultValue;
            Min = min;
            Max = max;
        }
    }

    public class FloatOption : NumberOption<float>
    {
        FloatOption(string id, ParentElement parent)
            : base(id, parent)
        {
        }

        public override object FromString(string strValue)
        {
            if (float.TryParse(strValue, out float result))
            {
                return result;
            }
            return null;
        }

        public override object MakeValid(object value)
        {
            return Math.Min(Math.Max((float)value, Min), Max);
        }

        public override bool IsValid(object value)
        {
            if (value != null && value is float)
            {
                return (float)value >= Min && (float)value <= Max;
            }
            return false;
        }

        public static FloatOption Create(string id, ParentElement parent, string name, string hint, float value = default, float defaultValue = default, float min = default, float max = default)
        {
            return new FloatOption(id, parent)
            {
                Name = name,
                Hint = hint,
                Value = value,
                Default = defaultValue,
                Min = min,
                Max = max
            };
        }

        public void Set(float value, float defaultValue = default, float min = default, float max = default)
        {
            Value = value;
            Default = defaultValue;
            Min = min;
            Max = max;
        }
    }

    public class PercentOption : TypedOption<float>
    {
        PercentOption(string id, ParentElement parent)
            : base(id, parent)
        {
        }

        public override object FromString(string strValue)
        {
            if (float.TryParse(strValue, out float result))
            {
                return result;
            }
            return null;
        }

        public override object MakeValid(object value)
        {
            return Math.Min(Math.Max((float)value, 0), 1);
        }

        public override bool IsValid(object value)
        {
            if (value != null && value is float)
            {
                return (float)value >= 0 && (float)value <= 1;
            }
            return false;
        }

        public static PercentOption Create(string id, ParentElement parent, string name, string hint, float value = default, float defaultValue = default)
        {
            return new PercentOption(id, parent)
            {
                Name = name,
                Hint = hint,
                Value = value,
                Default = defaultValue
            };
        }

        public void Set(float value, float defaultValue = default)
        {
            Value = value;
            Default = defaultValue;
        }
    }

    public class BoolOption : TypedOption<bool>
    {
        BoolOption(string id, ParentElement parent)
            : base(id, parent)
        {
        }

        public override object FromString(string strValue)
        {
            if (bool.TryParse(strValue, out bool result))
            {
                return result;
            }
            return null;
        }

        public override bool IsValid(object value)
        {
            return value != null && value is bool;
        }

        public static BoolOption Create(string id, ParentElement parent, string name, string hint, bool value = default, bool defaultValue = default)
        {
            return new BoolOption(id, parent)
            {
                Name = name,
                Hint = hint,
                Value = value,
                Default = defaultValue
            };
        }

        public void Set(bool value, bool defaultValue = default)
        {
            Value = value;
            Default = defaultValue;
        }
    }

    public class EnumOption : IntOption
    {
        public Type EnumType
        {
            get => _enumType;
            set
            {
                if (value != _enumType)
                {
                    _enumType = value;
                    int[] values = (int[])Enum.GetValues(value);
                    Min = values.Min();
                    Max = values.Max();
                }
            }
        }
        Type _enumType;

        EnumOption(string id, ParentElement parent)
            : base(id, parent)
        {
        }

        public static EnumOption Create(string id, ParentElement parent, string name, string hint, Type enumType, int value = default, int defaultValue = default)
        {
            return new EnumOption(id, parent)
            {
                Name = name,
                Hint = hint,
                EnumType = enumType,
                Value = value,
                Default = defaultValue
            };
        }

        public void Set(int value, int defaultValue = default)
        {
            Value = value;
            Default = defaultValue;
        }
    }
}
