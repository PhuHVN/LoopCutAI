using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.AICommand
{
    public class AiCommand
    {
        [JsonPropertyName("action")] 
        public string Action { get; set; } = string.Empty;

        [JsonPropertyName("data")]   
        public JsonElement Data { get; set; }
    }
}
