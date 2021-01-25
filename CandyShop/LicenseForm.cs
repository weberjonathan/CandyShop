using System.Windows.Forms;

namespace CandyShop
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
            this.Text = $"{Application.ProductName} License";
            RtfLicense.SelectedRtf = Properties.Resources.AllLicenses;
            RtfLicense.DeselectAll();
        }
    }
}
