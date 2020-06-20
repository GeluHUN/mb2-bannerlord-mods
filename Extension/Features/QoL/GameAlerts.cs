using System;
using System.Collections.Generic;
using System.Linq;
using Extension.Config;
using Extension.Utils;
using Extension.Features.QoL.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using System.Text;
using TaleWorlds.Library;
using HarmonyLib;
using TaleWorlds.InputSystem;
using SandBox.View.Map;

namespace Extension.Features.QoL
{
    public enum AlertLevel
    {
        Minor,
        Middle,
        Major
    }

    abstract class GameAlert
    {
        public string Id { get; }
        public AlertLevel Level { get; protected set; }
        public abstract string Title { get; }
        public abstract string ShortText { get; }
        public abstract string Description { get; }
        public virtual string AlertColor => GetColor();
        public abstract string AlertIcon { get; }
        public abstract bool HasMapCoord { get; }
        public abstract Vec2 MapCoord { get; }

        protected GameAlert()
        {
            Id = Guid.NewGuid().ToString();
        }

        public virtual List<TooltipProperty> GetTooltip()
        {
            List<TooltipProperty> result = new List<TooltipProperty>();
            result.Add(new TooltipProperty("", Title, 0, false, TooltipProperty.TooltipPropertyFlags.Title));
            result.Add(new TooltipProperty("", "", 0, false, TooltipProperty.TooltipPropertyFlags.DefaultSeperator));
            result.Add(new TooltipProperty("", ShortText, 0, false, TooltipProperty.TooltipPropertyFlags.None));
            string s = Description;
            if (s != null)
            {
                result.Add(new TooltipProperty("", "", 0, false, TooltipProperty.TooltipPropertyFlags.RundownSeperator));
                result.Add(new TooltipProperty("", s, 0, false, TooltipProperty.TooltipPropertyFlags.MultiLine));
            }
            return result;
        }

        protected virtual string GetColor()
        {
            switch (Level)
            {
                case AlertLevel.Minor:
                    return "#FFD800FF";
                case AlertLevel.Middle:
                    return "#FF8F00FF";
                case AlertLevel.Major:
                    return "#FF0000FF";
            }
            return "#000000FF";
        }
    }

    abstract class MapEventAlert : GameAlert
    {
        public MapEvent MapEvent { get; }
        public PartyBase Attacker => MapEvent.AttackerSide.LeaderParty;
        public PartyBase Defender => MapEvent.DefenderSide.LeaderParty;
        public override bool HasMapCoord => true;
        public override Vec2 MapCoord => MapEvent.Position;

        public MapEventAlert(MapEvent mapEvent)
            : base()
        {
            MapEvent = mapEvent;
        }
    }

    abstract class SettlementAlert : MapEventAlert
    {
        public Settlement Settlement { get; }
        public override string Title => Settlement.Name.ToString();

        public SettlementAlert(MapEvent mapEvent)
            : base(mapEvent)
        {
            Settlement = mapEvent.MapEventSettlement;
        }
    }

    class RaidAlert : SettlementAlert
    {
        public Village Village => Settlement.Village;
        public override string ShortText => $"{Village.Name} is beign raided";
        public override string Description => GetDescription();
        public override string AlertIcon => "alert_raid";

        public RaidAlert(MapEvent mapEvent)
            : base(mapEvent)
        {
            Level = AlertLevel.Middle;
        }

