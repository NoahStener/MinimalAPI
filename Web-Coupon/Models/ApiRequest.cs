using Microsoft.AspNetCore.Mvc;
using static Web_Coupon.StaticDetails;

namespace Web_Coupon.Models
{
    public class ApiRequest
    {
        public ApiType apiType { get; set; }

        public string Url { get; set; }
        public Object Data { get; set; }
        public string AccessToken { get; set; }
    }
}
