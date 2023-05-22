using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class MyFileParser
    {
        public static string RemoveComment(string content)
        {
            if (content.Contains("//"))
            {
                int index = content.IndexOf("//");
                content = content.Remove(index);
            }
            return content;
        }
    }
}
