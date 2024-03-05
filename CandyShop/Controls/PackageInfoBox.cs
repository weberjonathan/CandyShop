using CandyShop.View;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    public partial class PackageInfoBox : UserControl
    {
        private readonly Spinner SpinnerCtl = new Spinner
        {
            Name = "Spinner",
            Dock = DockStyle.Fill
        };

        private readonly TextBox TextBoxCtl = new TextBox()
        {
            Name = "Textbox",
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Both
        };

        public PackageInfoBox()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return TextBoxCtl.Text;
            }
            set
            {
                TextBoxCtl.Text = value;
            }
        }

        public bool Loading
        {
            get
            {
                return Controls.ContainsKey(SpinnerCtl.Name);
            }
            set
            {
                if (value)
                {
                    Controls.Add(SpinnerCtl);
                    Controls.Remove(TextBoxCtl);
                    TextBoxCtl.Text = "";
                }
                else
                {
                    Controls.Add(TextBoxCtl);
                    Controls.Remove(SpinnerCtl);
                }
            }
        }
    }
}
