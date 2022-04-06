using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Supplier_AzureTBLRModel.Entities;
using Supplier_AzureTBLRModel.ViewModels;

namespace GetSupplierByIdFunc_AzureTBL
{
    public static class Function1
    {
        [FunctionName("GetSupplier")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string id = req.Query["id"];

                string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

                if (string.IsNullOrEmpty(id))
                {
                    return new OkObjectResult(new ResponseModel() { Data = "Id was not supplied", Success = false });
                }
                CloudStorageAccount storageAcc = CloudStorageAccount.Parse(connectionString);

                //// create table client
                CloudTableClient tblclient = storageAcc.CreateCloudTableClient(new TableClientConfiguration());

                // get customer table
                CloudTable cloudTable = tblclient.GetTableReference("Suppliers");

                TableOperation retrieveOperation = TableOperation.Retrieve<Supplier>(id, id);
                TableResult retrievedResult = cloudTable.Execute(retrieveOperation);

                Supplier supplier = (Supplier)retrievedResult.Result;


                return new OkObjectResult(new ResponseModel() { Data = supplier, Success = true });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new ResponseModel() { Data = e.Message, Success = true });
            }
        }
    }
}
