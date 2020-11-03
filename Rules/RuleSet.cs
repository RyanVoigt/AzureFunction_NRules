
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
using Manulife.Domain;

namespace Manulife.Rules
{
    public class CustomerRateRule: Rule
        {
            public override void Define()
            {
                Customer customer = null;

                When()
                    .Match<Customer>(() => customer, c => c.LookingForNewRate == true);

                Then()
                    .Do(ctx => UpdateMessage("Customer Is looking for new Rate"));
                    
            }
                private static void UpdateMessage(string message)
            {
                    OutputString.responseMessage = message;
            }
        }
}