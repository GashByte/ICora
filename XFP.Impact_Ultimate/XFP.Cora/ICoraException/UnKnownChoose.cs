//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.ICora.ICoraException
{
    public class UnKnownChoose
    {
        LogWriter log = new();

        /// <summary>
        /// Unknown user selection
        /// </summary>
        /// <param name="UserChoose"></param>
        public UnKnownChoose(string UserChoose)
        {
            Growl.Error("Throw Exception Un Known Choose : " + UserChoose);
            log.ErrorLog("Throw Exception Un Known Choose : " + UserChoose, -10);
        }
    }
}
