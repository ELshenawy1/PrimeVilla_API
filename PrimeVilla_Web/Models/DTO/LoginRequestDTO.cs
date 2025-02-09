using System.ComponentModel.DataAnnotations;

namespace PrimeVilla_Web.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
