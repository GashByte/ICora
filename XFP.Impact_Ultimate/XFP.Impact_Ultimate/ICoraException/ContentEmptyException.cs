//Copyright(C) XFP Group and Contributors. All rights resvered.
//Licensed under the MIT Licensed.

using HandyControl.Controls;

namespace XFP.Impact_Ultimate.ICoraException
{
    public class ContentEmptyException
    {
        private int ReturnCode { get; set; }

        public void ContentIsEmpty(string content, int returnCode)
        {
            returnCode = ReturnCode;
            Growl.Clear();
            Growl.Error($"Throw Exception : The content is empty || {content}\n Return Code{returnCode}");
        }

        public void UserInfoIsEmpty(string content, int returnCode)
        {
            returnCode = ReturnCode;
            Growl.Clear();
            Growl.Error($"Throw Exception : The User info is empty || {content}\n Return Code{returnCode}");
        }
    }
}
