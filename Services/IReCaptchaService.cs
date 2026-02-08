using System.Text.Json;
using System.Text.Json.Serialization;

public interface IReCaptchaService
{
    Task<bool> IsCaptchaValid(string token);

}
