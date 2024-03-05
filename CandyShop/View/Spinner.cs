using System.Windows.Forms;

namespace CandyShop.View
{
    public partial class Spinner : UserControl
    {
        public override string Text
        {
            get
            {
                return LabelDescription.Text;
            }
            set
            {
                LabelDescription.Text = value;
                UpdateRows();
            }
        }

        public Spinner()
        {
            InitializeComponent();
            TablePanel.RowStyles[0].SizeType = SizeType.Percent;
            TablePanel.RowStyles[1].SizeType = SizeType.Percent;
            TablePanel.RowStyles[2].SizeType = SizeType.Percent;
            UpdateRows();
        }

        private void UpdateRows()
        {
            if (string.IsNullOrEmpty(LabelDescription.Text))
            {
                TablePanel.RowStyles[0].Height = 100;
                TablePanel.RowStyles[1].Height = 0;
                TablePanel.RowStyles[2].Height = 0;
                SpinnerImage.Dock = DockStyle.Fill;
            }
            else
            {
                TablePanel.RowStyles[0].Height = 40;
                TablePanel.RowStyles[1].Height = 10;
                TablePanel.RowStyles[2].Height = 50;
                SpinnerImage.Dock = DockStyle.Bottom;
            }
        }
    }
}
