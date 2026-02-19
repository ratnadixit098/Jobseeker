using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Jobseeker.Models;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly string _conStr;

    public LoginController(IConfiguration config)
    {
        _config = config;
        _conStr = _config.GetConnectionString("DefaultConnection");
    }

    // ✅ STEP 1: Send OTP (ONLY REGISTERED USER)
    [HttpPost("send-otp")]
    public IActionResult SendOtp([FromBody] SendOtpModel model)
    {
        if (string.IsNullOrEmpty(model.Mobile))
            return BadRequest("Mobile required");

        using (SqlConnection con = new SqlConnection(_conStr))
        {
            string query = "SELECT COUNT(*) FROM Users_Registration WHERE Mobile=@Mobile";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Mobile", model.Mobile);

            con.Open();
            int count = (int)cmd.ExecuteScalar();
            con.Close();

            // ❌ Not registered
            if (count == 0)
            {
                return BadRequest(new
                {
                    message = "User not registered. Please register first."
                });
            }
        }

        // ✅ Demo OTP
        string otp = "123456";

        return Ok(new { message = "OTP Sent Successfully" });
    }

    // ✅ STEP 2: Verify OTP + Generate JWT + ROLE
    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpModel model)
    {
        // ✅ Default OTP
        if (model.Otp != "123456")
            return Unauthorized("Invalid OTP");

        string role = "User"; // default

        using (SqlConnection con = new SqlConnection(_conStr))
        {
            string query = "SELECT Role=case when Mobile='8948160370' then 'Admin' else 'User' end  FROM Users_Registration WHERE Mobile=@Mobile";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Mobile", model.Mobile);

            con.Open();
            var result = cmd.ExecuteScalar();
            con.Close();

            if (result != null)
                role = result.ToString(); // Admin ya User
        }

        var token = GenerateToken(model.Mobile, role);

        return Ok(new
        {
            token,
            role
        });
    }

    // ✅ JWT Generator
    private string GenerateToken(string mobile, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, mobile),
            new Claim(ClaimTypes.Role, role)
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

    // ✅ Protected API
    [Authorize]
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var mobile = User.Identity?.Name;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return Ok($"Welcome {mobile} ({role}), this is protected data.");
    }
}
