using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFP.Impact_Ultimate.Utlis.Log;

namespace XFP.Impact_Ultimate.ICoraException
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
