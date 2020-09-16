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
    public static class GetRating {
        [FunctionName("GetRating")]
        public async static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "rating/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "BFYOC",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosConnectionString",
                SqlQuery = "select * from ratings r where r.id = {id}")] IEnumerable<UserRating> rating,
            ILogger log)
        {


            if(! rating.Any())
                return new NotFoundObjectResult("Not Found");

            return new OkObjectResult(rating.First());
        }
    }
}
