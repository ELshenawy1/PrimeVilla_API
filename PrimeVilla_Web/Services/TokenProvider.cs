using PrimeVilla_Utility;
using PrimeVilla_Web.Models.DTO;
using PrimeVilla_Web.Services.IServices;

namespace PrimeVilla_Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void ClearTokne()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.RefreshToken);
        }

        public TokenDTO GetToken()
        {
            try
            {
                bool hasAccessToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string accessToken);
                bool hasRefreshToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.RefreshToken, out string refreshToken);

                TokenDTO tokenDto = new()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                return (hasAccessToken && hasRefreshToken) ? tokenDto : null;
            }
            catch
            {
                return null;
            }
        }

        public void SetToken(TokenDTO tokenDto)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddMinutes(59) };
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken,tokenDto.AccessToken, cookieOptions);
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.RefreshToken,tokenDto.RefreshToken, cookieOptions);
        }
    }
}
