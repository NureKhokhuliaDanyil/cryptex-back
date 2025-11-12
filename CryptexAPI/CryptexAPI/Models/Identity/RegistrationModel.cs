namespace CryptexAPI.Models.Identity;

public class RegistrationModel
{
    public string? GoogleID { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public int Age { get; set; }
    public string Country { get; set; }
    public string Adress { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
