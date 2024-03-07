namespace CandyShop.PackageCore
{
    /// <summary>
    /// The exception that is thrown when Chocolatey returned an error or the communication with Chocolatey failed.
    /// </summary>
    [System.Serializable]
    public class PackageManagerException : System.Exception
    {
        public PackageManagerException() { }
        public PackageManagerException(string message) : base(message) { }
        public PackageManagerException(string message, System.Exception inner) : base(message, inner) { }
    }
}
