using Jobseeker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    private string ConnStr => _config.GetConnectionString("DefaultConnection");

    // ================= SEND OTP =================

    [HttpPost("send-otp")]
    public IActionResult SendOtp([FromBody] SendOtpRequest request)
    {
        var otp = new Random().Next(100000, 999999).ToString();

        using SqlConnection con = new SqlConnection(ConnStr);

        string query = @"INSERT INTO OtpVerifications (Mobile, Otp, ExpiryTime, IsUsed)
                         VALUES (@Mobile, @Otp, DATEADD(MINUTE,2,GETDATE()), 0)";

        using SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@Mobile", request.Mobile);
        cmd.Parameters.AddWithValue("@Otp", otp);

        con.Open();
        cmd.ExecuteNonQuery();

        return Ok(new { message = "OTP sent", otp }); // testing
    }

    // ================= VERIFY OTP + REGISTER =================

    [HttpPost("verify-register")]
    public IActionResult VerifyAndRegister([FromBody] VerifyRegisterRequest request)
    {
        using SqlConnection con = new SqlConnection(ConnStr);
        con.Open();

        // 🔥 BYPASS OTP (123456 allowed)
        bool isBypassOtp = request.Otp == "123456";
        bool isOtpValid = false;

        if (isBypassOtp)
        {
            isOtpValid = true;
        }
        else
        {
            // 🔹 check OTP from DB
            string otpQuery = @"SELECT TOP 1 ExpiryTime FROM OtpVerifications
                                WHERE Mobile=@Mobile AND Otp=@Otp AND IsUsed=0
                                ORDER BY Id DESC";

            using SqlCommand otpCmd = new SqlCommand(otpQuery, con);
            otpCmd.Parameters.AddWithValue("@Mobile", request.Mobile);
            otpCmd.Parameters.AddWithValue("@Otp", request.Otp);

            var result = otpCmd.ExecuteScalar();

            if (result == null)
                return BadRequest("Invalid OTP");

            DateTime expiry = Convert.ToDateTime(result);

            if (expiry < DateTime.Now)
                return BadRequest("OTP expired");

            isOtpValid = true;

            // mark OTP used
            string updateOtp = @"UPDATE OtpVerifications
                                 SET IsUsed = 1
                                 WHERE Mobile=@Mobile AND Otp=@Otp";

            using SqlCommand updCmd = new SqlCommand(updateOtp, con);
            updCmd.Parameters.AddWithValue("@Mobile", request.Mobile);
            updCmd.Parameters.AddWithValue("@Otp", request.Otp);
            updCmd.ExecuteNonQuery();
        }

        if (!isOtpValid)
            return BadRequest("OTP verification failed");

        // 🔹 check duplicate user
        string checkUser = "SELECT COUNT(*) FROM Users_Registration WHERE Mobile=@Mobile";

        using SqlCommand chkCmd = new SqlCommand(checkUser, con);
        chkCmd.Parameters.AddWithValue("@Mobile", request.Mobile);

        int exists = (int)chkCmd.ExecuteScalar();
        if (exists > 0)
            return BadRequest("User already exists");

        // 🔹 SAVE USER ✅
        string insertUser = @"INSERT INTO Users_Registration
                             (FullName, Email, Mobile, IsVerified, CreatedAt)
                             VALUES (@FullName, @Email, @Mobile, 1, GETDATE())";

        using SqlCommand insCmd = new SqlCommand(insertUser, con);
        insCmd.Parameters.AddWithValue("@FullName", request.FullName);
        insCmd.Parameters.AddWithValue("@Email", request.Email);
        insCmd.Parameters.AddWithValue("@Mobile", request.Mobile);

        insCmd.ExecuteNonQuery();

        return Ok("User registered successfully");
    }
}
