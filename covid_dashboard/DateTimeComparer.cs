using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace covid_dashboard
{
    class DateTimeComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime date1, DateTime date2)
        {
            //if both objects are null, they are equal
            if (date1 == null && date1 == null) return true;
            //if only one object is null, they are not equal
            else if (date1 == null || date2 == null) return false;
            //if hour of either is before 1400, subtract one day
            if (date1.Hour < 14) date1 = date1.AddDays(-1);
            if (date2.Hour < 14) date2 = date2.AddDays(-1);
            //return true if day, month and year are equal after adjusting day for time collected. Return false if not
            if (date1.Day == date2.Day && date1.Month == date2.Month && date1.Year == date2.Year)
            {
                return true;
            } 
            else
            {
                return false;
            }

        }

        public int GetHashCode(DateTime obj)
        {
            if (obj.Hour < 14) obj = obj.AddDays(-1);
            long hCode = obj.Year ^ obj.Day * obj.Month;
            return hCode.GetHashCode();
        }

    }
}
