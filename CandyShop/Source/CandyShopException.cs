namespace CandyShop
{
    [System.Serializable]
    public class CandyShopException : System.Exception
    {
        public CandyShopException() { }
        public CandyShopException(string message) : base(message) { }
        public CandyShopException(string message, System.Exception inner) : base(message, inner) { }
        protected CandyShopException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
