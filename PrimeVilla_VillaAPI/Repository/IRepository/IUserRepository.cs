using PrimeVilla_VillaAPI.Models;
using PrimeVilla_VillaAPI.Models.DTO;

namespace PrimeVilla_VillaAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
 