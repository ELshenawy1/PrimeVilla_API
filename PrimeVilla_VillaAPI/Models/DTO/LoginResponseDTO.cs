using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace PrimeVilla_VillaAPI.Models.DTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
