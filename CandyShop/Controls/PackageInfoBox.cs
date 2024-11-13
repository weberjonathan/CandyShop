using System.Windows.Forms;

namespace CandyShop.Controls
{
    public class PackageInfoBox : SpinnerBaseControl<TextBox>
    {
        public PackageInfoBox()
        {
            Other = new TextBox()
            {
                Name = "Textbox",
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both
            };
        }

        public override string Text
        {
            get
            {
                return Other.Text;
            }
            set
            {
                Other.Text = value;
            }
        }
    }
}
