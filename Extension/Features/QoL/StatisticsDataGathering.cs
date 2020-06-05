using Extension.Config;
using Extension.Features.QoL.UI;
using Extension.Utils;
using HarmonyLib;
using SandBox.View.Map;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;

namespace Extension.Features.QoL
{
    class KingdomData
    {
        public readonly float Strength;
        public readonly int Towns;
        public readonly int Castles;
        public readonly int Villages;
        public readonly float Militia;
        public readonly int Troops;
        public readonly int Garrison;
        public readonly int Heroes;
        public readonly int Wanderers;
        public readonly int Clans;
        public readonly int Parties;
        public readonly int Cash;
        public readonly int Wealth;
        public readonly int Wars;

        public KingdomData(Kingdom kingdom)
        {
            Strength = kingdom.TotalStrength;
            Towns = (from s in kingdom.Settlements where s.IsTown select s).Count();
            Castles = (from s in kingdom.Settlements where s.IsCastle select s).Count();
            Villages = (from s in kingdom.Settlements where s.IsVillage select s).Count();
            Militia = (from s in kingdom.Settlements
                       where s.IsVillage || s.IsCastle || s.IsTown
                       let m = s.IsVillage ? s.Militia : (s.IsCastle || s.IsTown ? s.Town.Militia : 0)
                       select m).Sum();
            Troops = (from p in kingdom.Parties
                      where p.MemberRoster != null
                      select p)
                      .Sum(p => p.MemberRoster.TotalManCount);
            Garrison = (from s in kingdom.Settlements
                        where (s.IsTown || s.IsCastle)
                              && s.Town.GarrisonParty != null
                        select s).Sum(s => s.Town.GarrisonParty.MemberRoster.TotalManCount);
            Heroes = kingdom.Heroes.Where(h => h.IsAlive
                                               && h.IsActive
                                               && h.IsTemplate == false).Count();
            Clans = kingdom.Clans.Count();
            Parties = kingdom.Parties.Count();
            Wealth = GetTotalGold(kingdom);
            Cash = kingdom.RulingClan.Gold;
            Wars = GetNumberOfWars(kingdom);
            Wanderers = kingdom.Heroes.Where(h => h.IsWanderer
                                                  && h.IsAlive
                                                  && h.IsActive
                                                  && h.IsTemplate == false).Count();
        }

        int GetTotalGold(Kingdom kingdom)
        {
            int gold = (from s in kingdom.Settlements
                        where s.IsVillage || s.IsCastle || s.IsTown
                        let g = s.IsVillage ? s.Village.Gold : s.Town.Gold
                        select g)
                        .Sum();
            gold += (from h in kingdom.Heroes
                     select h.Gold)
                     .Sum();
            return gold;
        }

        int GetNumberOfWars(Kingdom kingdom)
        {
            List<StanceLink> stances = (List<StanceLink>)
                Traverse.Create(kingdom)
                        .Field("_stances")
                        .GetValue();
            return (from s in stances
                    where s.IsAtWar
                          && s.Faction1.IsKingdomFaction
                          && s.Faction2.IsKingdomFaction
                    select s).Count();
        }
    }

    class WorldData
    {
        public readonly int Gold;
        public readonly float Militia;
        public readonly int Troops;
        public readonly int Garrison;
        public readonly int Wars;
        public readonly int Heroes;
        public readonly int Wanderers;
        public readonly int Clans;
        public readonly int Parties;
        public readonly int Raids;
        public readonly int Sieges;
        public readonly int Towns;
        public readonly int Castles;
        public readonly int Villages;

        public WorldData()
        {
            Gold = GetTotalGold();
            Militia = (from s in Settlement.All
                       where s.IsVillage || s.IsCastle || s.IsTown
                       let m = s.IsVillage ? s.Militia : (s.IsCastle || s.IsTown ? s.Town.Militia : 0)
                       select m).Sum();
            Troops = (from p in MobileParty.All
                      select p).Sum(p => p.MemberRoster.TotalManCount);
            Garrison = (from s in Settlement.All
                        where (s.IsTown || s.IsCastle)
                              && s.Town.GarrisonParty != null
                        select s).Sum(s => s.Town.GarrisonParty.MemberRoster.TotalManCount);
            Wars = GetNumberOfWars();
            Heroes = Hero.All.Where(h => h.IsAlive
                                         && h.IsActive
                                         && h.IsTemplate == false).Count();
            Wanderers = Hero.All.Where(h => h.IsWanderer
                                            && h.IsAlive
                                            && h.IsActive
                                            && h.IsTemplate == false).Count();
            Clans = Clan.All.Count;
            Parties = MobileParty.All.Count;
            Raids = GetTotalRaids();
            Sieges = GetTotalSieges();
            Towns = (from s in Settlement.All where s.IsTown select s).Count();
            Castles = (from s in Settlement.All where s.IsCastle select s).Count();
            Villages = (from s in Settlement.All where s.IsVillage select s).Count();
        }

