using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    public class CommonSearchBar : UserControl
    {
        public virtual event EventHandler SearchChanged;
        public virtual event EventHandler FilterTopLevelOnlyChanged;
        public virtual event EventHandler SearchEnterPressed;

        public override string Text { get; set; }
        public virtual bool FilterTopLevelOnly { get; set; }
    }
}
