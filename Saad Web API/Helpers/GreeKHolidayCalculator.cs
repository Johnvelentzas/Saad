namespace Saad_Web_API.Helpers
{
    public static class GreekHolidayCalculator
    {
        public static HashSet<DateTime> GetHolidaysForYear(int year)
        {
            var holidays = new HashSet<DateTime>
            {
                new DateTime(year, 1, 1),   // New Year's Day
                new DateTime(year, 1, 6),   // Epiphany (Theofania)
                new DateTime(year, 3, 25),  // Independence Day
                new DateTime(year, 5, 1),   // Labour Day (Protomagia)
                new DateTime(year, 8, 15),  // Assumption of Mary (Dekapentavgoustos)
                new DateTime(year, 10, 28), // Ochi Day
                new DateTime(year, 12, 25), // Christmas Day
                new DateTime(year, 12, 26)  // Synaxis of the Mother of God
            };

            // Calculate Orthodox Easter mathematically
            DateTime orthodoxEaster = CalculateOrthodoxEaster(year);

            // Add the floating, Easter-dependent holidays
            holidays.Add(orthodoxEaster.AddDays(-48)); // Clean Monday (Kathara Deftera)
            holidays.Add(orthodoxEaster.AddDays(-2));  // Good Friday (Megali Paraskevi)
            holidays.Add(orthodoxEaster.AddDays(1));   // Easter Monday
            holidays.Add(orthodoxEaster.AddDays(50));  // Holy Spirit Monday (Agiou Pnevmatos)*

            return holidays;
        }

        private static DateTime CalculateOrthodoxEaster(int year)
        {
            // Julian Calendar math for Orthodox Easter
            int a = year % 19;
            int b = year % 7;
            int c = year % 4;

            int d = (19 * a + 16) % 30;
            int e = (2 * c + 4 * b + 6 * d) % 7;

            DateTime julianEaster = new DateTime(year, 3, 21).AddDays(d + e);

            // Add 13 days to convert Julian to Gregorian (valid for years 1900-2099)
            return julianEaster.AddDays(13);
        }
    }
}