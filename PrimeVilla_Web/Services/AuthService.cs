using PrimeVilla_Utility;
using PrimeVilla_Web.Models;
using PrimeVilla_Web.Models.DTO;
using PrimeVilla_Web.Services.IServices;

namespace PrimeVilla_Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string villaUrl;
        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseService baseService)
        {
            _baseService = baseService;
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("SerivceUrls:VillaAPI");
        }

        public async Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Post,
                Data = obj,
                Url = villaUrl + $"/api/{SD.ApiVersion}/UserAuth/Login"
            },withBearer:false);
        }

        public async Task<T> LogoutAsync<T>(TokenDTO tokenDto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Post,
                Data = tokenDto,
                Url = villaUrl + $"/api/{SD.ApiVersion}/UserAuth/Revoke"
            });
        }

        public async Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Post,
                Data = obj,
                Url = villaUrl + $"/api/{SD.ApiVersion}/UserAuth/Register"
            }, withBearer: false);
        }
    }
}
