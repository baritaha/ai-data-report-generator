namespace APIApp.Models
{
    public class AiRequest
    {
        public string? model { get; set; } = "llama2";
        public string? prompt { get; set; }
        public bool stream { get; set; } = false;
    }

    public class AiResponse
    {
        public string? model { get; set; }
        public string? response { get; set; }
        public bool done { get; set; }
    }
}