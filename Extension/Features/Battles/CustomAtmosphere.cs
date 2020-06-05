using Extension.Config;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;

namespace Extension.Features.Battles
{
    class CustomAtmosphere
    {
        static internal bool IsGroupEnabled()
        {
            return Options.Battles.Atmosphere.Group.Enabled;
        }
    }

    [HarmonyPatch(typeof(DefaultMapWeatherModel))]
    class CustomTimeOfDay : CustomAtmosphere
    {
        static bool EnableTimeOfDay => Options.Battles.Atmosphere.CustomTimeOfDay.EnableTimeOfDay.Value;
        static float TimeOfDay => Options.Battles.Atmosphere.CustomTimeOfDay.TimeOfDay.Value;

        [HarmonyPatch("GetHourOfDay")]
        [HarmonyPrefix]
        internal static bool GetHourOfDay_Prefix(ref float __result)
        {
            __result = TimeOfDay;
            return false;
        }

        static internal bool Prepare()
        {
            return IsGroupEnabled() && EnableTimeOfDay;
        }

        static internal void Initialize_Configuration()
        {
            Options.Battles.Atmosphere.CustomTimeOfDay.EnableTimeOfDay.Set(
                value: true,
                defaultValue: true);
            Options.Battles.Atmosphere.CustomTimeOfDay.TimeOfDay.Set(
                value: 12,
                defaultValue: 12,
                min: 0,
                max: 23);
        }
    }

    class MapWeatherModelExt : DefaultMapWeatherModel
    {
        static internal void Initialize_Configuration()
        {
            Options.Battles.Atmosphere.Group.Classes.Add(typeof(MapWeatherModelExt));
        }
    }
}
