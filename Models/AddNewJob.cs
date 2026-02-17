namespace Jobseeker.Models
{
    public class AddNewJob
    {
        
        public int JobId { get; set; }

        public string JobTitle { get; set; }
        public string RecruitingOrganization { get; set; }
        public string PostName { get; set; }
        public int TotalVacancies { get; set; }
        public string CategoryWiseBreakup { get; set; }
        public string QualificationRequired { get; set; }
        public string AgeLimit { get; set; }
        public string SalaryPayLevel { get; set; }
        public string JobLocation { get; set; }
        public string SelectionProcess { get; set; }

        public DateTime ApplicationStartDate { get; set; }
        public DateTime LastDate { get; set; }
        public DateTime? ExamDate { get; set; }

        public string ApplyLink { get; set; }
        public string SourceDepartmentWebsite { get; set; }
        public string Status { get; set; }

        public IFormFile OfficialNotification { get; set; } // PDF File
    
}
}
