namespace TasteOfHome.Models
{
    public class OpenAiOptions
    {
        public string ApiKey { get; set; } = "";
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
        public string Model { get; set; } = "gpt-5.4-mini";
    }
}