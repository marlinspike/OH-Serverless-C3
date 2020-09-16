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

namespace com.cleetus
{
    public static class HTTP_SaveRating
    {
        [FunctionName("HTTP_SaveRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
            databaseName: "BFYOC",
            collectionName: "ratings",
            ConnectionStringSetting = "CosmosConnectionString")] IAsyncCollector<UserRating> cosmosDoc,
            ILogger log) {

            try {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var user_rating = JsonConvert.DeserializeObject<UserRating>(requestBody);
                await cosmosDoc.AddAsync(user_rating);
            } catch (Exception e){
                log.LogError($"Insert Error: {e}");
            }

            return new OkObjectResult("Created");
        }
    }

    
}
