namespace CandyShop.Chocolatey
{
    /// <summary>
    /// The exception that is thrown when Chocolatey returned an error or the communication with Chocolatey failed.
    /// </summary>
    [System.Serializable]
    public class ChocolateyException : System.Exception
    {
        public ChocolateyException() { }
        public ChocolateyException(string message) : base(message) { }
        public ChocolateyException(string message, System.Exception inner) : base(message, inner) { }
    }
}
