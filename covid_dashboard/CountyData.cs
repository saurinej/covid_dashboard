using System;
using System.Collections.Generic;
using System.Text;

namespace covid_dashboard
{
    class CountyData
    {
        //private Dictionary<DateTime, Dictionary<string, int>> dataByDay;
        //private Dictionary<DateTime, Dictionary<string, int>> dayaByOnsetDay;


        //Holds the date and time data was collected as the key, and the collected data in a list as the value
        private Dictionary<DateTime, LinkedList<DataLine>> dataByDay;
        //Represents the name of the county the data corresponds to
        public string CountyName { get; private set; }
        //constructor
        public CountyData(string countyName)
        {
            this.CountyName = countyName;
            dataByDay = new Dictionary<DateTime, LinkedList<DataLine>>(new DateTimeComparer());
        }
        //add a single line of data for a specific date
        public void add(DateTime date, DataLine data)
        {
            if (dataByDay.ContainsKey(date))
            {
                if (dataByDay[date] != null) 
                {
                    dataByDay[date].AddLast(data);
                }
            }
            else
            {
                LinkedList<DataLine> newList = new LinkedList<DataLine>();
                newList.AddLast(data);
                dataByDay.Add(date, newList);
            }
        }
        //adds all the data for a specific date
        public void add(DateTime date, LinkedList<DataLine> list)
        {
            if (dataByDay.ContainsKey(date))
            {
                dataByDay[date] = list;
            } 
            else
            {
                dataByDay.Add(date, list);
            }
        }

