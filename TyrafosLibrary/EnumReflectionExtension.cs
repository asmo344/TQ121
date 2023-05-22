using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tyrafos
{
    public static class EnumReflectionExtension
    {
        public static Attribute GetAttribute(this Enum source, Type attributeType)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            if (!attributeType.IsNull())
            {
                return fi.GetCustomAttribute(attributeType, false);
            }
            else
                throw new ArgumentException("Unknow Attribute Type");
        }
    }
}