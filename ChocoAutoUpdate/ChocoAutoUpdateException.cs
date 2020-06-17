namespace ChocoAutoUpdate
{
    [System.Serializable]
    public class ChocoAutoUpdateException : System.Exception
    {
        public ChocoAutoUpdateException() { }
        public ChocoAutoUpdateException(string message) : base(message) { }
        public ChocoAutoUpdateException(string message, System.Exception inner) : base(message, inner) { }
        protected ChocoAutoUpdateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