        /// <summary>
        /// Gets case, hospitalized, and death count totals for all categories and each individual sex and age category for the latest date.
        /// </summary>
        /// <returns>Dictionary of string keys and integer values representing counts for corresponding county. Null if no data is present.</returns>
        public Dictionary<string, int> getCounts()
        {
            //return null if no data exists
            if (dataByDay.Count == 0) return null;
            //find the latest date in list
            DateTime latestDate = DateTime.Now;
            while (!dataByDay.ContainsKey(latestDate))
            {
                latestDate = latestDate.AddDays(-1);
            }
            //get list of DataLine objects for the latest date present in county data
            LinkedList<DataLine> dataList = dataByDay[latestDate];
            //initialize count variables
            int caseCount = 0, hospCount = 0, deathCount = 0, maleSexCaseCount = 0, femaleSexCaseCount = 0,
                unknownSexCaseCount = 0, age0To19CaseCount = 0, age20To29CaseCount = 0, age30To39CaseCount = 0, age40To49CaseCount = 0,
                age50To59CaseCount = 0, age60To69CaseCount = 0, age70To79CaseCount = 0, age80PlusCaseCount = 0, unknownAgeCaseCount = 0;
            int maleSexHospCount = 0, femaleSexHospCount = 0, unknownSexHospCount = 0, age0To19HospCount = 0, age20To29HospCount = 0,
                age30To39HospCount = 0, age40To49HospCount = 0, age50To59HospCount = 0, age60To69HospCount = 0, age70To79HospCount = 0,
                age80PlusHospCount = 0, unknownAgeHospCount = 0;
            int maleSexDeathCount = 0, femaleSexDeathCount = 0, unknownSexDeathCount = 0, age0To19DeathCount = 0, age20To29DeathCount = 0,
                age30To39DeathCount = 0, age40To49DeathCount = 0, age50To59DeathCount = 0, age60To69DeathCount = 0, age70To79DeathCount = 0,
                age80PlusDeathCount = 0, unknownAgeDeathCount = 0;
            //iterate through data and add to counts
            foreach (DataLine dL in dataList)
            {
                caseCount += dL.CaseCount;
                hospCount += dL.HospCount;
                deathCount += dL.DeathCount;
                //sex
                if (dL.Sex.Equals("Male"))
                {
                    maleSexCaseCount += dL.CaseCount;
                    maleSexHospCount += dL.HospCount;
                    maleSexDeathCount += dL.DeathCount;
                }
                else if (dL.Sex.Equals("Female"))
                {
                    femaleSexCaseCount += dL.CaseCount;
                    femaleSexHospCount += dL.HospCount;
                    femaleSexDeathCount += dL.DeathCount;
                }
                else
                {
                    unknownSexCaseCount += dL.CaseCount;
                    unknownSexHospCount += dL.HospCount;
                    unknownSexDeathCount += dL.DeathCount;

                }
                //age ranges
                if (dL.AgeRange.Equals("0-19"))
                {
                    age0To19CaseCount += dL.CaseCount;
                    age0To19HospCount += dL.HospCount;
                    age0To19DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("20-29"))
                {
                    age20To29CaseCount += dL.CaseCount;
                    age20To29HospCount += dL.HospCount;
                    age20To29DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("30-39"))
                {
                    age30To39CaseCount += dL.CaseCount;
                    age30To39HospCount += dL.HospCount;
                    age30To39DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("40-49"))
                {
                    age40To49CaseCount += dL.CaseCount;
                    age40To49HospCount += dL.HospCount;
                    age40To49DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("50-59"))
                {
                    age50To59CaseCount += dL.CaseCount;
                    age50To59HospCount += dL.HospCount;
                    age50To59DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("60-69"))
                {
                    age60To69CaseCount += dL.CaseCount;
                    age60To69HospCount += dL.HospCount;
                    age60To69DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("70-79"))
                {
                    age70To79CaseCount += dL.CaseCount;
                    age70To79HospCount += dL.HospCount;
                    age70To79DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("80+"))
                {
                    age80PlusCaseCount += dL.CaseCount;
                    age80PlusHospCount += dL.HospCount;
                    age80PlusDeathCount += dL.DeathCount;
                }
                else
                {
                    unknownAgeCaseCount += dL.CaseCount;
                    unknownAgeHospCount += dL.HospCount;
                    unknownAgeDeathCount += dL.DeathCount;
                }
            }
            //create and return list holding the counts
            Dictionary<string, int> counts = new Dictionary<string, int>()
            {
                { "TotalCaseCount", caseCount },
                { "TotalHospCount", hospCount },
                { "TotalDeathCount", deathCount },
                { "MaleCaseCount", maleSexCaseCount },
                { "MaleHospCount", maleSexHospCount },
                { "MaleDeathCount", maleSexDeathCount },
                { "FemaleCaseCount", femaleSexCaseCount },
                { "FemaleHospCount", femaleSexHospCount },
                { "FemaleDeathCount", femaleSexDeathCount },
                { "UnknownSexCaseCount", unknownSexCaseCount },
                { "UnknownSexHospCount", unknownSexHospCount },
                { "UnknownSexDeathCount", unknownSexDeathCount },
                { "0-19CaseCount", age0To19CaseCount },
                { "0-19HospCount", age0To19HospCount },
                { "0-19DeathCount", age0To19DeathCount },
                { "20-29CaseCount", age20To29CaseCount },
                { "20-29HospCount", age20To29HospCount },
                { "20-29DeathCount", age20To29DeathCount },
                { "30-39CaseCount", age30To39CaseCount },
                { "30-39HospCount", age30To39HospCount },
                { "30-39DeathCount", age30To39DeathCount },
                { "40-49CaseCount", age40To49CaseCount },
                { "40-49HospCount", age40To49HospCount },
                { "40-49DeathCount", age40To49DeathCount },
                { "50-59CaseCount", age50To59CaseCount },
                { "50-59HospCount", age50To59HospCount },
                { "50-59DeathCount", age50To59DeathCount },
                { "60-69CaseCount", age60To69CaseCount },
                { "60-69HospCount", age60To69HospCount },
                { "60-69DeathCount", age60To69DeathCount },
                { "70-79CaseCount", age70To79CaseCount },
                { "70-79HospCount", age70To79HospCount },
                { "70-79DeathCount", age70To79DeathCount },
                { "80+CaseCount", age80PlusCaseCount },
                { "80+HospCount", age80PlusHospCount },
                { "80+DeathCount", age80PlusDeathCount },
                { "UnknownAgeCaseCount", unknownAgeCaseCount },
                { "UnknownAgeHospCount", unknownAgeHospCount },
                { "UnknownAgeDeathCount", unknownAgeDeathCount }
            };
            return counts;
        }

