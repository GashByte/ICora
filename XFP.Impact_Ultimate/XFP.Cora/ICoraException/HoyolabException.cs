namespace XFP.ICora.ICoraException
{
    public class HoyolabException : Exception
    {
        public int ReturnCode { get; init; }

        public HoyolabException(int returnCode, string message) : base($"{message}({returnCode})")
        {
            ReturnCode = returnCode;
        }
    }
}
