using System;
using System.Windows.Forms;

namespace CandyShop.Controls.PackageManager
{
    public class CommonSearchBar : UserControl
    {
        public virtual event EventHandler SearchChanged;
        public virtual event EventHandler SearchEnterPressed;
        public virtual event EventHandler FilterTopLevelOnlyChanged;
        public virtual event EventHandler FilterRequireSourceChanged;

        public override string Text { get; set; }
        public virtual bool FilterTopLevelOnly { get; set; } = false;
        public virtual bool FilterRequireSource { get; set; } = false;
    }
}
