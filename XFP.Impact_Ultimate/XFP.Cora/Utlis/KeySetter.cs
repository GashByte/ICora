//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.ICora.Utils
{
    public class KeySetter
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("Impact_Ultimate");

        public void sk(string name, string value)
        {
            key.SetValue(name, value);
        }

        public string gk(string name)
        {
            return (string)key.GetValue(name);
        }
    }
}
