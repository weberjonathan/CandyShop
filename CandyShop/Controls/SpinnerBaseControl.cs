using System.Windows.Forms;

namespace CandyShop.Controls
{
    /// <summary>
    /// This control can be used to inherit from to easily switch between the other control and a spinner in the same space by toggling the Loading property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class SpinnerBaseControl<T> : Control where T: Control
    {
        internal readonly Spinner SpinnerCtl = new()
        {
            Name = "Spinner",
            Dock = DockStyle.Fill
        };

        internal T Other { get; set; }

        public override ContextMenuStrip ContextMenuStrip { get => Other.ContextMenuStrip; set => Other.ContextMenuStrip = value; }

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
                    Controls.Remove(Other);
                }
                else
                {
                    Controls.Add(Other);
                    Controls.Remove(SpinnerCtl);
                }
            }
        }
    }
}
