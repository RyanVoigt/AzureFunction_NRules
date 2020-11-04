
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NRules;
using NRules.Fluent.Dsl;
using Manulife.Rules;

namespace Manulife.Domain
{
 public class Customer{
        public string Name { get; private set; }
        public int Age { get; set; }
        public string Occupation { get; set; }
        public string Gender { get; set; }
        public string[] PreExisingConditions { get; set; }
        public bool IsSmoker { get; set; }
        public int MSinceHospital { get; set; }
        public bool EligibleForNewRate { get; set; }
        public int PolicyMaximum { get; set; }
        public string InsuranceType { get; set; }
        public int ID { get; set; }
  
        // Universal, Participating, Term
    
        public Customer(string name, int age, string occupation, string gender, string[] preExistingCondition, bool isSmoker, int mSinceHospital, bool eligibleForNewRate, int policyMaximum, string insuranceType, int iD)
        {
            Name = name;
            Age = age;
            Occupation = occupation;
            Gender = gender;
            PreExisingConditions = preExistingCondition;
            IsSmoker = isSmoker;
            MSinceHospital = mSinceHospital;
            EligibleForNewRate = eligibleForNewRate;
            PolicyMaximum = policyMaximum;
            InsuranceType = insuranceType;
            ID = iD;
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
            "EligibleForNewRate": ,
            "PolicyMaximum": ,
            "InsuranceType" "",
            "ID": 
            }*/
}