        int GetTotalSieges()
        {
            List<MapEvent> events = (List<MapEvent>)
                Traverse.Create(Helper.TheCampaign.MapEventManager)
                        .Field("mapEvents")
                        .GetValue();
            return (from e in events
                    where e.IsSiegeAssault || e.IsSiegeOutside
                    select e).Count();
        }

        int GetTotalRaids()
        {
            List<MapEvent> events = (List<MapEvent>)
                Traverse.Create(Helper.TheCampaign.MapEventManager)
                        .Field("mapEvents")
                        .GetValue();
            return (from e in events
                    where e.IsRaid
                    select e).Count();
        }

        int GetTotalGold()
        {
            int gold = (from s in Settlement.All
                        where s.IsVillage || s.IsCastle || s.IsTown
                        let g = s.IsVillage ? s.Village.Gold : s.Town.Gold
                        select g)
                        .Sum();
            gold += (from h in Hero.All
                     select h.Gold)
                     .Sum();
            return gold;
        }

        int GetNumberOfWars()
        {
            List<StanceLink> globalStances = new List<StanceLink>();
            foreach (Kingdom kingdom in Kingdom.All)
            {
                List<StanceLink> stances = (List<StanceLink>)
                    Traverse.Create(kingdom)
                            .Field("_stances")
                            .GetValue();
                foreach (StanceLink stence in stances)
                {
                    if (!globalStances.Contains(stence))
                    {
                        globalStances.Add(stence);
                    }
                }
            }
            return (from s in globalStances
                    where s.IsAtWar
                          && s.Faction1.IsKingdomFaction
                          && s.Faction2.IsKingdomFaction
                    select s).Count();
        }
    }

    class StatData
    {
        public WorldData World { get; }
        public Dictionary<string, KingdomData> Kingdoms { get; }

        public StatData()
        {
            Kingdoms = new Dictionary<string, KingdomData>();
            foreach (Kingdom k in Kingdom.All)
            {
                Kingdoms.Add(k.StringId, new KingdomData(k));
            }
            World = new WorldData();
        }
    }

    class Statistics
    {
        readonly Dictionary<float, StatData> Data = new Dictionary<float, StatData>();

        // TODO ability to save the collected data

        public StatData Collect(float time)
        {
            if (Data.TryGetValue(time, out StatData result) == false)
            {
                Data.Add(time, result = new StatData());
            }
            return result;
        }
    }

    class StatisticsHotKeyCategory : GameKeyContext
    {
        public static string CategoryId => "StatisticsHotKeyCategory";
        public static bool Initialized { get; private set; } = false;

        StatisticsHotKeyCategory()
            : base(CategoryId, 0)
        {
            RegisterHotKey(new HotKey("ShowStatistics", CategoryId, InputKey.S, HotKey.Modifiers.Control, HotKey.Modifiers.None), true);
        }

        public static void Remove()
        {
            Initialized = false;
        }

        static void Initialize()
        {
            Dictionary<string, GameKeyContext> categories = (Dictionary<string, GameKeyContext>)
                Traverse.Create(typeof(HotKeyManager))
                        .Field("_categories")
                        .GetValue();
            if (categories.TryGetValue(CategoryId, out GameKeyContext category) == false)
            {
                categories.Add(CategoryId, category = new StatisticsHotKeyCategory());
            }
            InputContext context = (InputContext)MapScreen.Instance?.Input;
            if (context != null)
            {
                if (context.IsCategoryRegistered(category) == false)
                {
                    context.RegisterHotKeyCategory(category);
                    Initialized = true;
                }
            }
        }

        public static bool IsHotKeyPressed()
        {
            if (Initialized == false)
            {
                Initialize();
                return false;
            }
            return MapScreen.Instance.Input.IsHotKeyPressed("ShowStatistics");
        }
    }

    class StatisticsDataGathering : CampaignBehaviorExt
    {
        readonly Statistics Data = new Statistics();
        StatisticScreen Screen;

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            // TODO test StatisticsDataLogHelper and its usage
            //StatisticsDataLogHelper.StartCapturing();
            StatisticsHotKeyCategory.Remove();
            Module.Instance.GameEndEvent += OnGameEnd;
            Module.Instance.ApplicationTickEvent += OnApplicationTick;
            Screen = new StatisticScreen();
            StatData data = Data.Collect(TaleWorlds.CampaignSystem.Campaign.CurrentTime);
            Screen.UpdateData(data);
        }

        void OnGameEnd(Game game)
        {
            StatisticsHotKeyCategory.Remove();
            if (Screen != null)
            {
                Screen.OnFinalize();
                Screen = null;
            }
        }

        protected override void OnDailyTick()
        {
            //StatisticsDataLogger.Save();
            if (Helper.TheCampaign == null)
            {
                return;
            }
            StatData data = Data.Collect(TaleWorlds.CampaignSystem.Campaign.CurrentTime);
            Screen.UpdateData(data);
        }

        void OnApplicationTick(float dt)
        {
            if (MapScreen.Instance == null)
            {
                return;
            }
            if (StatisticsHotKeyCategory.IsHotKeyPressed())
            {
                Screen.ToggleVisible();
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.QoL.Statistics.Group.Classes.Add(typeof(StatisticsDataGathering));
        }
    }
}
