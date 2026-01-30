using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.ChatDTO
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
    public class GeminiResponse
    {
        public List<Candidate>? Candidates { get; set; }
    }

    public class Candidate
    {
        public GeminiContent? Content { get; set; }
    }

    public class GeminiContent
    {
        public List<GeminiPart>? Parts { get; set; }
    }

    public class GeminiPart
    {
        public string? Text { get; set; }
    }
}
