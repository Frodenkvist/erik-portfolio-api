using System.ComponentModel.DataAnnotations;

namespace ErikPortfolioApi.Model
{
    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
