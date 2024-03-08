using System;
using System.Linq;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class PackageListBoxColumn(string key, string name, float value, PackageListBoxSize unit)
    {
        public string Key { get; set; } = key;
        public string Name { get; set; } = name;
        public float Value { get; set; } = value;
        public PackageListBoxSize Unit { get; set; } = unit;
    }

    public enum PackageListBoxSize { Fixed, Percent }

    internal class PackageListBox : AbstractLoadingControl<ListView>
    {
        private readonly int AggregatedMiscWidth;
        private int AggregatedFixedColumnWidth = 0;

        public PackageListBox()
        {
            AggregatedMiscWidth = Margin.Left + Margin.Right + SystemInformation.VerticalScrollBarWidth;

            Other = new ListView()
            {
                Name = "ListView",
                CheckBoxes = true,
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                MultiSelect = false,
                ShowItemToolTips = true,
                UseCompatibleStateImageBehavior = false,
                View = System.Windows.Forms.View.Details
            };

            Resize += PackageListBox_Resize;
        }

        public bool CheckBoxes
        {
            get { return Other.CheckBoxes; }
            set { Other.CheckBoxes = value; }
        }

        private bool _NoPackages = false;
        public bool NoPackages
        {
            get
            {
                return _NoPackages;
            }
            set
            {
                if (value)
                {
                    Other.CheckBoxes = false;
                    Other.HeaderStyle = ColumnHeaderStyle.None;
                    Other.ItemSelectionChanged += Other_PreventItemSelectionChange;
                }
                else
                {
                    Other.CheckBoxes = CheckBoxes;
                    Other.HeaderStyle = ColumnHeaderStyle.Nonclickable;
                    Other.ItemSelectionChanged -= Other_PreventItemSelectionChange;
                }
                _NoPackages = value;
            }
        }

        private PackageListBoxColumn[] _Columns;
        public PackageListBoxColumn[] Columns
        {
            get
            {
                return _Columns;
            }
            set
            {
                if (value  != null)
                {
                    _Columns = value;
                    AggregatedFixedColumnWidth = _Columns
                        .Where(col => col.Unit.Equals(PackageListBoxSize.Fixed))
                        .Select(col => (int)Math.Round(col.Value))
                        .Sum();
                    Other.Columns.Clear();
                    Other.Columns.AddRange(value
                        .Select(col => new ColumnHeader(col.Name) { Text = col.Name })
                        .ToArray());
                    PackageListBox_Resize(this, EventArgs.Empty);
                }
            }
        }

        public string Hint
        {
            get { return SpinnerCtl.Text; }
            set { SpinnerCtl.Text = value; }
        }

        public int IndexOfColumn(string key)
        {
            return _Columns.ToList().FindIndex(p => key.Equals(p.Key));
        }

        public PackageListBoxColumn GetColumnByKey(string key)
        {
            return _Columns.First(c => key.Equals(c.Name));
        }


        private void PackageListBox_Resize(object sender, EventArgs e)
        {
            if (Columns != null && Columns.Length.Equals(Other.Columns.Count))
            {
                int availWidth = Width - AggregatedFixedColumnWidth - AggregatedMiscWidth;
                for (int i = 0; i < Columns.Length; i++)
                {
                    var col = Columns[i];
                    if (col.Unit.Equals(PackageListBoxSize.Fixed))
                    {
                        Other.Columns[i].Width = (int)col.Value;
                    }
                    else if (col.Unit.Equals(PackageListBoxSize.Percent))
                    {
                        Other.Columns[i].Width = (int)(availWidth * col.Value);
                    }
                    else
                    {
                        Other.Columns[i].Width = 20;
                    }
                }
            }
        }

        private void Other_PreventItemSelectionChange(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            e.Item.Selected = false;
        }
    }
}
