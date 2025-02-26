using Newtonsoft.Json;
using PrimeVilla_Utility;
using PrimeVilla_Web.Models;
using PrimeVilla_Web.Services.IServices;
using System.Text;

namespace PrimeVilla_Web.Services
{
    public class ApiMessageRequestBuilder : IApiMessageRequestBuilder
    {
        public HttpRequestMessage Build(APIRequest apiRequest)
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", (apiRequest.ContentType == SD.ContentType.Json) ? "application/json" : "*/*");
            message.RequestUri = new Uri(apiRequest.Url);

            if (apiRequest.ContentType == SD.ContentType.MultipartFormData)
            {
                var content = new MultipartFormDataContent();
                foreach (var prop in apiRequest.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(apiRequest.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                        {
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                    }
                }
                message.Content = content;
            }
            else
            {
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }
            }



            switch (apiRequest.ApiType)
            {
                case SD.ApiType.Post:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.Put:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.Delete:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            return message;
        }
    }
}
