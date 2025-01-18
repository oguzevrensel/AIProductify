using AIProductify.Application.Helper;
using AIProductify.Application.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI.Chat;


namespace AIProductify.Application.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly ChatClient _chatClient;

        public OpenAiService(IOptions<GetSettingsConfig> options)
        {
            var settings = options.Value;

            if (string.IsNullOrEmpty(settings.ApiKey))
                throw new ArgumentNullException("API key is not provided in the configuration.");

            if (string.IsNullOrEmpty(settings.Model))
                throw new ArgumentNullException("Model is not provided in the configuration.");

            // Initialize ChatClient
            _chatClient = new ChatClient(model: settings.Model, apiKey: settings.ApiKey);
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are an expert assistant specializing in product content generation, translation, and scoring based on provided parameters."),
                new UserChatMessage($"Generate a response based on the prompt:\n\n{prompt}")
            };

            var completionResult = await _chatClient.CompleteChatAsync(messages);

            var responseMessage = completionResult.Value.Content[0].Text;
            return responseMessage ?? "No response was generated.";
        }


        public async Task<string> TranslateTextAsync(string text)
        {
            var prompt = $"Translate the following text into English: Just give me the English translation." +
                $" Only write to me english version. Don't give me any other output other than the English translation. \n\n{text}";
            return await GenerateResponseAsync(prompt);
        }
    }
}