        /// <summary>
        /// Gets case, hospitalized, and death count totals for all categories and each individual sex and age category for the given date.
        /// </summary>
        /// <param name="date">Snapshot of data on given date</param>
        /// <returns>Dictionary of string keys and integer values representing counts for corresponding county on the date given. Null if no data present for given day.</returns>
        public Dictionary<string, int> getCounts(DateTime date)
        {
            //add another day on the date since date may have default hour of zero
            if (date.Hour < 14) date = date.AddDays(1);
            //return null if no data exists for a given day
            if (!dataByDay.ContainsKey(date) || dataByDay.Count == 0) return null;
            //get list of DataLine objects for the required date
            LinkedList<DataLine> dataList = dataByDay[date];
            //initialize count variables
            int caseCount = 0, hospCount = 0, deathCount = 0, maleSexCaseCount = 0, femaleSexCaseCount = 0,
                unknownSexCaseCount = 0, age0To19CaseCount = 0, age20To29CaseCount = 0, age30To39CaseCount = 0, age40To49CaseCount = 0,
                age50To59CaseCount = 0, age60To69CaseCount = 0, age70To79CaseCount = 0, age80PlusCaseCount = 0, unknownAgeCaseCount = 0;
            int maleSexHospCount = 0, femaleSexHospCount = 0, unknownSexHospCount = 0, age0To19HospCount = 0, age20To29HospCount = 0,
                age30To39HospCount = 0, age40To49HospCount = 0, age50To59HospCount = 0, age60To69HospCount = 0, age70To79HospCount = 0,
                age80PlusHospCount = 0, unknownAgeHospCount = 0;
            int maleSexDeathCount = 0, femaleSexDeathCount = 0, unknownSexDeathCount = 0, age0To19DeathCount = 0, age20To29DeathCount = 0,
                age30To39DeathCount = 0, age40To49DeathCount = 0, age50To59DeathCount = 0, age60To69DeathCount = 0, age70To79DeathCount = 0,
                age80PlusDeathCount = 0, unknownAgeDeathCount = 0;
            //iterate through data and add to counts
            foreach (DataLine dL in dataList)
            {
                caseCount += dL.CaseCount;
                hospCount += dL.HospCount;
                deathCount += dL.DeathCount;
                //sex
                if (dL.Sex.Equals("Male"))
                {
                    maleSexCaseCount += dL.CaseCount;
                    maleSexHospCount += dL.HospCount;
                    maleSexDeathCount += dL.DeathCount;
                }
                else if (dL.Sex.Equals("Female"))
                {
                    femaleSexCaseCount += dL.CaseCount;
                    femaleSexHospCount += dL.HospCount;
                    femaleSexDeathCount += dL.DeathCount;
                }
                else
                {
                    unknownSexCaseCount += dL.CaseCount;
                    unknownSexHospCount += dL.HospCount;
                    unknownSexDeathCount += dL.DeathCount;
                }
                //age ranges
                if (dL.AgeRange.Equals("0-19"))
                {
                    age0To19CaseCount += dL.CaseCount;
                    age0To19HospCount += dL.HospCount;
                    age0To19DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("20-29"))
                {
                    age20To29CaseCount += dL.CaseCount;
                    age20To29HospCount += dL.HospCount;
                    age20To29DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("30-39"))
                {
                    age30To39CaseCount += dL.CaseCount;
                    age30To39HospCount += dL.HospCount;
                    age30To39DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("40-49"))
                {
                    age40To49CaseCount += dL.CaseCount;
                    age40To49HospCount += dL.HospCount;
                    age40To49DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("50-59"))
                {
                    age50To59CaseCount += dL.CaseCount;
                    age50To59HospCount += dL.HospCount;
                    age50To59DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("60-69"))
                {
                    age60To69CaseCount += dL.CaseCount;
                    age60To69HospCount += dL.HospCount;
                    age60To69DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("70-79"))
                {
                    age70To79CaseCount += dL.CaseCount;
                    age70To79HospCount += dL.HospCount;
                    age70To79DeathCount += dL.DeathCount;
                }
                else if (dL.AgeRange.Equals("80+"))
                {
                    age80PlusCaseCount += dL.CaseCount;
                    age80PlusHospCount += dL.HospCount;
                    age80PlusDeathCount += dL.DeathCount;
                }
                else
                {
                    unknownAgeCaseCount += dL.CaseCount;
                    unknownAgeHospCount += dL.HospCount;
                    unknownAgeDeathCount += dL.DeathCount;
                }
            }
            //create and return dictionary holding the counts
            Dictionary<string, int> counts = new Dictionary<string, int>()
            {
                { "TotalCaseCount", caseCount },
                { "TotalHospCount", hospCount },
                { "TotalDeathCount", deathCount },
                { "MaleCaseCount", maleSexCaseCount },
                { "MaleHospCount", maleSexHospCount },
                { "MaleDeathCount", maleSexDeathCount },
                { "FemaleCaseCount", femaleSexCaseCount },
                { "FemaleHospCount", femaleSexHospCount },
                { "FemaleDeathCount", femaleSexDeathCount },
                { "UnknownSexCaseCount", unknownSexCaseCount },
                { "UnknownSexHospCount", unknownSexHospCount },
                { "UnknownSexDeathCount", unknownSexDeathCount },
                { "0-19CaseCount", age0To19CaseCount },
                { "0-19HospCount", age0To19HospCount },
                { "0-19DeathCount", age0To19DeathCount },
                { "20-29CaseCount", age20To29CaseCount },
                { "20-29HospCount", age20To29HospCount },
                { "20-29DeathCount", age20To29DeathCount },
                { "30-39CaseCount", age30To39CaseCount },
                { "30-39HospCount", age30To39HospCount },
                { "30-39DeathCount", age30To39DeathCount },
                { "40-49CaseCount", age40To49CaseCount },
                { "40-49HospCount", age40To49HospCount },
                { "40-49DeathCount", age40To49DeathCount },
                { "50-59CaseCount", age50To59CaseCount },
                { "50-59HospCount", age50To59HospCount },
                { "50-59DeathCount", age50To59DeathCount },
                { "60-69CaseCount", age60To69CaseCount },
                { "60-69HospCount", age60To69HospCount },
                { "60-69DeathCount", age60To69DeathCount },
                { "70-79CaseCount", age70To79CaseCount },
                { "70-79HospCount", age70To79HospCount },
                { "70-79DeathCount", age70To79DeathCount },
                { "80+CaseCount", age80PlusCaseCount },
                { "80+HospCount", age80PlusHospCount },
                { "80+DeathCount", age80PlusDeathCount },
                { "UnknownAgeCaseCount", unknownAgeCaseCount },
                { "UnknownAgeHospCount", unknownAgeHospCount },
                { "UnknownAgeDeathCount", unknownAgeDeathCount }
            };
            return counts;
        }

