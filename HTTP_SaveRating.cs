using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using com.cleetus.models;
using System.Net.Http;

namespace com.cleetus
{
    public static class HTTP_SaveRating {
        [FunctionName("SaveRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
            databaseName: "BFYOC",
            collectionName: "ratings",
            ConnectionStringSetting = "CosmosConnectionString")] IAsyncCollector<UserRating> cosmosDoc,
            ILogger log) {
                //CI build kickoff

            UserRating user_rating = null;
            try {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                user_rating = JsonConvert.DeserializeObject<UserRating>(requestBody);
                user_rating.Id = Guid.NewGuid().ToString(); //Manually add a Guid so we can return it
                user_rating.timestamp = DateTime.UtcNow.ToString(); //Add UTC time so we can return it

                bool is_user_valid = await isUserIdValid(user_rating.userId);
                bool is_product_id_valid = await isProductIdValid(user_rating.productId);
                bool is_rating_valid = isRatingValid(Int16.Parse(user_rating.rating));

                //if isRatingValid(user_rating.rating)
                if (is_user_valid && is_product_id_valid && is_rating_valid) {
                    await cosmosDoc.AddAsync(user_rating);
                }

            } catch (Exception e){
                log.LogError($"Insert Error: {e}");
            }

            return new OkObjectResult(user_rating);
        }

        //Validates rating
        private static bool isRatingValid(int rating) {
            return (rating >= 0 && rating <= 5) ? true : false;
        } //endfunc

        //Validates UseID
        private static async Task<bool> isUserIdValid(string user_id) {
            string GET_USER_ID_URL = "https://serverlessohuser.trafficmanager.net/api/GetUser/";
            bool is_valid = false;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{GET_USER_ID_URL}?userId={user_id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var userInfo = JsonConvert.DeserializeObject<UserRating>(apiResponse);
                        is_valid = userInfo.userName.Length > 0 ? true : false;
                    }

                    return is_valid;
                }
            }
        } //endfunc

        //Validates ProductId
        private static async Task<bool> isProductIdValid(string product_id) {
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
        }//endfunc


    }

    
}
