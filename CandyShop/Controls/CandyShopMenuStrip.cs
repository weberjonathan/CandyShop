using System;
using System.Windows.Forms;

namespace CandyShop.Controls
{
    internal class CandyShopMenuStrip : MenuStrip
    {
        public ToolStripMenuItem ItemAt(params string[] itemNames)
        {
            ArgumentNullException.ThrowIfNull(itemNames);

            if (itemNames.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(itemNames));

            ToolStripMenuItem item = (ToolStripMenuItem)Items[itemNames[0]];
            for (int i = 1; i < itemNames.Length; i++)
            {
                item = (ToolStripMenuItem)item.DropDownItems[itemNames[i]];
            }
            return item;
        }
    }
}
