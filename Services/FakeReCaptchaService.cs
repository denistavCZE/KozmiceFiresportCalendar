using System.Text.Json;
using System.Text.Json.Serialization;

public class FakeReCaptchaService : IReCaptchaService
{
    public FakeReCaptchaService()
    {

    }

    public Task<bool> IsCaptchaValid(string token)
    {
        return Task.FromResult(true);
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
