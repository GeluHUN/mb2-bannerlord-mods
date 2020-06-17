using HarmonyLib;
using System;
using System.IO;
using System.Reflection;

namespace Extension.Utils
{
    class HarmonyHelper
    {
        public static HarmonyHelper Instance => Singleton.Instance;
        public static string LibName => "0Harmony.dll";
        public static string LibVersion => "2.0.2.0";

        public Harmony Module { get; private set; }

        public void Initialize(string modulId)
        {
            Module = new Harmony(modulId);
        }

        public void Release()
        {
            Module = null;
        }

        public bool LoadAssembly()
        {
            string s = Path.Combine(new string[]
            {
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                LibName
            });
            Assembly harmony = Assembly.Load(AssemblyName.GetAssemblyName(s));
            Version expectedVersion = new Version(LibVersion);
            if (expectedVersion == harmony.GetName().Version)
            {
                Helper.LogMessage($"{harmony.FullName} is OK");
                return true;
            }
            else
            {
                Helper.LogMessage($"{harmony.FullName} is WRONG");
                return false;
            }
        }

        HarmonyHelper()
        {
        }

        private class Singleton
        {
            static Singleton()
            {
            }

            internal static readonly HarmonyHelper Instance = new HarmonyHelper();
        }
    }
}
