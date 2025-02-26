using PrimeVilla_Utility;
using PrimeVilla_Web.Models;
using PrimeVilla_Web.Models.DTO;
using PrimeVilla_Web.Services.IServices;

namespace PrimeVilla_Web.Services
{

    public class VillaService : IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBaseService _baseService;
        private string villaUrl;

        public VillaService(IHttpClientFactory clientFactory, IConfiguration configuration, IBaseService baseService)
        {
            _baseService = baseService;
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("SerivceUrls:VillaAPI");
        }
        public async Task<T> CreateAsync<T>(VillaCreateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Post,
                Data = dto,
                Url = villaUrl + $"/api/{SD.ApiVersion}/VillaApi",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Delete,
                Url = villaUrl + $"/api/{SD.ApiVersion}/VillaApi/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = villaUrl + $"/api/{SD.ApiVersion}/VillaApi"
            },withBearer:false);
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = villaUrl + $"/api/{SD.ApiVersion}/VillaApi/" + id
            });
        }

        public async Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Put,
                Data = dto,
                Url = villaUrl + $"/api/{SD.ApiVersion}/VillaApi/" + dto.Id,
                ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}
