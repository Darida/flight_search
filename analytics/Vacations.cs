using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace analytics {
    public class Vacations {   
        public static ISet<DateTime> HOLIDAYS = new HashSet<DateTime>() { new DateTime(2019, 7, 4), new DateTime(2019, 7, 5) };
        public static int calculate(DateTime from, DateTime to) {
            int vacations = 0;
            for(DateTime day = from.Date; day <= to; day = day.Add(TimeSpan.FromDays(1))) {
                if(day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if(HOLIDAYS.Contains(day))
                    continue;

                vacations++;
            }
            return vacations;
        }
    }
}