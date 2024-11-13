using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    public partial class SearchBar : UserControl
    {
        public SearchBar()
        {
            InitializeComponent();

            checkBox1.CheckedChanged += new EventHandler((sender, e) => CheckedChanged?.Invoke(this, EventArgs.Empty));
            TextSearch.TextChanged += new EventHandler((sender, e) => SearchChanged?.Invoke(this, EventArgs.Empty));
            TextSearch.KeyDown += new KeyEventHandler((sender, e) =>
            {
                if (e.KeyCode.Equals(Keys.Enter)) SearchEnterPressed?.Invoke(this, EventArgs.Empty);
            });
        }

        public event EventHandler SearchChanged;
        public event EventHandler SearchEnterPressed;
        public event EventHandler CheckedChanged;

        public override string Text
        {
            get { return TextSearch.Text; }
            set { TextSearch.Text = value; }
        }

        public bool Checked
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }
        }

        public string CheckboxText
        {
            get { return checkBox1.Text; }
            set
            {
                checkBox1.Text = value;
                SplitContainer.SplitterDistance = SplitContainer.Width - 15 - SplitContainer.SplitterWidth - checkBox1.Width;
            }
        }

        public bool ShowCheckBox
        {
            get { return checkBox1.Visible; }
            set { checkBox1.Visible = value; }
        }
    }
}
