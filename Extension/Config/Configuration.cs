using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace Extension.Config
{
    sealed class Configuration : IEnumerable<Category>, IEnumerable
    {
        public Category this[string key] => Categories[key];

        readonly string FileName;
        string ModuleId => Module.ModuleId;
        readonly Dictionary<string, Category> Categories = new Dictionary<string, Category>();

        public static Configuration Instance => Singleton.Instance;

        Configuration()
        {
            FileName = Utilities.GetConfigsPath() + $"{ModuleId}Config.txt";
        }

        public void Initialize()
        {
            foreach (Type t in from t in Assembly.GetExecutingAssembly().GetTypes()
                               where AccessTools.Method(t, "Initialize_Configuration") != null
                               select t)
            {

                Traverse.Create(t)
                    .Method("Initialize_Configuration")
                    .GetValue();
            }
        }

        public void Add(Category category)
        {
            Categories.Add(category.Id, category);
        }

        public void ForEach(Action<Category> action)
        {
            foreach (Category category in this)
            {
                action(category);
            }
        }

        public IEnumerator<Category> GetEnumerator()
        {
            return Categories.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Categories.Values.GetEnumerator();
        }

        public void Save(ApplicationVersion version)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{ModuleId}.Version={version}\n");
            foreach ((Category category, ChildElement child) in from category in Categories.Values
                                                                from child in category
                                                                select (category, child))
            {
                if (child is Group group)
                {
                    sb.Append($"{category.Id}.{group.Id}.Enabled={group.Enabled}\n");
                    foreach (Option option in group)
                    {
                        sb.Append($"{category.Id}.{group.Id}.{option.Id}={option.GetValue()}\n");
                    }
                }
                else if (child is Option option)
                {
                    sb.Append($"{category.Id}.{option.Id}={option.GetValue()}\n");
                }
            }
            File.WriteAllText(FileName, sb.ToString());
        }

        public void Load(ApplicationVersion version)
        {
            if (!File.Exists(FileName))
            {
                Save(version);
                return;
            }
            Dictionary<string, string> configValues = (
                from values in
                    from line in File.ReadAllText(FileName).Split('\n')
                    where line.Length > 0
                          && line.Contains('=')
                    select line.Split('=')
                select new { key = values[0], value = values[1] })
                .ToDictionary(x => x.key, x => x.value);
            foreach ((Category category, ChildElement child) in from category in Categories.Values
                                                                from child in category
                                                                select (category, child))
            {
                if (child is Group group)
                {
                    if (configValues.TryGetValue($"{category.Id}.{group.Id}.Enabled", out string groupValue)
                        && bool.TryParse(groupValue, out bool boolValue))
                    {
                        group.Enabled = boolValue;
                    }
                    foreach (Option option in group)
                    {
                        if (configValues.TryGetValue($"{category.Id}.{group.Id}.{option.Id}", out string optionValue))
                        {
                            object value = option.FromString(optionValue);
                            if (value != null)
                            {
                                option.SetValue(option.MakeValid(value));
                            }
                        }
                    }
                }
                else if (child is Option option)
                {
                    if (configValues.TryGetValue($"{category.Id}.{option.Id}", out string s))
                    {
                        option.SetValue(option.MakeValid(option.FromString(s)));
                    }
                }
            }
            if (configValues.TryGetValue(ModuleId, out string strVersion))
            {
                ApplicationVersion configVersion = ApplicationVersion.FromString(strVersion, ApplicationVersionGameType.Singleplayer);
                if (!version.Equals(configVersion))
                {
                    Save(version);
                }
            }
            else
            {
                Save(version);
            }
        }

        private class Singleton
        {
            static Singleton()
            {
            }

            internal static readonly Configuration Instance = new Configuration();
        }
    }
}
