namespace CookieAuthentication.Models
{
    public class UserModel
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string Role { get; set; } //Should define enum constant
    }
}
