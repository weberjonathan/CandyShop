using CandyShop.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.Devices.SmartCards;

namespace CandyShop.Controls
{
    public partial class Banner : UserControl
    {
        public event EventHandler Closing;

        public override string Text
        {
            get
            {
                return LblAdmin.Text;
            }
            set
            {
                LblAdmin.Text = value;
            }
        }

        public Banner()
        {
            InitializeComponent();
            ColoredContainer.BackColor = SystemColors.Info;
            BtnHideWarning.Click += (sender, e) => Closing?.Invoke(sender, e);
        }
    }
}
