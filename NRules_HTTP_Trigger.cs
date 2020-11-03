using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NRules;
using NRules.Fluent;
using Newtonsoft.Json.Linq;
using NRules.Fluent.Dsl;
using System.Reflection;
using Manulife.Rules;
using Manulife.Domain;


namespace Manulife
{
     static class OutputString{
        public static string responseMessage;
    }

    public class Rate
    {
        public int Id { get; private set; }
        public Customer Customer { get; private set; }
        public double RatePrice { get; private set; }
        public double PercentDiscount { get; private set; }

        public Rate(int id, Customer customer, double ratePrice)
        {
            Id = id;
            Customer = customer;
            RatePrice = ratePrice;
        }

        public void ApplyDiscount(double percentDiscount)
        {
            PercentDiscount = percentDiscount;
        }
    }

    // public class DiscountNotificationRule : Rule
    // {
    //     public override void Define()
    //     {
    //         Customer customer = null;

    //         When()
    //             .Match<Customer>(() => customer);

    //         Then()
    //             OutputString.responseMessage = "Discount Notification";
    //     }
    // }


    public static class NRules_HTTP_Trigger
    {
        [FunctionName("NRules_HTTP_Trigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //Getting Request Body and Parsing
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            JObject data = JObject.Parse(requestBody);
            string Name = (string)data.SelectToken("Name");
            bool Age = (bool)data.SelectToken("Age");
            string Occupation = (string)data.SelectToken("Occupation");
            string Occupation = (string)data.SelectToken("Gender");
            JArray PreExisingConditions = (JArray)data.SelectToken("PreExisingConditions");
            bool IsSmoker = (bool)data.SelectToken("IsSmoker");
            int MSinceHospital = (int)data.SelectToken("MSinceHospital");
            int PolicyMaximum = (int)data.SelectToken("PolicyMaximum");
            bool LookingForNewRate = (bool)data.SelectToken("LookingForNewRate");
            
            /*{
            "Name": "",
            "Age": ,
            "Occupation": "",
            "Gender": "",
            "PreExisingConditions": ["", ""],
            "IsSmoker": ,
            "MSinceHospital": ,
            "LookingForNewRate": ,
            "LookingForNewRate": ,
            }*/
            
            //Load rules
            var repository = new RuleRepository();
            repository.Load(x => x.From(typeof(CustomerRateRule).Assembly));

            //Compile rules
            var factory = repository.Compile();

            //Create a working session
            var session = factory.CreateSession();
                        
            //Load domain model
            var customer = new Customer(Name, IsPreferred, IsSmoker, MSinceHospital, PreExisingConditions.ToObject<string[]>(), ReasonForHospitalVisit, LookingForNewRate);
            //var rate = new Rate(ID, customer, RatePrice);

            //Insert facts into rules engine's memory
            session.Insert(customer);
            //session.Insert(rate);

            //Start match/resolve/act cycle
            session.Fire();
            

            log.LogInformation("C# HTTP trigger function processed a request.");;
 

            OutputString.responseMessage += string.IsNullOrEmpty(Name)
                ? "\nThis HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"\nHello, {Name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(OutputString.responseMessage);
        }
    }
}
