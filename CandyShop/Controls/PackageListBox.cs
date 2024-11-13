using CandyShop.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class PinnedChangedArgs : EventArgs
    {
        public string Name;
    }

    internal enum ColumnType { Text, Checked, Pinned }

    internal class PackageListBoxColumn
    {
        public PackageListBoxColumn(ColumnType type)
        {
            if (type.Equals(ColumnType.Pinned))
            {
                ColumnType = ColumnType.Pinned;
                Key = "Pinned";
                Name = "";
                Value = 30;
                Unit = PackageListBoxSize.Fixed;
            }
        }

        public PackageListBoxColumn(string key, string name, float value, PackageListBoxSize unit)
        {
            ColumnType = ColumnType.Text;
            Key = key;
            Name = name;
            Value = value;
            Unit = unit;
        }

        public ColumnType ColumnType { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public PackageListBoxSize Unit { get; set; }

        public DataGridViewColumn ToDataGridViewColumn()
        {
            switch (ColumnType)
            {
                case ColumnType.Text:
                    return ToDataGridViewTextColumn();
                case ColumnType.Checked:
                    // TODO this is currently unused; check box column is added via PackageListBox.Checkbox
                    throw new NotImplementedException();
                case ColumnType.Pinned:
                    return ToDataGridViewImageColumn();
                default:
                    throw new NotImplementedException();
            }
        }

        private DataGridViewTextBoxColumn ToDataGridViewTextColumn()
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

        private DataGridViewImageColumn ToDataGridViewImageColumn()
        {
            DataGridViewImageColumn rtn = new()
            {
                Name = Key,
                HeaderText = Name,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable, // TODO allow sorting
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = (int)Value,
            };

            rtn.DefaultCellStyle.NullValue = null;
            return rtn;
        }
    }

    public enum PackageListBoxSize { Fixed, Percent }

    internal class PackageListBox : SpinnerBaseControl<DataGridView>
    {
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
                if (CheckBoxes && e.RowIndex >= 0)
                {
                    bool previous = (bool)Other.Rows[e.RowIndex].Cells[CheckBoxColumn.Index].Value;
                    Other.Rows[e.RowIndex].Cells[CheckBoxColumn.Index].Value = !previous;
                }

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

            // define default cell styles
            DefaultStyle = Other.DefaultCellStyle;
            DataGridViewCellStyle pinnedStyle = new(Other.DefaultCellStyle)
            {
                SelectionBackColor = Color.LightGray,
                SelectionForeColor = Color.Black,
            };
            PinnedStyle = pinnedStyle;

            // overwrite checkbox alignment
            Other.RowDefaultCellStyleChanged += new DataGridViewRowEventHandler((sender, e) =>
            {
                if (CheckedCol != null && CheckedCol.Index >= 0)
                {
                    e.Row.Cells[CheckedCol.Index].Style = new DataGridViewCellStyle(e.Row.DefaultCellStyle)
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    };
                }

                if (PinnedCol != null && PinnedCol.Index >= 0)
                {
                    e.Row.Cells[PinnedCol.Index].Style = new DataGridViewCellStyle(e.Row.DefaultCellStyle)
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    };

                }
            });

            ContextMenuStrip = DefineContextMenu();
        }

        public event EventHandler<PinnedChangedArgs> PinChangeRequest;
        public event DataGridViewCellEventHandler ItemChecked;

        public bool CheckBoxes
        {
            get { return CheckBoxColumn.Visible; }
            set { CheckBoxColumn.Visible = value; }
        }

        public DataGridViewColumn PinnedCol { get; private set; }
        public DataGridViewColumn NameCol { get; private set; }
        public DataGridViewColumn CheckedCol => CheckBoxColumn;
        public DataGridViewRow SelectedItem => Other.SelectedRows.Count > 0 ? Other.SelectedRows[0] : null;
        public string SelectedName => SelectedItem?.Cells[NameCol.Index].Value.ToString();
        public DataGridViewCellStyle DefaultStyle { get; set; }
        public DataGridViewCellStyle PinnedStyle { get; set; }

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
                    Other.SelectionChanged += Other_SelectionChanged;
                }
                else
                {
                    CheckBoxColumn.Visible = true;
                    Other.ColumnHeadersVisible = true;
                    Other.Enabled = true;
                    Other.SelectionChanged -= Other_SelectionChanged;
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
                    foreach (var col in value)
                    {
                        var dgvCol = col.ToDataGridViewColumn();
                        Other.Columns.Add(dgvCol);
                        if (col.ColumnType.Equals(ColumnType.Pinned))
                            PinnedCol = dgvCol;

                        if (col.Name.Equals("Name")) // TODO should be ColumnType.Key
                            NameCol = dgvCol;
                    }
                }
            }
        }

        public string Hint
        {
            get { return SpinnerCtl.Text; }
            set { SpinnerCtl.Text = value; }
        }

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

        public void AddItem(object[] data)
        {
            var index = Other.Rows.Add(data);
            if (NoPackages) NoPackages = false;
            if (Loading) Loading = false;
            UpdateRowStyle(Other.Rows[index]);
        }

        public void UpdateRowStyle(DataGridViewRow row)
        {
            if (PinnedCol != null)
            {
                if (row.Cells[PinnedCol.Index].Value == null)
                {
                    row.DefaultCellStyle = DefaultStyle;
                    row.Cells[CheckedCol.Index].ReadOnly = false;
                }
                else
                {
                    row.DefaultCellStyle = PinnedStyle;
                    row.Cells[CheckedCol.Index].ReadOnly = true;
                }
            }
        }

        public void SetPinned(string name, bool pinned)
        {
            // updates icon and checked state according to param pinned
            var rows = Other.Rows;
            foreach (DataGridViewRow row in rows)
            {
                var cellName = (string)row.Cells[NameCol.Index].Value;
                if (name.Equals(cellName))
                {
                    if (pinned)
                    {
                        row.Cells[PinnedCol.Index].Value = Resources.ic_pin;
                        row.Cells[CheckedCol.Index].Value = false;
                    }
                    else
                    {
                        row.Cells[PinnedCol.Index].Value = null;
                        row.Cells[CheckedCol.Index].Value = true;
                    }
                    UpdateRowStyle(row);
                    return;
                }
            }
        }

        private void Other_SelectionChanged(object sender, EventArgs e)
        {
            Other.ClearSelection();
        }

        private ContextMenuStrip DefineContextMenu()
        {
            var itemPin = new ToolStripMenuItem("&Toggle pin")
            {
                Name = "Pin",
            };
            itemPin.Click += new EventHandler((sender, e) =>
            {
                if (TryGetSelectedItem(out var row))
                {
                    var name = (string)row.Cells[NameCol.Index].Value;
                    PinChangeRequest?.Invoke(this, new PinnedChangedArgs() { Name = name });
                }
            });

            var contextMenu = new ContextMenuStrip();
            contextMenu.Opening += new System.ComponentModel.CancelEventHandler((sender, e) =>
            {
                if (PinnedCol != null && TryGetSelectedItem(out var row))
                {
                    bool pinned = row.Cells[PinnedCol.Index].Value != null;
                    itemPin.Checked = pinned;
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            });

            contextMenu.Items.Add(itemPin);
            return contextMenu;
        }
    }
}
