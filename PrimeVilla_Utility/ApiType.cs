namespace PrimeVilla_Utility
{
    public static class SD
    {
        public enum ApiType
        {
            Get,
            Post,
            Put,
            Delete,
        }
        public static string AccessToken = "JWTToken";
        public static string RefreshToken = "RefreshToken";
        public static string ApiVersion = "v2";
        public static string Admin = "admin";
        public static string Customer = "customer";
        public enum ContentType
        {
            Json, 
            MultipartFormData,
        }
    }
}
