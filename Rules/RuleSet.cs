
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
                    .Yield(_ => new RateTracker(1, list, false, 0, 0));
                    
            }
        }
    public class AgeCheck: Rule
        {
            public override void Define()
            {
                Customer customer = null;
                RateTracker rateTracker = null;
                //Using rate tracker object to track variables and a trigger for other functions if EligibleForNewRate and LookingForNewRate is true
                When()
                    .Match(() => rateTracker)
                        .And(xx => xx 
                    .Match<Customer>(() => customer, c => c.Age != 0));
                    
                Then()
                //Calling function to apply the logic within this rule
                    .Do(ctx => ApplyAgeFactor(customer, ref rateTracker));
                    
            }
                private static void ApplyAgeFactor(Customer customer, ref RateTracker rateTracker)
            {
                    if(0< customer.Age && customer.Age <= 21){
                        OutputString.responseMessage = "age < 21";
                    }
                    else if (21< customer.Age && customer.Age <= 39){
                        rateTracker.RiskFactor  += 0.1;
                        OutputString.responseMessage = "21 < age < 39";
                    }
                     else if (39< customer.Age && customer.Age <= 59){
                        OutputString.responseMessage = "39 < age < 59 ";
                        rateTracker.RiskFactor  += 0.2;
                    }
                     else if (59< customer.Age){
                        OutputString.responseMessage = "60 < age ";
                        rateTracker.RiskFactor  += 0.3;
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
                //Using rate tracker object to track variables and a trigger for other functions if EligibleForNewRate and LookingForNewRate is true
                    .Match(() => rateTracker)
                        .And(xx => xx
                    .Match<Customer>(() => customer, c => c.IsSmoker));
                    
                Then()
                //Calling function to apply the logic within this rule
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
            //Using rate tracker object to track variables and a trigger for other functions if EligibleForNewRate and LookingForNewRate is true
                .Match(() => rateTracker)
                    .And(xx => xx
                .Match<Customer>(() => customer, c => c.Occupation.Length != 0));
                
            Then()
            //Calling function to apply the logic within this rule
                .Do(ctx => ApplyOccupationFactor(customer, ref rateTracker));
                
        }
            private static void ApplyOccupationFactor(Customer customer, ref RateTracker rateTracker)
        {
                if (customer.Occupation == "Roofer" || customer.Occupation == "Logger" || customer.Occupation == "Police"){
                    OutputString.responseMessage += "\nDangerous Job";
                    rateTracker.RiskFactor  += 0.3;
                    rateTracker.HighRiskTracker.Add("Dangerous Job");
                }      
        }
    }

    public class HospitalCheck: Rule
    {
        public override void Define()
        {
            Customer customer = null;
            RateTracker rateTracker = null;

            When()
            //Using rate tracker object to track variables and a trigger for other functions if EligibleForNewRate and LookingForNewRate is true
                .Match(() => rateTracker)
                    .And(xx => xx
                .Match<Customer>(() => customer, c => c.MSinceHospital >= 0));
                
            Then()
            //Calling function to apply the logic within this rule
                .Do(ctx => HospitalFactor(customer, ref rateTracker));
                
        }
            private static void HospitalFactor(Customer customer, ref RateTracker rateTracker)
        {
                if(customer.MSinceHospital == 0){
                    rateTracker.RiskFactor  += 0.4;
                    OutputString.responseMessage += "\n1 Hospital Visit This Month";
                }
                else if (customer.PreExisingConditions.Length == 2){
                    rateTracker.RiskFactor  += 0.4 * 1/customer.MSinceHospital;
                    OutputString.responseMessage += $"\nHospital Visit {customer.MSinceHospital} Months Ago";
                }             
        }
    }

    public class GenderCheck: Rule
    {
        public override void Define()
        {
            Customer customer = null;
            RateTracker rateTracker = null;

            When()
            //Using rate tracker object to track variables and a trigger for other functions if EligibleForNewRate and LookingForNewRate is true
                .Match(() => rateTracker)
                    .And(xx => xx
                .Match<Customer>(() => customer, c => !String.IsNullOrEmpty(c.Gender)));
                
            Then()
            //Calling function to apply the logic within this rule
                .Do(ctx => HospitalFactor(customer, ref rateTracker));
                
        }
            private static void HospitalFactor(Customer customer, ref RateTracker rateTracker)
        {
                if(customer.Gender == "Male"){
                    rateTracker.RiskFactor  += 0.1;
                    OutputString.responseMessage += "\nGender: Male";
                }
                else if (customer.Gender == "Female"){
                    rateTracker.RiskFactor  -= 0.1;
                    OutputString.responseMessage += "\nGender Female";
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
            //Calling function to apply the logic within this rule
                .Do(ctx => PreExisitngConditionFactor(customer, ref rateTracker));
                
        }
            private static void PreExisitngConditionFactor(Customer customer, ref RateTracker rateTracker)
        {
                if(customer.PreExisingConditions.Length == 1){
                    OutputString.responseMessage += $"\n1 Pre-Exisitng Conditions \nRiskFactor: {Math.Round(rateTracker.RiskFactor, 1)} \nHigh Risk Cases: {rateTracker.HighRiskTracker.Count}";
                }
                else if (customer.PreExisingConditions.Length == 2){
                    rateTracker.RiskFactor  += 0.2;
                    OutputString.responseMessage += $"\n2 Pre-Exisitng Conditions \nRiskFactor: {Math.Round(rateTracker.RiskFactor, 1)} \nHigh Risk Cases: {rateTracker.HighRiskTracker.Count}";
                }
                else if (customer.PreExisingConditions.Length > 2){
                    rateTracker.RiskFactor  += 0.4;
                    OutputString.responseMessage += $"\nHigh Risk Pre-Exisitng Conditions \nRiskFactor: {Math.Round(rateTracker.RiskFactor, 1)} \nHigh Risk Cases: {rateTracker.HighRiskTracker.Count}";
                    rateTracker.HighRiskTracker.Add("PreExisitngCond");
                }              
        }
    }

    public class HighRiskCheck: Rule
    {
        public override void Define()
        {
            RateTracker rateTracker = null;

            When()
                .Match(() => rateTracker);
            //IT SEEMS LIKE I CAN NOT INCLUDE LOGIC AROUND RATETRACKER HERE

            Then()
            //Calling function to apply the logic within this rule
                .Do(ctx => ApplyHighRiskFactor(ref rateTracker));
                
        }
            private static void ApplyHighRiskFactor(ref RateTracker rateTracker)
            {
                if(rateTracker.HighRiskTracker.Count > 2){
                    OutputString.responseMessage += "\nHigh Risk";
                    rateTracker.HighRisk = true;
                }
                else{
                    OutputString.responseMessage += "\nLow Risk";
                    rateTracker.HighRisk = false;
            }
            }      
    }

    public class ApplyPricing: Rule
    {
        public override void Define()
        {
            Customer customer = null;
            RateTracker rateTracker = null;
            Order order = null;

            When()
            //Using rate tracker object to track variables and a trigger for other functions if EligibleForNewRate and LookingForNewRate is true
                .Match(() => rateTracker)
                    .And(xx => xx
                .Match<Customer>(() => customer, c => !String.IsNullOrEmpty(c.InsuranceType)))
                    .And(xxx => xxx
                .Match<Order>(() => order, o => !String.IsNullOrEmpty(o.InsuranceType)));
                
            Then()
            //Calling function to apply the logic within this rule
                .Do(ctx => ApplyPricingFunction(customer, ref rateTracker, order));
                
        }
            private static void ApplyPricingFunction(Customer customer, ref RateTracker rateTracker, Order order) 
        {
            if (rateTracker.HighRisk == true && order.InsuranceType == "Participating"){
                OutputString.responseMessage += $"\nUnable to apply Insurance Type: Participating, Due to High Risk (Only Term Currently Available for This User)";
            }
            else if (rateTracker.HighRisk == true && order.InsuranceType == "Universal"){
                OutputString.responseMessage += $"\nUnable to apply Insurance Type: Universal, Due to High Risk (Only Term Currently Available for This User)";
            }
            else if(rateTracker.HighRisk == true && order.PolicyMaximum > 1000000){
                OutputString.responseMessage += $"\nCustomer is High Risk and Cannot Request a Policy Over $1000000";
            }
            else{
                if(customer.InsuranceType != order.InsuranceType){
                    rateTracker.OneTimeFee += 200;
                    OutputString.responseMessage += $"\nChanging Exisiting Insurance Type Therefore $200 Conversion One Time Fee";
                }
                if (order.InsuranceType == "Term"){
                    rateTracker.Cost = 300;
                    OutputString.responseMessage += $"\nTerm Insurance Availible Initial Cost Before RiskFactor: $300";
                }
                else if (order.InsuranceType == "Participating"){
                    rateTracker.Cost = 250;
                    OutputString.responseMessage += $"\nParticipating Insurance Availible Initial Cost Before RiskFactor: $250";
                }  
                else if (order.InsuranceType == "Universal"){
                    rateTracker.Cost = 200;
                    OutputString.responseMessage += $"\nUniversal Insurance Availible Initial Cost Before RiskFactor: $200";
                }
                if(order.PolicyMaximum < 100000){
                    rateTracker.Cost += 30;
                }
                else if (order.PolicyMaximum >= 100000 && order.PolicyMaximum <= 1000000){
                    rateTracker.Cost += 60;
                }
                else if (order.PolicyMaximum > 1000000){
                    rateTracker.Cost += 80;
                }
                    OutputString.responseMessage += $"\n\nFinal Cost With Risk Factor of {Math.Round(rateTracker.RiskFactor, 1)} and Policy Maximum of {order.PolicyMaximum} is ${Math.Round(rateTracker.Cost*rateTracker.RiskFactor, 2)}"; 
            }         
        }
    }
}
