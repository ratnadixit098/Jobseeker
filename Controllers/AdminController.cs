using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jobseeker.Models;

namespace Jobseeker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AdminController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost]
        [Route("SaveJob")]
        public IActionResult SaveJob([FromForm] AddNewJob job)
        {
            if (job == null)
                return BadRequest("Job model is null");

            string filePath = null;

            // ===== SAFE ROOT PATH FIX =====
            string rootPath = _env.WebRootPath;

            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            // ===== SAVE PDF =====
            if (job.OfficialNotification != null && job.OfficialNotification.Length > 0)
            {
                string folderPath = Path.Combine(rootPath, "notifications");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() +
                                  Path.GetExtension(job.OfficialNotification.FileName);

                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    job.OfficialNotification.CopyTo(stream);
                }

                filePath = "/notifications/" + fileName;
            }

            string query = @"
                INSERT INTO Jobs
                (JobTitle, RecruitingOrganization, PostName, TotalVacancies,
                 CategoryWiseBreakup, QualificationRequired, AgeLimit,
                 SalaryPayLevel, JobLocation, SelectionProcess,
                 ApplicationStartDate, LastDate, ExamDate,
                 OfficialNotificationPath, ApplyLink,
                 SourceDepartmentWebsite, Status)
                VALUES
                (@JobTitle, @RecruitingOrganization, @PostName, @TotalVacancies,
                 @CategoryWiseBreakup, @QualificationRequired, @AgeLimit,
                 @SalaryPayLevel, @JobLocation, @SelectionProcess,
                 @ApplicationStartDate, @LastDate, @ExamDate,
                 @OfficialNotificationPath, @ApplyLink,
                 @SourceDepartmentWebsite, @Status)";

            using (SqlConnection con =
                   new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@JobTitle", job.JobTitle);
                    cmd.Parameters.AddWithValue("@RecruitingOrganization", job.RecruitingOrganization);
                    cmd.Parameters.AddWithValue("@PostName", job.PostName);
                    cmd.Parameters.AddWithValue("@TotalVacancies", job.TotalVacancies);
                    cmd.Parameters.AddWithValue("@CategoryWiseBreakup", job.CategoryWiseBreakup ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QualificationRequired", job.QualificationRequired ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AgeLimit", job.AgeLimit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SalaryPayLevel", job.SalaryPayLevel ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@JobLocation", job.JobLocation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SelectionProcess", job.SelectionProcess ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApplicationStartDate", job.ApplicationStartDate);
                    cmd.Parameters.AddWithValue("@LastDate", job.LastDate);
                    cmd.Parameters.AddWithValue("@ExamDate", job.ExamDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OfficialNotificationPath", filePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApplyLink", job.ApplyLink ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SourceDepartmentWebsite", job.SourceDepartmentWebsite ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", job.Status ?? "Draft");

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return Ok(new { message = "Job Saved Successfully ✅" });
        }
    }
}
