using System;
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
    }
}