        string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Started: ");
            sb.Append(CampaignTime.Now);
            sb.Append("\n");
            sb.Append("Attacker: ");
            sb.Append(Attacker.Name);
            sb.Append("\n");
            sb.Append("Attacker strength: ");
            sb.Append(Attacker.MemberRoster.TotalHealthyCount);
            sb.Append("\n");
            sb.Append("Defender: ");
            sb.Append(Defender.Name);
            sb.Append("\n");
            sb.Append("Defender strength: ");
            sb.Append(Defender.MemberRoster.TotalHealthyCount);
            return sb.ToString();
        }
    }

    class SiegeAlert : SettlementAlert
    {
        public Town Town => Settlement.Town;
        public override string ShortText => $"{Town.Name} is being sieged";
        public override string Description => GetDescription();
        public override string AlertIcon => "alert_siege";

        public SiegeAlert(MapEvent mapEvent)
            : base(mapEvent)
        {
            Level = AlertLevel.Major;
        }

        string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Started: ");
            sb.Append(CampaignTime.Now);
            sb.Append("\n");
            sb.Append("Attacker: ");
            sb.Append(Attacker.Name);
            sb.Append("\n");
            sb.Append("Attacker strength: ");
            sb.Append(Attacker.MemberRoster.TotalHealthyCount);
            sb.Append("\n");
            sb.Append("Defender: ");
            sb.Append(Defender.Name);
            sb.Append("\n");
            sb.Append("Defender strength: ");
            sb.Append(Defender.MemberRoster.TotalHealthyCount);
            return sb.ToString();
        }
    }

    class PartyAttackedAlert : MapEventAlert
    {
        public override string Title => Defender.Leader.Name.ToString();
        public override string ShortText => $"{Defender.LeaderHero.Name} is beign attacked";
        public override string Description => GetDescription();
        public override string AlertIcon => "alert_battle";

        public PartyAttackedAlert(MapEvent mapEvent)
            : base(mapEvent)
        {
            Level = AlertLevel.Middle;
        }

        string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Started: ");
            sb.Append(CampaignTime.Now);
            sb.Append("\n");
            sb.Append("Attacker: ");
            sb.Append(Attacker.Name);
            sb.Append("\n");
            sb.Append("Attacker strength: ");
            sb.Append(Attacker.MemberRoster.TotalHealthyCount);
            sb.Append("\n");
            sb.Append("Defender: ");
            sb.Append(Defender.Name);
            sb.Append("\n");
            sb.Append("Defender strength: ");
            sb.Append(Defender.MemberRoster.TotalHealthyCount);
            return sb.ToString();
        }
    }

    class PartyAttackingAlert : MapEventAlert
    {
        public override string Title => Attacker.Leader.Name.ToString();
        public override string ShortText => GetShortText();
        public override string Description => GetDescription();
        public override string AlertIcon => "alert_battle";

        public PartyAttackingAlert(MapEvent mapEvent)
            : base(mapEvent)
        {
            Level = AlertLevel.Minor;
        }

        string GetShortText()
        {
            if (MapEvent.IsFieldBattle)
            {
                string name;
                if (Defender.LeaderHero != null)
                {
                    name = Defender.LeaderHero.Name.ToString();
                }
                else
                {
                    name = Defender.Name.ToString();
                }
                return $"{Attacker.LeaderHero.Name} is attacking {name}";
            }
            else if (MapEvent.IsRaid)
            {
                return $"{Attacker.LeaderHero.Name} is raiding {Defender.Name}";
            }
            else if (MapEvent.IsSiegeAssault)
            {
                return $"{Attacker.LeaderHero.Name} is sieging {Defender.Name}";
            }
            return Attacker.Name.ToString();
        }

        string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Started: ");
            sb.Append(CampaignTime.Now);
            sb.Append("\n");
            sb.Append("Attacker: ");
            sb.Append(Attacker.Name);
            sb.Append("\n");
            sb.Append("Attacker strength: ");
            sb.Append(Attacker.MemberRoster.TotalHealthyCount);
            sb.Append("\n");
            sb.Append("Defender: ");
            sb.Append(Defender.Name);
            sb.Append("\n");
            sb.Append("Defender strength: ");
            if (Defender.IsSettlement)
            {
                sb.Append(Defender.Settlement.MilitaParty.MemberRoster.TotalHealthyCount);
            }
            else if (Defender.IsMobile)
            {
                sb.Append(Defender.MemberRoster.TotalHealthyCount);
            }
            return sb.ToString();
        }
    }

    class StarvingAlert : GameAlert
    {
        public Town Town { get; }
        public override string Title => Town.Name.ToString();
        public override string ShortText => $"{Town.Name} is starving";
        public override string Description => GetDescription();
        public override string AlertIcon => "alert_starve";
        public override bool HasMapCoord => true;
        public override Vec2 MapCoord => Town.Settlement.Position2D;

        public StarvingAlert(Town town)
            : base()
        {
            Town = town;
            Level = AlertLevel.Major;
        }

        string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Started: ");
            sb.Append(CampaignTime.Now);
            sb.Append("\n");
            sb.Append("Town: ");
            sb.Append(Town.Name);
            sb.Append("\n");
            sb.Append("Food stock: ");
            sb.Append(Town.FoodStocks);
            sb.Append("\n");
            sb.Append("Food change: ");
            sb.Append(Town.FoodChange);
            sb.Append("\n");
            return sb.ToString();
        }
    }

    class GameAlerts
    {
        public readonly Dictionary<string, GameAlert> AlertList = new Dictionary<string, GameAlert>();

        public bool AddStarvingTown(Town town, out StarvingAlert result)
        {
            result = (from a in AlertList.Values.ToList()
                      where a is StarvingAlert
                      let sa = a as StarvingAlert
                      where sa.Town == town
                      select sa).FirstOrDefault();
            if (result == null)
            {
                result = new StarvingAlert(town);
                AlertList.Add(result.Id, result);
                return true;
            }
            return false;
        }

        public bool RemoveStarvingTown(Town town, out StarvingAlert result)
        {
            result = (from a in AlertList.Values.ToList()
                      where a is StarvingAlert
                      let sa = a as StarvingAlert
                      where sa.Town == town
                      select sa).FirstOrDefault();
            if (result != null)
            {
                AlertList.Remove(result.Id);
                return true;
            }
            return false;
        }

        public bool AddMapEvent(MapEvent mapEvent, PartyBase attacker, PartyBase defender, out MapEventAlert result)
        {
            result = null;
            if (attacker == PartyBase.MainParty || defender == PartyBase.MainParty)
            {
                return false;
            }
            if (mapEvent.EventType == MapEvent.BattleTypes.FieldBattle)
            {
                if (defender.MapFaction.IsPlayerFaction() && defender.IsMobile && defender.Leader.IsHero)
                {
                    result = new PartyAttackedAlert(mapEvent);
                    AlertList.Add(result.Id, result);
                    return true;
                }
                if (attacker.MapFaction.IsPlayerFaction() && defender.IsMobile && defender.Leader.IsHero)
                {
                    result = new PartyAttackingAlert(mapEvent);
                    AlertList.Add(result.Id, result);
                    return true;
                }
            }
            if (mapEvent.EventType == MapEvent.BattleTypes.Raid)
            {
                if (defender.MapFaction.IsPlayerFaction() && defender.IsSettlement)
                {
                    result = new RaidAlert(mapEvent);
                    AlertList.Add(result.Id, result);
                    return true;
                }
                if (attacker.MapFaction.IsPlayerFaction() && defender.IsMobile && defender.Leader.IsHero)
                {
                    result = new PartyAttackingAlert(mapEvent);
                    AlertList.Add(result.Id, result);
                    return true;
                }
            }
            if (mapEvent.EventType == MapEvent.BattleTypes.Siege)
            {
                if (defender.MapFaction.IsPlayerFaction() && defender.IsSettlement)
                {
                    result = new SiegeAlert(mapEvent);
                    AlertList.Add(result.Id, result);
                    return true;
                }
                if (attacker.MapFaction.IsPlayerFaction() && defender.IsMobile && defender.Leader.IsHero)
                {
                    result = new PartyAttackingAlert(mapEvent);
                    AlertList.Add(result.Id, result);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveMapEvent(MapEvent mapEvent, out MapEventAlert result)
        {
            result = (from a in AlertList.Values.ToList()
                      where a is MapEventAlert
                      let me = a as MapEventAlert
                      where me.MapEvent == mapEvent
                      select me).FirstOrDefault();
            if (result != null)
            {
                AlertList.Remove(result.Id);
            }
            return result != null;
        }
    }

    class GameAlertsHotKeyCategory : GameKeyContext
    {
        public static string CategoryId => "GameAlertsHotKeyCategory";
        public static bool Initialized { get; private set; } = false;

        GameAlertsHotKeyCategory()
            : base(CategoryId, 0)
        {
            RegisterHotKey(new HotKey("ShowGameAlerts", CategoryId, InputKey.A, HotKey.Modifiers.Control, HotKey.Modifiers.None), true);
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
                categories.Add(CategoryId, category = new GameAlertsHotKeyCategory());
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
            return MapScreen.Instance.Input.IsHotKeyPressed("ShowGameAlerts");
        }
    }

    class GameAlertBehavior : CampaignBehaviorExt
    {
        readonly GameAlerts GameAlerts = new GameAlerts();
        AlertMapView MapView;
        AlertsVM DataSource;

        public override void RegisterEvents()
        {
            base.RegisterEvents();
            CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(OnMapEventStarted));
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(OnMapEventEnded));
        }

        protected override void OnSessionLaunched(CampaignGameStarter starter)
        {
            base.OnSessionLaunched(starter);
            GameAlertsHotKeyCategory.Remove();
            Module.Instance.GameEndEvent += OnGameEnd;
            Module.Instance.ApplicationTickEvent += OnApplicationTick;
            DataSource = new AlertsVM();
            UpdateStarvingTownAlerts();
            UpdateMapEventAlerts();
        }

        void UpdateMapEventAlerts()
        {
            List<MapEvent> mapEvents = Traverse.Create(Helper.TheCampaign.MapEventManager)
                .Field("mapEvents")
                .GetValue() as List<MapEvent>;
            foreach (MapEvent mapEvent in mapEvents)
            {
                OnMapEventStarted(mapEvent, mapEvent.AttackerSide.LeaderParty, mapEvent.DefenderSide.LeaderParty);
            }
        }

        void OnGameEnd(Game game)
        {
            GameAlertsHotKeyCategory.Remove();
            Module.Instance.GameEndEvent -= OnGameEnd;
            Module.Instance.ApplicationTickEvent -= OnApplicationTick;
            MapView = null;
            DataSource = null;
        }

        void OnApplicationTick(float dt)
        {
            if (MapScreen.Instance == null)
            {
                return;
            }
            if (MapView == null)
            {
                MapView = AlertMapView.AddAlertView(DataSource);
            }
            if (GameAlertsHotKeyCategory.IsHotKeyPressed())
            {
                MapView.ToggleVisible();
            }
        }

        protected override void OnDailyTick()
        {
            UpdateStarvingTownAlerts();
        }

        void UpdateStarvingTownAlerts()
        {
            foreach (Town town in from s in Settlement.All
                                  where s.IsTown
                                  let t = s.Town
                                  where t.MapFaction.IsPlayerFaction()
                                  select t)
            {
                if (town.Settlement.IsStarving)
                {
                    if (GameAlerts.AddStarvingTown(town, out StarvingAlert alert))
                    {
                        DataSource.AddAlert(alert);
                    }
                }
                else
                {
                    if (GameAlerts.RemoveStarvingTown(town, out StarvingAlert alert))
                    {
                        DataSource.RemoveAlert(alert);
                    }
                }
            }
        }

        void OnMapEventStarted(MapEvent mapEvent, PartyBase attacker, PartyBase defender)
        {
            if (defender.MapFaction.IsPlayerFaction() || attacker.MapFaction.IsPlayerFaction())
            {
                if (GameAlerts.AddMapEvent(mapEvent, attacker, defender, out MapEventAlert alert))
                {
                    DataSource.AddAlert(alert);
                }
            }
        }

        void OnMapEventEnded(MapEvent mapEvent)
        {
            if (GameAlerts.RemoveMapEvent(mapEvent, out MapEventAlert alert))
            {
                DataSource.RemoveAlert(alert);
            }
        }

        static internal void Initialize_Configuration()
        {
            Options.QoL.Alerts.Group.Classes.Add(typeof(GameAlertBehavior));
        }
    }
}
