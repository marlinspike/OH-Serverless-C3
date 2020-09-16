using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using com.cleetus.models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace com.cleetus
{
    public static class GetRating
    {
        [FunctionName("GetRatings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ratings/{userId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "BFYOC",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosConnectionString",
                SqlQuery = "select * from ratings r where r.userId = {userId}")]  IEnumerable<UserRating> ratings,
            ILogger log)
        {


            if (! ratings.Any()) {
                log.LogInformation("Item not found");
                //rating = new UserRating();
                return new NotFoundObjectResult("user not found");
            }

            return new OkObjectResult(ratings);
        }
    }
}
