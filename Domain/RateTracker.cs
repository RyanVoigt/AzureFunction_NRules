using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NRules;
using NRules.Fluent.Dsl;
using Manulife.Rules;
using System.Collections;
using System.Collections.Generic;


namespace Manulife.Domain
{
 public class RateTracker{
        public double RiskFactor { get; set; }
        public double Cost { get; set; }
        public double OneTimeFee { get; set; }
        public List<string> HighRiskTracker = new List<string>();
        public bool HighRisk { get; set; }

    // Note: fix to case to conform with .NET naming convention
  
        // Universal, Participating, Term
    
        public RateTracker(double riskFactor, List<String> highRiskTracker,bool highRisk, double cost, double oneTimeFee)
        {
            RiskFactor = riskFactor;
            HighRiskTracker = highRiskTracker;
            HighRisk = highRisk;
            Cost = cost;
            OneTimeFee = oneTimeFee;
        }
    }
}