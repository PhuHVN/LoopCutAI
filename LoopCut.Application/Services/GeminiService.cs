using LoopCut.Application.DTOs.AICommand;
using LoopCut.Application.DTOs.ChatDTO;
using LoopCut.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IHandleChatService _handleChatService;
        private readonly string _systemPrompt;

        public GeminiService(HttpClient httpClient, IConfiguration config, IHandleChatService handleChatService)
        {
            _httpClient = httpClient;
            _handleChatService = handleChatService;
            _apiKey = config["Gemini:ApiKey"]!;
            _systemPrompt = LoadSystemPrompt();
        }
        private static string LoadSystemPrompt()
        {
            var assembly = typeof(GeminiService).Assembly;
            var resourceName = "C:\\FPTU\\SP26\\EXE202\\LoopCutAI\\LoopCut.Application\\Prompt\\prompt.txt";

            using var stream = new FileStream(resourceName, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private async Task<string> CallGeminiAsync(string message)
        {
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
            var body =
                new
                {
                    systemInstruction = new
                    {
                        parts = new[] { new { text = _systemPrompt } }
                    },
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[] { new { text = message } }
                        }
                    }
                };

            var jsonBody = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(jsonBody, Encoding.UTF8, "application/json")
            );

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Gemini API Error: " + responseText);

            using var doc = JsonDocument.Parse(responseText);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()
                ?? "";
        }
        private async Task<string> ExecuteCommandAsync(AiCommand command)
        {
            switch (command.Action)
            {
                case "GetSubscriptionInfo":
                    return await _handleChatService.HandleSubscriptionAsync(command);

                case "GetMembershipCurrentUser":
                    return await _handleChatService.HandleMembershipUserAsync(command);

                case "GetMembershipInfo":
                    return await _handleChatService.HandleMembershipInfoAsync(command);

                default:
                    return await _handleChatService.HandleUnknownAsync(command);
            }
        }
        public async Task<string> SendMessageAsync(string message)
        {
            var reply = await CallGeminiAsync(message);
            if (IsPromptLeak(reply))
                return "Mình chỉ hỗ trợ các vấn đề về subscription/membership.";

            AiCommand? command;
            try
            {
                var cleanJson = reply.Trim();
                if (cleanJson.StartsWith("```json"))
                    cleanJson = cleanJson.Substring(7);
                if (cleanJson.StartsWith("```"))
                    cleanJson = cleanJson.Substring(3);
                if (cleanJson.EndsWith("```"))
                    cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
                command = JsonSerializer.Deserialize<AiCommand>(reply);

            }
            catch
            {
                return "AI trả sai định dạng JSON, bạn thử lại giúp mình.";
            }
            if (command == null)
                return "Mình chưa hiểu ý của bạn.";

            return await ExecuteCommandAsync(command);
        }
        private static bool IsPromptLeak(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
                return false;

            var lower = reply.ToLower();

            return lower.Contains("chính sách nội bộ")
                || lower.Contains("system prompt")
                || lower.Contains("systeminstruction")
                || lower.Contains("instruction")
                || lower.Contains("quy tắc")
                || lower.Contains("nhiệm vụ")
                || lower.Contains("prompt hệ thống")
                || lower.Contains("ai router")
                || !lower.Trim().StartsWith("{");
        }
    }
}

