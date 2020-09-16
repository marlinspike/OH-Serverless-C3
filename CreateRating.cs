using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using com.cleetus.models;

namespace com.cleetus
{


    public static class CreateRating {

        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = null)] HttpRequest req,
            ILogger log) {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //var data = JsonConvert.DeserializeObject<UserRating>(requestBody);
            //var user_id = data?.userId;
            //var is_valid = isUserIdValid(data?.userId).GetAwaiter().GetResult();
            var data = JsonConvert.DeserializeObject<Product>(requestBody);
            var user_id = data?.productId;
            var is_valid = isProductIdValid(data?.productId).GetAwaiter().GetResult();


            return new OkObjectResult($"IsValid: {is_valid}");
        }

        //Validates rating
        private static bool isRatingValid(int rating){
            return (rating >= 0 && rating <= 5) ? true : false;
        }

        //Validates UseID
        private static async Task<bool> isUserIdValid(string user_id) {
            string GET_USER_ID_URL = "https://serverlessohuser.trafficmanager.net/api/GetUser/";
            bool is_valid = false;
            using (var httpClient = new HttpClient()) {
                using (var response = await httpClient.GetAsync($"{GET_USER_ID_URL}?userId={user_id}")) {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var userInfo = JsonConvert.DeserializeObject<UserRating>(apiResponse);
                        is_valid = userInfo.userName.Length > 0 ? true : false;
                    }

                    return is_valid;
                }
            }
        }

        //Validates ProductId
        private static async Task<bool> isProductIdValid(string product_id){
            string GET_PRODUCT_ID_URL = "https://serverlessohproduct.trafficmanager.net/api/GetProduct/";
            bool is_valid = false;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{GET_PRODUCT_ID_URL}?productId={product_id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var productInfo = JsonConvert.DeserializeObject<Product>(apiResponse);
                        is_valid = productInfo.productId.Length > 0 ? true : false;
                    }

                    return is_valid;
                }
            }
        }
    }

}
