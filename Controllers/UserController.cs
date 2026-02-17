using Jobseeker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _config;

    public UserController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("final-submit")]
    public IActionResult FinalSubmit([FromBody] UserRegistrationDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid Data");

        string connString = _config.GetConnectionString("DefaultConnection");

        using (SqlConnection conn = new SqlConnection(connString))
        {
            conn.Open();

            // Optional: Duplicate mobile check
            using (SqlCommand checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Mobile=@Mobile", conn))
            {
                checkCmd.Parameters.AddWithValue("@Mobile", dto.Mobile);
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                    return BadRequest("User already registered with this mobile");
            }

            string query = @"INSERT INTO Users
                (FullName, Mobile, Email, Dob, StateId, DistrictId, City, Pincode,
                 QualificationId, Stream, PassingYear, UniversityId, GovtType, CategoryId, Languages)
                VALUES
                (@FullName, @Mobile, @Email, @Dob, @StateId, @DistrictId, @City, @Pincode,
                 @QualificationId, @Stream, @PassingYear, @UniversityId, @GovtType, @CategoryId, @Languages)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FullName", dto.FullName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mobile", dto.Mobile ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", dto.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Dob", dto.Dob ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StateId", dto.StateId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DistrictId", dto.DistrictId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", dto.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Pincode", dto.Pincode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@QualificationId", dto.QualificationId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Stream", dto.Stream ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PassingYear", dto.PassingYear ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UniversityId", dto.UniversityId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GovtType", dto.GovtType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryId", dto.CategoryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Languages", dto.Languages != null ? string.Join(",", dto.Languages) : (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        return Ok(new { success = true, message = "User Registered Successfully" });
    }
}
