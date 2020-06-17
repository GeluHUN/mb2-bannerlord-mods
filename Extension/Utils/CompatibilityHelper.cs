using System.Linq;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace Extension.Utils
{
    class CompatibilityHelper
    {
        public static CompatibilityHelper Instance => Singleton.Instance;
        public static string GameVersion => "e1.4.2";

        readonly Dictionary<string, ApplicationVersion> ExpectedGameModules = new Dictionary<string, ApplicationVersion>()
        {
            { "Native", ApplicationVersion.FromString(GameVersion, ApplicationVersionGameType.Singleplayer)},
            { "Sandbox", ApplicationVersion.FromString(GameVersion, ApplicationVersionGameType.Singleplayer)},
            { "SandBoxCore", ApplicationVersion.FromString(GameVersion, ApplicationVersionGameType.Singleplayer)},
            { "StoryMode", ApplicationVersion.FromString(GameVersion, ApplicationVersionGameType.Singleplayer)}
        };

        public bool CheckGameVersion()
        {
            bool result = (from e in ExpectedGameModules
                           from m in ModuleInfo.GetModules()
                           where m.Id == e.Key && m.Version == e.Value
                           select m).Count() == ExpectedGameModules.Count;
            if (result)
            {
                Helper.LogMessage($"Game version OK");
            }
            else
            {
                Helper.LogMessage($"Game version is WRONG");
            }
            return result;
        }

        public bool CheckOtherModules(string ModuleId)
        {
            bool result = true;
            foreach (ModuleInfo mod in from m in ModuleInfo.GetModules()
                                       where m.Id != ModuleId
                                             && m.IsSelected
                                             && m.Id != "Native"
                                             && m.Id != "Sandbox"
                                             && m.Id != "SandBoxCore"
                                             && m.Id != "StoryMode"
                                             && m.Id != "CustomBattle"
                                       select m)
            {
                result = false;
                Helper.LogMessage($"Unknown mod found: {mod.Name} {mod.Version}");
            }
            if (result)
            {
                Helper.LogMessage($"No other mods found");
            }
            return result;
        }

        CompatibilityHelper()
        {
        }

        private class Singleton
        {
            static Singleton()
            {
            }

            internal static readonly CompatibilityHelper Instance = new CompatibilityHelper();
        }
    }
}
