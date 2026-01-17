using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Enums.EnumConfig
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomId : Attribute
    {
        public string Type { get; set; }
        public CustomId(string type)
        {
            Type = type;
        }
    }
}
