using LoopCut.Application.DTOs.AICommand;
using LoopCut.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
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
        private readonly IMemoryCache _cache;
        private readonly string _systemPrompt;

        public GeminiService(HttpClient httpClient, IConfiguration config,
            IHandleChatService handleChatService, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _handleChatService = handleChatService;
            _cache = cache;
            _apiKey = config["Gemini:ApiKey"]!;
            _systemPrompt = LoadSystemPrompt().GetAwaiter().GetResult();
        }

        private static async Task<string> LoadSystemPrompt()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "LoopCut.Application.Prompt.prompt.txt";

            using Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Không tìm thấy tài nguyên: {resourceName}");

            using StreamReader reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        private async Task<string> CallGeminiAsync(string message)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var body = new
            {
                systemInstruction = new { parts = new[] { new { text = _systemPrompt } } },
                contents = new[] { new { role = "user", parts = new[] { new { text = message } } } }
            };

            var response = await _httpClient.PostAsync(url,
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Gemini API Error: " + responseText);

            using var doc = JsonDocument.Parse(responseText);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";
        }

        private async Task<string> ExecuteCommandAsync(AiCommand command)
        {
            return command.Action switch
            {
                "GetSubscriptionHistory" => await _handleChatService.HandleSubscriptionHistoryAsync(command),
                "GetSubscriptionStatus" => await _handleChatService.HandleSubscriptionStatusAsync(command),
                "GetMembershipCurrentUser" => await _handleChatService.HandleMembershipUserAsync(command),
                "GetMembershipInfo" => await _handleChatService.HandleMembershipInfoAsync(command),
                "GetMembershipList" => await _handleChatService.HandleMembershipListAsync(command),
                "GetPaymentHistory" => await _handleChatService.HandlePaymentHistoryAsync(command),
                _ => await _handleChatService.HandleUnknownAsync(command)
            };
        }

        public async Task<string> SendMessageAsync(string message)
        {
            //quick answers without calling AI
            var quickResponse = TryQuickResponse(message);
            if (quickResponse != null)
            {
                return quickResponse;
            }
            //check cache
            var cacheKey = $"chat_{message.ToLower().Trim()}";
            if (_cache.TryGetValue<string>(cacheKey, out var cachedResponse))
            {
                return cachedResponse;
            }
            //call Gemini API
            var reply = await CallGeminiAsync(message);
            Console.WriteLine($"Raw reply: {reply}");
            if (!reply.Trim().StartsWith("{"))
                return "Mình chỉ hỗ trợ các vấn đề về subscription/membership.";

            AiCommand? command;
            try
            {
                var cleanJson = CleanJson(reply);
                command = JsonSerializer.Deserialize<AiCommand>(cleanJson);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return "Xin lỗi, mình đang gặp lỗi kỹ thuật. Vui lòng thử lại sau.";
            }

            if (command == null)
                return "Mình chưa hiểu ý của bạn.";

            var result = await ExecuteCommandAsync(command);
            _cache.Set(cacheKey, result, TimeSpan.FromHours(1));

            return result;
        }
        private string? TryQuickResponse(string message)
        {
            var lower = message.ToLower().Trim();

            if (lower == "xin chao" || lower == "hi" || lower == "hello" || lower == "chao")
                return "Chào bạn! Mình là trợ lý ảo LoopCut, chuyên hỗ trợ về membership và subscription. Bạn cần giúp gì?";

            if (lower.Contains("cam on") || lower.Contains("thank"))
                return "Không có gì đâu! Rất vui được hỗ trợ bạn.";

            if (lower.Contains("tam biet") || lower == "bye")
                return "Tạm biệt bạn! Chúc bạn một ngày tuyệt vời!";

            return null; 
        }

        private static string CleanJson(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
                return reply;

            var text = reply.Trim();

            int start = text.IndexOf('{');
            int end = text.LastIndexOf('}');

            if (start >= 0 && end > start)
                return text.Substring(start, end - start + 1);

            return text;
        }
    }
}