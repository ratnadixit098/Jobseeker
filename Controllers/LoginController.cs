using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Jobseeker.Models;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;

    public LoginController(IConfiguration config)
    {
        _config = config;
    }

    // ✅ STEP 1: Send OTP
    [HttpPost("send-otp")]
    public IActionResult SendOtp([FromBody] SendOtpModel model)
    {
        if (string.IsNullOrEmpty(model.Mobile))
            return BadRequest("Mobile required");

        // ⚠ Demo purpose only
        string otp = "123456";

        // Normally: DB me save karo + expiry lagao

        return Ok(new { message = "OTP Sent Successfully" });
    }

    // ✅ STEP 2: Verify OTP + Generate JWT
    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpModel model)
    {
        if (model.Otp != "123456")
            return Unauthorized("Invalid OTP");

        var token = GenerateToken(model.Mobile);

        return Ok(new { token });
    }

    // ✅ JWT Generator
    private string GenerateToken(string mobile)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, mobile),   // 👈 Mobile store hoga token me
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ✅ Protected API Example
    [Authorize]
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var mobile = User.Identity.Name;
        return Ok($"Welcome {mobile}, this is protected data.");
    }
}
