using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    public static class PackageDecoder
    {
        public static string GetValue(string src,string name)
        {
            if (!src.Contains(name))
                return "Error";
            string substr = src.Substring(src.IndexOf(name) + name.Length + 1);
            string result = substr.Substring(0, substr.IndexOf(" "));
            return result;
        }
    }

    public static class PackageEncoder
    {
        public static string SetValue(string src, string name, string val)
        {
            if (src.Contains(name))
                return "Error";
            string substr = src.Substring(src.IndexOf(name) + name.Length + 1);
            string result = substr.Substring(0, substr.IndexOf(" "));
            return result;
        }
    }
}
