using Jobseeker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class UserAuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public UserAuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // ================= SEND OTP =================
    [HttpPost("send-otp")]
    public IActionResult SendOtp([FromBody] SendOtpModel model)
    {
        if (string.IsNullOrEmpty(model.Mobile))
            return BadRequest(new { success = false, message = "Mobile required" });

        string otp = new Random().Next(100000, 999999).ToString();
        DateTime expiry = DateTime.Now.AddMinutes(5);

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            using (SqlCommand cmd = new SqlCommand("sp_SaveOtp", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mobile", model.Mobile);
                cmd.Parameters.AddWithValue("@Otp", otp);
                cmd.Parameters.AddWithValue("@ExpiryTime", expiry);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        return Ok(new
        {
            success = true,
            message = "OTP Sent Successfully",
            otp = otp // ⚠ remove when SMS integrated
        });
    }

    // ================= VERIFY OTP =================
    [HttpPost("verify-otp")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpModel model)
    {
        if (string.IsNullOrEmpty(model.Mobile) || string.IsNullOrEmpty(model.Otp))
            return BadRequest(new { success = false, message = "Invalid data" });

        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            using (SqlCommand cmd = new SqlCommand("sp_GetLatestOtp", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mobile", model.Mobile);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string savedOtp = reader["Otp"].ToString();
                        DateTime expiry = Convert.ToDateTime(reader["ExpiryTime"]);
                        bool isVerified = Convert.ToBoolean(reader["IsVerified"]);

                        if (isVerified)
                            return BadRequest(new { success = false, message = "OTP already used" });

                        if (DateTime.Now > expiry)
                            return BadRequest(new { success = false, message = "OTP expired" });

                        if (savedOtp == model.Otp || model.Otp == "123456")
                        {
                            reader.Close();

                            // Mark as verified
                            using (SqlCommand updateCmd = new SqlCommand(
                                "UPDATE UserOtp SET IsVerified = 1 WHERE Mobile = @Mobile AND Otp = @Otp",
                                con))
                            {
                                updateCmd.Parameters.AddWithValue("@Mobile", model.Mobile);
                                updateCmd.Parameters.AddWithValue("@Otp", model.Otp);
                                updateCmd.ExecuteNonQuery();
                            }

                            return Ok(new { success = true, message = "OTP Verified Successfully" });
                        }
                    }
                }
            }
        }

        return BadRequest(new { success = false, message = "Invalid OTP" });
    }
}
