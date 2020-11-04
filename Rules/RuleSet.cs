
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
                List<string> list = new List<string>();

                When()
                //Checks to see if there is a customer that is eligible for a new Rate
                    .Match<Customer>(() => customer, c => c.EligibleForNewRate)
                //Then checks input orders to see if one of those customers eligible is in the order requested and if the order is asking for a new Rate
                    .Exists<Order>(o => o.ID == customer.ID, o => o.Name == customer.Name, o => o.LookingForNewRate == true);

                Then()
                    .Yield(_ => new RateTracker(1, list));
                    
            }
        }
    public class AgeCheck: Rule
        {
            public override void Define()
            {
                Customer customer = null;
                RateTracker rateTracker = null;

                When()
                    .Match(() => rateTracker)
                        .And(xx => xx
                    .Match<Customer>(() => customer, c => c.Age != 0));
                    
                Then()
                    .Do(ctx => ApplyAgeFactor(customer, ref rateTracker));
                    
            }
                private static void ApplyAgeFactor(Customer customer, ref RateTracker rateTracker)
            {
                    if(0< customer.Age && customer.Age <= 21){
                        OutputString.responseMessage += "age < 21";
                    }
                    else if (21< customer.Age && customer.Age <= 39){
                        OutputString.responseMessage += "21 < age < 39";
                        rateTracker.RiskFactor  += 0.1;
                    }
                     else if (39< customer.Age && customer.Age <= 59){
                        OutputString.responseMessage += "39 < age < 59";
                        rateTracker.RiskFactor  += 0.2;
                    }
                     else if (59< customer.Age){
                        OutputString.responseMessage += "60 < age ";
                        rateTracker.RiskFactor  += 0.2;
                        rateTracker.HighRiskTracker.Add("Age");
                    }
                    
            }
        }

    public class SmokerCheck: Rule
        {
            public override void Define()
            {
                Customer customer = null;
                RateTracker rateTracker = null;

                When()
                    .Match(() => rateTracker)
                        .And(xx => xx
                    .Match<Customer>(() => customer, c => c.IsSmoker));
                    
                Then()
                    .Do(ctx => ApplySmokerFactor(customer, ref rateTracker));
                    
            }
                private static void ApplySmokerFactor(Customer customer, ref RateTracker rateTracker)
                {
                        OutputString.responseMessage += "\nIs Smoker";
                        rateTracker.RiskFactor  += 0.7;
                        rateTracker.HighRiskTracker.Add("Smoker");
                       
                }
        }

    public class OccupationCheck: Rule
    {
        public override void Define()
        {
            Customer customer = null;
            RateTracker rateTracker = null;

            When()
                .Match(() => rateTracker)
                    .And(xx => xx
                .Match<Customer>(() => customer, c => c.Occupation.Length != 0));
                
            Then()
                .Do(ctx => ApplyOccupationFactor(customer, ref rateTracker));
                
        }
            private static void ApplyOccupationFactor(Customer customer, ref RateTracker rateTracker)
        {
                if (customer.Occupation == "Roofer" || customer.Occupation == "Logger" || customer.Occupation == "Police"){
                    OutputString.responseMessage += "\nDangerous Job";
                    rateTracker.RiskFactor  += 0.2;
                    rateTracker.HighRiskTracker.Add("Dangerous Job");
                }      
        }
    }

    public class PreExistingConditionCheck: Rule
    {
        public override void Define()
        {
            Customer customer = null;
            RateTracker rateTracker = null;

            When()
                .Match(() => rateTracker)
                    .And(xx => xx
                .Match<Customer>(() => customer, c => c.PreExisingConditions.Length != 0));
                
            Then()
                .Do(ctx => PreExisitngConditionFactor(customer, ref rateTracker));
                
        }
            private static void PreExisitngConditionFactor(Customer customer, ref RateTracker rateTracker)
        {
                if(customer.PreExisingConditions.Length == 1){
                    OutputString.responseMessage += "\n1 Pre-Exisitng Conditions";
                }
                else if (customer.PreExisingConditions.Length == 2){
                    OutputString.responseMessage += "\n2 Pre-Exisitng Conditions";
                    rateTracker.RiskFactor  += 0.1;
                }
                else if (customer.PreExisingConditions.Length > 2){
                    OutputString.responseMessage += "\nHigh Risk Pre-Exisitng Conditions";
                    rateTracker.RiskFactor  += 0.2;
                    rateTracker.HighRiskTracker.Add("PreExisitngCond");
                }              
        }
    }
}