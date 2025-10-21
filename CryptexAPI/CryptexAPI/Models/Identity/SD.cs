namespace CryptexAPI.Models.Identity;

public static class SD
{
    public static string GoogleAPIEndpointURL = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=";
    public static string Audience { get; set; }
    public static string JWTKey { get; set; }
    public static string Issuer { get; set; }
}
