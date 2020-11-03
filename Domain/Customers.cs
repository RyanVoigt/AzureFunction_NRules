
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
        public bool LookingForNewRate { get; set; }
        public int PolicyMaximum { get; set; }
        public int InsuranceType { get; set; }
        // Universal, Participating, Term
    
        public Customer(string name, int age, string occupation, string gender, string[] preExistingCondition, bool isSmoker, int mSinceHospital, bool lookingForNewRate, int policyMaximum)
        {
            Name = name;
            Age = age;
            Occupation = occupation;
            Gender = gender;
            PreExisingConditions = preExistingCondition;
            IsSmoker = isSmoker;
            MSinceHospital = mSinceHospital;
            LookingForNewRate = lookingForNewRate;
            PolicyMaximum = policyMaximum;
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
            }*/
}