using SandBox.View.Map;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace Extension.Features.QoL.UI
{
    class AlertItemLine : ViewModel
    {
        public readonly GameAlert Alert;

        public AlertItemLine(GameAlert alert)
            : base()
        {
            Alert = alert;
        }

        public void ShowTooltip()
        {
            InformationManager.AddTooltipInformation(typeof(List<TooltipProperty>), new object[] { Alert.GetTooltip() });
        }

        public void HideTooltip()
        {
            InformationManager.HideInformations();
        }

        public void JumpToEvent()
        {
            MapScreen.Instance.SetMapCameraPosition(Alert.MapCoord);
        }

        [DataSourceProperty]
        public string ShortText => Alert.ShortText;

        [DataSourceProperty]
        public bool HasMapCoord => Alert.HasMapCoord;

        [DataSourceProperty]
        public string AlertColor => Alert.AlertColor;

        [DataSourceProperty]
        public string AlertIcon => Alert.AlertIcon;
    }

    class AlertsVM : ViewModel
    {
        public AlertsVM()
            : base()
        {
        }

        public void AddAlert(GameAlert alert)
        {
            AlertList.Add(new AlertItemLine(alert));
            OnPropertyChanged("AlertList");
            OnPropertyChanged("IsVisible");
        }

        public void RemoveAlert(GameAlert alert)
        {
            AlertList.Remove((from a in AlertList where a.Alert == alert select a).FirstOrDefault());
            OnPropertyChanged("AlertList");
            OnPropertyChanged("IsVisible");
        }

        public void Initialize(List<GameAlert> alertList)
        {
            foreach (GameAlert alert in alertList)
            {
                AlertList.Add(new AlertItemLine(alert));
            }
            OnPropertyChanged("AlertList");
            OnPropertyChanged("IsVisible");
        }

        [DataSourceProperty]
        public bool IsVisible => AlertList.Count > 0;

        [DataSourceProperty]
        public MBBindingList<AlertItemLine> AlertList { get; } = new MBBindingList<AlertItemLine>();
    }
}
