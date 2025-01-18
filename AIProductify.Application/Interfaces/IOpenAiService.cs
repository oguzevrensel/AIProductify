
namespace AIProductify.Application.Interfaces
{
    public interface IOpenAiService
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task<string> TranslateTextAsync(string text);

    }
}
