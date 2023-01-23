using System;

namespace XFP.Impact_Ultimate.ICoraException
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
