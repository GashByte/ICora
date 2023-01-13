using Microsoft.Win32;

namespace XFP.Impact_Ultimate.Utlis
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
