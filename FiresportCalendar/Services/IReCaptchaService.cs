using System.Text.Json;
using System.Text.Json.Serialization;

public interface IReCaptchaService
{
    bool IsEnabled { get; }
    Task<bool> IsCaptchaValid(string token);

}
