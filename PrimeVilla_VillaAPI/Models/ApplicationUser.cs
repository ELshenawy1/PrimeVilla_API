using Microsoft.AspNetCore.Identity;

namespace PrimeVilla_VillaAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

    }
}
