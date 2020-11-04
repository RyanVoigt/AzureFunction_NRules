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
        public List<string> HighRiskTracker = new List<string>();

    // Note: fix to case to conform with .NET naming convention
  
        // Universal, Participating, Term
    
        public RateTracker(double riskFactor, List<String> highRiskTracker)
        {
            RiskFactor = riskFactor;
            HighRiskTracker = highRiskTracker;
        }
    }
}