using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class PackageListBoxColumn(string key, string name, float value, PackageListBoxSize unit) // TODO rename to PackageListBoxColumnHeader
    {
        public string Key { get; set; } = key;
        public string Name { get; set; } = name;
        public float Value { get; set; } = value;
        public PackageListBoxSize Unit { get; set; } = unit;

        public DataGridViewColumn ToDataGridViewColumn()
        {
            DataGridViewTextBoxColumn rtn = new()
            {
                Name = Key,
                HeaderText = Name,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable, // TODO allow sorting
            };
            if (Unit.Equals(PackageListBoxSize.Percent))
            {
                rtn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                rtn.FillWeight = (int)(Value * 100);
            }
            else
            {
                rtn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                rtn.Width = (int)Value;
            }
            return rtn;
        }
    }

    public enum PackageListBoxSize { Fixed, Percent }

    internal class PackageListBox : AbstractLoadingControl<DataGridView>
    {
        // TODO on empty list, row should not be visibly selected

        private readonly DataGridViewCheckBoxColumn CheckBoxColumn;

        public PackageListBox()
        {
            Other = new DataGridView()
            {
                Name = "ListView",
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = false,
                AllowUserToResizeColumns = true,
                AllowUserToResizeRows = false,
                BackgroundColor = SystemColors.Window,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                Dock = DockStyle.Fill,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ShowCellToolTips = true
            };

            Other.CellMouseDown += new DataGridViewCellMouseEventHandler((sender, e) =>
            {
                if (e.Button == MouseButtons.Right)
                    Other.CurrentCell = Other[e.ColumnIndex, e.RowIndex];
            });

            Other.Paint += new PaintEventHandler((sender, e) =>
            {
                if (e.ClipRectangle.Equals(Other.Bounds))
                    ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);
            });

            // add checkbox column; but hide it by default
            CheckBoxColumn = new()
            {
                Name = "Checked",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                HeaderText = "",
                ReadOnly = false,
                Visible = false,
                Width = 30
            };

            Other.CellMouseUp += new DataGridViewCellMouseEventHandler((sender, e) =>
            {
                if (e.ColumnIndex == CheckBoxColumn.Index && e.RowIndex != -1)
                    Other.EndEdit();
            });

            Other.CellDoubleClick += new DataGridViewCellEventHandler((sender, e) =>
            {
                if (e.ColumnIndex == CheckBoxColumn.Index && e.RowIndex != -1)
                    Other.EndEdit();
            });

            Other.CellValueChanged += new DataGridViewCellEventHandler((sender, e) =>
            {
                if (e.ColumnIndex == CheckBoxColumn.Index && e.RowIndex != -1)
                    ItemChecked?.Invoke(sender, e);
            });
            Other.Columns.Add(CheckBoxColumn);

            // define border, grid and header styles
            Other.EnableHeadersVisualStyles = false;
            Other.BorderStyle = BorderStyle.FixedSingle;
            Other.GridColor = Color.LightGray;
            Other.ColumnHeadersDefaultCellStyle = Other.DefaultCellStyle.Clone();
            Other.ColumnHeadersDefaultCellStyle.SelectionForeColor = Other.ColumnHeadersDefaultCellStyle.ForeColor;
            Other.ColumnHeadersDefaultCellStyle.SelectionBackColor = Other.ColumnHeadersDefaultCellStyle.BackColor;
            Other.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            Other.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
            Other.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            Other.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
        }

        public event DataGridViewCellEventHandler ItemChecked;

        public bool CheckBoxes
        {
            get { return CheckBoxColumn.Visible; }
            set { CheckBoxColumn.Visible = value; }
        }

        public DataGridViewRow SelectedItem => Other.SelectedRows.Count > 0 ? Other.SelectedRows[0] : null;
        public bool TryGetSelectedItem(out DataGridViewRow value)
        {
            if (Other.SelectedRows.Count > 0)
            {
                value = Other.SelectedRows[0];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
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
                    CheckBoxColumn.Visible = false;
                    Other.ColumnHeadersVisible = false;
                    Other.Enabled = false;
                }
                else
                {
                    CheckBoxColumn.Visible = true;
                    Other.ColumnHeadersVisible = true;
                    Other.Enabled = true;
                }
                _NoPackages = value;
            }
        }

        private PackageListBoxColumn[] _ColumnHeaders;
        public PackageListBoxColumn[] ColumnHeaders
        {
            get
            {
                return _ColumnHeaders;
            }
            set
            {
                if (value != null)
                {
                    _ColumnHeaders = value;
                    Other.Columns.Clear();
                    Other.Columns.Add(CheckBoxColumn);
                    Other.Columns.AddRange(
                        value
                        .Select(col => col.ToDataGridViewColumn())
                        .ToArray());
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
            foreach (DataGridViewColumn col in Other.Columns)
            {
                if (col.Name.Equals(key))
                {
                    return col.Index;
                }
            }
            return -1;
        }
    }
}
