using CryptexAPI.Models.Identity;
using CryptexAPI.Models.Persons;
using CryptexAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptexAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenGeneratingService _tokenGeneratingService;
    private readonly IUserService _userService;
    private readonly IWalletService _walletService;
    public AuthController(ITokenGeneratingService tokenGeneratingService, IUserService userService, IWalletService walletService)
    {
        _tokenGeneratingService = tokenGeneratingService;
        _userService = userService;
        _walletService = walletService;
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", new { returnUrl }) };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return BadRequest();
        }

        var claims = authenticateResult.Principal.Claims.ToList();

        var googleId = claims.Where(x => x.Type.ToString() == ClaimTypes.NameIdentifier).FirstOrDefault().Value;

        var user = await _userService.GetByGoogleId(googleId);
        string? jwtToken;

        if (user == null)
        {
            var registrationModel = new RegistrationModel
            {
                GoogleID = googleId,
                Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                Name = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value ?? "",
                Surname = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value ?? "",
                PhoneNumber = claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone)?.Value ?? "",
                Country = claims.FirstOrDefault(x => x.Type == ClaimTypes.Country)?.Value ?? "",
                Adress = claims.FirstOrDefault(x => x.Type == ClaimTypes.StreetAddress)?.Value ?? "",
                Role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? ""
            };
            var wallet = _walletService.CreateWallet();
            user = await _userService.Insert(registrationModel, wallet, isGoogleRegistration: true);
        }

        jwtToken = _tokenGeneratingService.GenerateToken(user);

        return Ok(jwtToken);
    }

    [HttpPatch("login")]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        var user = await _userService.Login(loginModel);
        if (user == null)
        {
            return NotFound();
        }

        var jwtToken = _tokenGeneratingService.GenerateToken(user);
        return Ok(jwtToken);
    }

    [HttpPost("registration")]
    public async Task<ActionResult> Registration(RegistrationModel registrationModel)
    {
        var wallet = _walletService.CreateWallet();
        var result = await _userService.Insert(registrationModel, wallet);
        if (result != null)
        {
            var jwtToken = _tokenGeneratingService.GenerateToken(result);
            return Ok(jwtToken);
        }

        return BadRequest();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(User user)
    {
        var result = await _userService.Update(user);

        return result ? Ok() : BadRequest();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
