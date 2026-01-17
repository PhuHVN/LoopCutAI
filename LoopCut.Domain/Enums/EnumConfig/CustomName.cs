using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Enums.EnumConfig
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomName : Attribute
    {
        public string Names { get; set; }

        public CustomName(string name)
        {
            Names = name;
        }
    }
}
