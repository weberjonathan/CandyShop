using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
            this.Text = String.Format(Properties.Strings.License_Title, Application.ProductName);
            RtfLicense.SelectedRtf = Properties.Resources.AllLicenses;
            RtfLicense.DeselectAll();
        }
    }
}
