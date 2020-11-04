using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NRules;
using NRules.Fluent.Dsl;
using Manulife.Rules;

namespace Manulife.Domain
{
 public class Order{
        public bool LookingForNewRate { get; set; }
        public int PolicyMaximum { get; set; }
        public string InsuranceType { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
  
        // Universal, Participating, Term
    
        public Order(string name, int iD, bool lookingForNewRate, int policyMaximum, string insuranceType)
        {
            Name = name;
            ID = iD;
            LookingForNewRate = lookingForNewRate;
            PolicyMaximum = policyMaximum;
            InsuranceType = insuranceType;
        }
    }

                /*{
            "Name": "",
            "Age": ,
            "Occupation": "",
            "Gender": "",
            "PreExisingConditions": ["", ""],
            "IsSmoker": ,
            "MSinceHospital": ,
            "LookingForNewRate": ,
            "PolicyMaximum": ,
            "InsuranceType" "",
            }*/
}