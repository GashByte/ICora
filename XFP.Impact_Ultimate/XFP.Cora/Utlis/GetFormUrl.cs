//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.ICora.Utils
{
    public class GetFormUrl
    {
        public string Get(string url)
        {
            try
            {
                string strBuff = "";
                int byteRead = 0;
                char[] cbuffer = new char[256];
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
                Stream respStream = httpResp.GetResponseStream();
                StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.UTF8);
                byteRead = respStreamReader.Read(cbuffer, 0, 256);
                while (byteRead != 0)
                {
                    string strResp = new string(cbuffer, 0, byteRead);
                    strBuff = strBuff + strResp;
                    byteRead = respStreamReader.Read(cbuffer, 0, 256);
                }
                respStream.Close();
                return strBuff;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
