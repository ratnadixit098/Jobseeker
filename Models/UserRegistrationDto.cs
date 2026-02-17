namespace Jobseeker.Models
{
    public class UserRegistrationDto
    {
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime? Dob { get; set; }

        public string StateId { get; set; }
        public string DistrictId { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }

        public int? QualificationId { get; set; }
        public string Stream { get; set; }
        public int? PassingYear { get; set; }
        public int? UniversityId { get; set; }

        public string GovtType { get; set; }
        public int? CategoryId { get; set; }

        public List<string> Languages { get; set; }
    }

}
