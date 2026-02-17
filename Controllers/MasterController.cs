using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class MasterController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public MasterController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    // ===================== STATES =====================
    [HttpGet("states")]
    public IActionResult GetStates()
    {
        List<object> states = new List<object>();

        using (SqlConnection con = GetConnection())
        {
            string query = "SELECT StateCode, StateName FROM StateMaster";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                states.Add(new
                {
                    StateCode = reader["StateCode"],
                    StateName = reader["StateName"].ToString()
                });
            }
        }

        return Ok(states);
    }

    // ===================== DISTRICTS =====================
    [HttpGet("districts/{stateCode}")]
    public IActionResult GetDistricts(int stateCode)
    {
        List<object> districts = new List<object>();

        using (SqlConnection con = GetConnection())
        {
            string query = "SELECT DistrictCode, DistrictName FROM DistrictMaster WHERE StateCode = @StateCode";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@StateCode", stateCode);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                districts.Add(new
                {
                    DistrictCode = reader["DistrictCode"],
                    DistrictName = reader["DistrictName"].ToString()
                });
            }
        }

        return Ok(districts);
    }

    // ===================== CATEGORY =====================
    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        List<object> categories = new List<object>();

        using (SqlConnection con = GetConnection())
        {
            string query = "SELECT CategoryId, CategoryName FROM CategoryMaster";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(new
                {
                    CategoryId = reader["CategoryId"],
                    CategoryName = reader["CategoryName"].ToString()
                });
            }
        }

        return Ok(categories);
    }

    // ===================== QUALIFICATION =====================
    [HttpGet("qualifications")]
    public IActionResult GetQualifications()
    {
        List<object> list = new List<object>();

        using (SqlConnection con = GetConnection())
        {
            string query = "SELECT QualificationId, QualificationName FROM QualificationMaster";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new
                {
                    QualificationId = reader["QualificationId"],
                    QualificationName = reader["QualificationName"].ToString()
                });
            }
        }

        return Ok(list);
    }

    // ===================== UNIVERSITY =====================
    [HttpGet("universities")]
    public IActionResult GetUniversities()
    {
        List<object> list = new List<object>();

        using (SqlConnection con = GetConnection())
        {
            string query = "SELECT UniversityId, UniversityName FROM UniversityMaster";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new
                {
                    UniversityId = reader["UniversityId"],
                    UniversityName = reader["UniversityName"].ToString()
                });
            }
        }

        return Ok(list);
    }
}
