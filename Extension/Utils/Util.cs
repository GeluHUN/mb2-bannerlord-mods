using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Extension.Utils
{
    static class Helper
    {
        public static Campaign TheCampaign => Campaign.Current;
        public static Game TheGame => Game.Current;

        public static void DisplayMessage(string msg)
        {
            DisplayMessage(msg, Colors.White);
        }

        public static void DisplayMessage(string msg, Color color)
        {
            InformationManager.DisplayMessage(new InformationMessage(msg, color));
        }

        public static T GetCampaignBehavior<T>()
            where T : CampaignBehaviorBase
        {
            return (Game.Current?.GameType as Campaign)?.GetCampaignBehavior<T>();
        }

        public static void ForEach<T>(this MBBindingList<T> list, Action<T> action)
        {
            foreach (T item in list)
            {
                action(item);
            }
        }

        public static bool IsPlayerFaction(this IFaction faction)
        {
            if (faction == null)
            {
                return false;
            }
            if (faction.IsBanditFaction)
            {
                return false;
            }
            if (faction.IsClan)
            {
                Clan clan = faction as Clan;
                if (clan == Clan.PlayerClan)
                {
                    return true;
                }
                if (clan.Kingdom != null && clan.Kingdom == Clan.PlayerClan.Kingdom)
                {
                    return true;
                }
            }
            if (faction.IsKingdomFaction)
            {
                Kingdom kingdom = faction as Kingdom;
                if (kingdom == Clan.PlayerClan.Kingdom)
                {
                    return true;
                }
            }
            return false;
        }

        public static void LogException(MethodBase mehtod, Exception error)
        {
            Debug.Print($"{Module.ModuleId} encountered the following problem during {mehtod.DeclaringType.Name}.{mehtod.Name}: {error.Message}\nError stack:\n{error.StackTrace}");
        }

        public static void LogMessage(string message)
        {
            Debug.Print($"{Module.ModuleId} {Module.Version} {message}");
        }

        public static void LogFunctionStart(MethodBase mehtod)
        {
            Debug.Print($"{Module.ModuleId} {Module.Version} {mehtod.DeclaringType.Name}.{mehtod.Name} started");
        }

        public static void LogFunctionEnd(MethodBase mehtod)
        {
            Debug.Print($"{Module.ModuleId} {Module.Version} {mehtod.DeclaringType.Name}.{mehtod.Name} finished");
        }
    }
}