        /// <summary>
        /// Gets counts from data collected each day. 
        /// </summary>
        /// <param name="startDate">No date before 06Apr2020.</param>
        /// <param name="endDate">Last date data is required for.</param>
        /// <returns>Dictionary with historical data date as the keys and a dictionary with categories as keys and count values as the value.</returns>
        public Dictionary<DateTime, Dictionary<string, int>> getCountsOverTime(DateTime startDate, DateTime endDate)
        {
            //assume edge dates are for the specific day, meaning hour must be after 1400
            if (startDate.Hour < 14) startDate = startDate.AddDays(1);
            if (endDate.Hour < 14) endDate = endDate.AddDays(1);
            //return null if there is no data
            if (dataByDay.Count == 0) return null;
            //return null if startDate is earlier than when historical data began collection
            if (DateTime.Compare(startDate, new DateTime(2020, 4, 6)) < 0) return null;
            //get counts for dates between and including startDate and endDate
            Dictionary<DateTime, Dictionary<string, int>> historicalData = new Dictionary<DateTime, Dictionary<string, int>>();
            DateTime iterateDate = startDate;
            while (DateTime.Compare(iterateDate, endDate) <= 0)
            {
                if (dataByDay.ContainsKey(iterateDate))
                {
                    historicalData.Add(iterateDate, getCounts(iterateDate));
                }
                else
                {
                    historicalData.Add(iterateDate, null);
                }
            }

            return historicalData;
        }

    }
}
