namespace Jobseeker.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class SendOtpModel
    {
        public string Mobile { get; set; }
    }

    public class VerifyOtpModel
    {
        public string Mobile { get; set; }
        public string Otp { get; set; }
    }

}

