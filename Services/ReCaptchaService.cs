using System.Text.Json;
using System.Text.Json.Serialization;

public class ReCaptchaService : IReCaptchaService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public ReCaptchaService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<bool> IsCaptchaValid(string token)
    {
        var secretKey = _configuration["ReCaptcha:SecretKey"];
        var response = await _httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ReCaptchaResponse>(json);

        return result?.Success ?? false;
    }

    private class ReCaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("score")]
        public float Score { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("challenge_ts")]
        public string ChallengeTs { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
