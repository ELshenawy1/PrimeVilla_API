using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace PrimeVilla_VillaAPI.Models.DTO
{
    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
