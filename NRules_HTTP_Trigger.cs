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
using NRules.Fluent.Dsl;

namespace Manulife
{
     static class OutputString{
        public static string responseMessage;
    }

    public class Customer{
        public string Name { get; private set; }
        public bool IsPreferred { get; set; }

        public Customer(string name)
        {
            Name = name;
        }

        public void NotifyAboutDiscount()
        {
            Console.WriteLine("Customer {0} was notified about a discount", Name);
        }
    }

public class Order
{
    public int Id { get; private set; }
    public Customer Customer { get; private set; }
    public int Quantity { get; private set; }
    public double UnitPrice { get; private set; }
    public double PercentDiscount { get; private set; }
    public bool IsDiscounted { get { return PercentDiscount > 0; } }

    public double Price
    {
        get { return UnitPrice*Quantity*(1.0 - PercentDiscount/100.0); }
    }

    public Order(int id, Customer customer, int quantity, double unitPrice)
    {
        Id = id;
        Customer = customer;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void ApplyDiscount(double percentDiscount)
    {
        PercentDiscount = percentDiscount;
    }
}
public class PreferredCustomerDiscountRule : Rule
{
    public override void Define()
    {
        Customer customer = null;
        IEnumerable<Order> orders = null;

        When()
            .Match<Customer>(() => customer, c => c.IsPreferred)
            .Query(() => orders, x => x
                .Match<Order>(
                    o => o.Customer == customer,
                    o => !o.IsDiscounted)
                .Collect()
                .Where(c => c.Any()));

        Then()
            .Do(ctx => ApplyDiscount(orders, 10.0))
            .Do(ctx => ctx.UpdateAll(orders));
            OutputString.responseMessage = "Discount Applied";

    }

    private static void ApplyDiscount(IEnumerable<Order> orders, double discount)
    {
        foreach (var order in orders)
        {
            order.ApplyDiscount(discount);
            OutputString.responseMessage = "Discount Applied";
        }
    }
}

public class DiscountNotificationRule : Rule
{
    public override void Define()
    {
        Customer customer = null;

        When()
            .Match<Customer>(() => customer)
            .Exists<Order>(o => o.Customer == customer, o => o.PercentDiscount > 0.0);

        Then()
            .Do(_ => customer.NotifyAboutDiscount());
            OutputString.responseMessage = "Discount Applied";
    }
}


    public static class NRules_HTTP_Trigger
    {
        [FunctionName("NRules_HTTP_Trigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //Load rules
            var repository = new RuleRepository();
            repository.Load(x => x.From(typeof(PreferredCustomerDiscountRule).Assembly));

            //Compile rules
            var factory = repository.Compile();

            //Create a working session
            var session = factory.CreateSession();
                        
            //Load domain model
            var customer = new Customer("John Doe") {IsPreferred = false};
            var order1 = new Order(123456, customer, 2, 25.0);
            var order2 = new Order(123457, customer, 1, 100.0);

            //Insert facts into rules engine's memory
            session.Insert(customer);
            session.Insert(order1);
            session.Insert(order2);

            //Start match/resolve/act cycle
            session.Fire();
            

            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            OutputString.responseMessage += string.IsNullOrEmpty(name)
                ? "\nThis HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"\nHello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(OutputString.responseMessage);
        }
    }
}
