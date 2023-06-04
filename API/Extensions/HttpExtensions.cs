using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response,PaginationHeader header){
            //header should be in JSON format
            //and not in c# object format
            var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            response.Headers.Add("Pagination",JsonSerializer.Serialize(header,jsonOptions));
            //for custom header we have to explicitly allow CORS policy
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
            
        }
    }
}