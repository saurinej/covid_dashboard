using System;
using System.Collections.Generic;
using System.Text;

namespace covid_dashboard
{
    class DataLine
    {
        public string Sex { get; private set; }
        public string AgeRange { get; private set; }
        public DateTime OnsetDate { get; private set; }
        public int CaseCount { get; private set; }
        public int HospCount { get; private set; }
        public int DeathCount { get; private set; }

        public DataLine(string sex, string ageRange, string onsetDate, int caseCount, int hospCount, int deathCount)
        {
            this.Sex = sex;
            this.AgeRange = ageRange;
            this.OnsetDate = DateTime.ParseExact(onsetDate, "M/d/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
            this.CaseCount = caseCount;
            this.HospCount = hospCount;
            this.DeathCount = deathCount;
        }

        //Dictionary<string, int> storage 

    }
}
