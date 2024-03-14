using System.Windows.Forms;

namespace CandyShop.Util
{
    /// <summary>
    /// Renderer for toolstrips that behaves like the system renderer,
    /// but does not draw any borders. Use in conjunction with
    /// <c>ToolStripRenderMode.ManagerRenderMode</c>.
    /// </summary>
    internal class CandyShopTsRenderer : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // noop
        }
    }
}
