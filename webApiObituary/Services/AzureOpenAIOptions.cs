using System;

namespace Assignment1.Services
{
    public class AzureOpenAIOptions
    {
        public string Endpoint { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public string ApiVersion { get; set; } = "";
        public string Model { get; set; } = "";
        public int MaxTokens { get; set; }
    }
}

