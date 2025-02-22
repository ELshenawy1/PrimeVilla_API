using PrimeVilla_Web.Models.DTO;

namespace PrimeVilla_Web.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(TokenDTO tokenDto);
        TokenDTO? GetToken();
        void ClearTokne();
    }
}
