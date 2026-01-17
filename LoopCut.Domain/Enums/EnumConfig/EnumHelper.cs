using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Enums.EnumConfig
{
    public static class EnumHelper
    {
        public static string Names<T>(this T srcValue) =>
            GetCustomName(typeof(T).GetField(srcValue?.ToString() ?? string.Empty));

        private static string GetCustomName(FieldInfo? fi)
        {
            Type type = typeof(CustomName);

            Attribute? attr = null;
            if (fi is not null)
            {
                attr = Attribute.GetCustomAttribute(fi, type);
            }

            return (attr as CustomName)?.Names ?? string.Empty;
        }

        public static string Type(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<CustomId>();
            return attribute?.Type ?? string.Empty;
        }
    }   
}
