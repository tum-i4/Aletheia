using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Aletheia.CommandLine
{
    public static class IConverter
    {
        public static T changeType<T>(object value)
        {
            return (T)changeType(typeof(T), value);
        }

        public static object changeType(Type t, object value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }
    }
}
