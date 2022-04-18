using CandyShop.Properties;
using System;
using System.Windows.Forms;

namespace CandyShop.View
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
            this.Text = String.Format(LocaleEN.TEXT_LICENSE_TITLE, Application.ProductName);
            RtfLicense.SelectedRtf = Resources.AllLicenses;
            RtfLicense.DeselectAll();
        }
    }
}
