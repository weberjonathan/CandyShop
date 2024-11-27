using System.Linq;
using System.Security.Principal;

namespace CandyShop
{
    internal class Util
    {
        public static bool IsAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool HasDots(string value, int n)
        {
            return value.Where(c => c.Equals('.')).Count() == n;
        }

        public static bool IsNumeric(string value, char allowed = '.')
        {
            return value.Where(c => !c.Equals(allowed) || !char.IsNumber(c)).Any();
        }
    }
}
