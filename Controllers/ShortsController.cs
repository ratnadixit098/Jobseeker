using Jobseeker.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class ShortsController : ControllerBase
{
    private readonly DbHelper _db;

    public ShortsController(DbHelper db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetShorts()
    {
        var list = new List<object>();

        using (SqlConnection con = _db.GetConnection())
        {
            con.Open();
            string query = @"
                SELECT VideoId, Title, VideoUrl
                FROM Video
                WHERE CategoryId = 2";

            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    VideoId = dr["VideoId"],
                    Title = dr["Title"],
                    VideoUrl = dr["VideoUrl"]
                });
            }
        }

        return Ok(list);
    }
}
