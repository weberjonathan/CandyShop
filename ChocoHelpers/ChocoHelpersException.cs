[System.Serializable]
public class ChocoHelpersException : System.Exception
{
    public ChocoHelpersException() { }
    public ChocoHelpersException(string message) : base(message) { }
    public ChocoHelpersException(string message, System.Exception inner) : base(message, inner) { }
    protected ChocoHelpersException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}