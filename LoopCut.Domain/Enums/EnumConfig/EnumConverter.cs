using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopCut.Domain.Enums.EnumConfig
{
    public class ExclusiveEnumConverterFactory : JsonConverterFactory
    {
        private readonly HashSet<Type> _excludeFromString;

        public ExclusiveEnumConverterFactory(Type[] excludeFromString)
        {
            _excludeFromString = new HashSet<Type>(excludeFromString);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (_excludeFromString.Contains(typeToConvert))
            {
                var intConverterType = typeof(GenericEnumIntConverter<>).MakeGenericType(typeToConvert);
                return (JsonConverter)Activator.CreateInstance(intConverterType);
            }

            var stringConverterType = typeof(GenericEnumStringConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(stringConverterType);
        }
    }
}
