using System;
using System.Windows.Forms;
using CandyShop.Controls.PackageManager;

namespace CandyShop.Controls
{
    public partial class WingetSearchBar : CommonSearchBar
    {
        public WingetSearchBar()
        {
            InitializeComponent();

            checkBox1.CheckedChanged += new EventHandler((sender, e) => FilterRequireSourceChanged?.Invoke(this, EventArgs.Empty));
            textBox1.TextChanged += new EventHandler((sender, e) => SearchChanged?.Invoke(this, EventArgs.Empty));
            textBox1.KeyDown += new KeyEventHandler((sender, e) =>
            {
                if (e.KeyCode.Equals(Keys.Enter)) SearchEnterPressed?.Invoke(this, EventArgs.Empty);
            });
        }

        public override event EventHandler SearchChanged;
        public override event EventHandler SearchEnterPressed;
        public override event EventHandler FilterRequireSourceChanged;

        public override string Text
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public override bool FilterRequireSource
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }
        }
    }
}
