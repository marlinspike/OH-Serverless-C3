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

namespace com.cleetus
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "BFYOC",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosConnectionString",
                SqlQuery = "SELECT * from ratings")] IEnumerable<UserRating> ratings,
                //Id = "{Query.id}",
                //PartitionKey = "{Query.pk}")]  dynamic rating,
            ILogger log)
        {
            UserRating res = new UserRating();
            var user_id = req.Query["id"].ToString();

            log.LogInformation($"Querying for rating: {req.Query["id"]}");
            //var user_rating = ratings.Where(r => r.Id.Equals(user_id));
            var user_rating = from UserRating r in ratings
                where r.Id == user_id
                select r;
            if (user_rating == null){
                log.LogInformation("Item not found");
                //rating = new UserRating();
                res.fullName = "Not found guy";
            }

            return new OkObjectResult(user_rating);
        }
    }
}
