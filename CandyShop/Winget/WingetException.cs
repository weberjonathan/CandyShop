namespace CandyShop.Winget
{
    /// <summary>
    /// The exception that is thrown when winget returned an error or the communication with winget failed.
    /// </summary>
    [System.Serializable]
    public class WingetException : System.Exception
    {
        public WingetException() { }
        public WingetException(string message) : base(message) { }
        public WingetException(string message, System.Exception inner) : base(message, inner) { }
    }
}
