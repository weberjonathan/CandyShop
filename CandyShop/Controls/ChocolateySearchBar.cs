using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    public partial class ChocolateySearchBar : CommonSearchBar
    {
        public ChocolateySearchBar()
        {
            InitializeComponent();

            checkBox1.CheckedChanged += new EventHandler((sender, e) => FilterTopLevelOnlyChanged?.Invoke(this, EventArgs.Empty));
            TextSearch.TextChanged += new EventHandler((sender, e) => SearchChanged?.Invoke(this, EventArgs.Empty));
            TextSearch.KeyDown += new KeyEventHandler((sender, e) =>
            {
                if (e.KeyCode.Equals(Keys.Enter)) SearchEnterPressed?.Invoke(this, EventArgs.Empty);
            });
        }

        public override event EventHandler SearchChanged;
        public override event EventHandler SearchEnterPressed;
        public override event EventHandler FilterTopLevelOnlyChanged;

        public override string Text
        {
            get { return TextSearch.Text; }
            set { TextSearch.Text = value; }
        }

        public override bool FilterTopLevelOnly
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }
        }
    }
}
