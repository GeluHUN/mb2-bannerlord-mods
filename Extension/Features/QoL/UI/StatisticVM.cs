using Extension.Utils;
using System;
using System.Globalization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Extension.Features.QoL.UI
{
    class DataLine : ViewModel
    {
        public DataLine(Action<MBBindingList<DataCell>> createCells)
            : base()
        {
            createCells(DataColumns);
        }

        public virtual void Update(StatData data)
        {
            DataColumns.ForEach(c => c.Update(data));
        }

        [DataSourceProperty]
        public MBBindingList<DataCell> DataColumns { get; } = new MBBindingList<DataCell>();
    }

    class DataCell : ViewModel
    {
        StatData Data;
        readonly Kingdom Kingdom;
        readonly Func<StatData, Kingdom, string> GetText;

        public DataCell(Kingdom kingdom, Func<StatData, Kingdom, string> getText)
            : base()
        {
            Kingdom = kingdom;
            GetText = getText;
        }

        [DataSourceProperty]
        public string AsText
        {
            get
            {
                if (Helper.TheCampaign == null || Data == null)
                {
                    return "no data available";
                }
                return GetText(Data, Kingdom);
            }
        }

        public void Update(StatData data)
        {
            Data = data;
            OnPropertyChanged("AsText");
        }
    }

    class StatisticVM : ViewModel
    {
        NumberFormatInfo FormatInfo;

        public StatisticVM()
            : base()
        {
            FormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            FormatInfo.NumberGroupSeparator = " ";
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Data"));
                columsn.Add(new DataCell(null, (d, k) => "World"));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => k.Name.ToString()));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Kingdom wars"));
                columsn.Add(new DataCell(null, (d, k) => d.World.Wars.ToString()));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayInteger(d.Kingdoms[k.StringId].Wars)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Kingdom strength"));
                columsn.Add(new DataCell(null, (d, k) => ""));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayFloat(d.Kingdoms[k.StringId].Strength)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Sieges"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Sieges)));
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Raids"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Raids)));
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Towns"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Towns)));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayPercent((float)d.Kingdoms[k.StringId].Towns / d.World.Towns * 100)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Castles"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Castles)));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayPercent((float)d.Kingdoms[k.StringId].Castles / d.World.Castles * 100)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Villages"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Villages)));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayPercent((float)d.Kingdoms[k.StringId].Villages / d.World.Villages * 100)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Gold"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Gold)));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayInteger(d.Kingdoms[k.StringId].Cash)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Troops"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Troops)));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayPercent((float)d.Kingdoms[k.StringId].Troops / d.World.Troops * 100)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Militia"));
                columsn.Add(new DataCell(null, (d, k) => DisplayFloat(d.World.Militia)));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayPercent(d.Kingdoms[k.StringId].Militia / d.World.Militia * 100)));
                }
            }));
            DataLines.Add(new DataLine(columsn =>
            {
                columsn.Add(new DataCell(null, (d, k) => "Garrison"));
                columsn.Add(new DataCell(null, (d, k) => DisplayInteger(d.World.Garrison)));
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    columsn.Add(new DataCell(kingdom, (d, k) => DisplayPercent((float)d.Kingdoms[k.StringId].Garrison / d.World.Garrison * 100)));
                }
            }));
        }

        string DisplayInteger(int num)
        {
            return num.ToString("N0", FormatInfo);
        }

        string DisplayFloat(float num)
        {
            return num.ToString("N01", FormatInfo);
        }

        string DisplayPercent(float num)
        {
            return $"{num.ToString("N0", FormatInfo)}%";
        }

        public void UpdateData(StatData data)
        {
            DataLines.ForEach(i => i.Update(data));
        }

        [DataSourceProperty]
        public bool IsVisible => true;

        [DataSourceProperty]
        public MBBindingList<DataLine> DataLines { get; } = new MBBindingList<DataLine>();
    }
}
