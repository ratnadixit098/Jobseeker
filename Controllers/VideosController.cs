using Microsoft.AspNetCore.Mvc;
using Jobseeker.Data;
using Jobseeker.Models;
using System.Data.SqlClient;

namespace Jobseekers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private readonly DbHelper _db;

        public VideosController(DbHelper db)
        {
            _db = db;
        }

        // GET videos with category
        [HttpGet]
        public IActionResult GetVideos()
        {
            var list = new List<object>();

            using (SqlConnection con = _db.GetConnection())
            {
                con.Open();
                string query = @"
                  SELECT v.VideoId, v.Title, v.VideoUrl, c.CategoryName
                  FROM Video v
                  INNER JOIN Category c ON v.CategoryId = c.CategoryId";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        VideoId = dr["VideoId"],
                        Title = dr["Title"],
                        VideoUrl = dr["VideoUrl"],
                        Category = dr["CategoryName"]
                    });
                }
            }

            return Ok(list);
        }
       

    }
}
