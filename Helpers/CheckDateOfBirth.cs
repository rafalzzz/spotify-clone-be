namespace SpotifyAPI.Helpers
{
    public class CheckDateOfBirth
    {
        public static bool BeAValidDate(string dateOfBirth)
        {
            return DateTime.TryParse(dateOfBirth, out _);
        }

        public static bool BeLessThan100YearsOld(string dateOfBirth)
        {
            if (!DateTime.TryParse(dateOfBirth, out DateTime dateTime)) return false;

            int diff = DateTime.Now.Year - dateTime.Year;
            if (dateTime > DateTime.Now.AddYears(-diff)) diff--;

            return diff <= 100;
        }

        public static bool BeLessThanOrEqualToToday(string dateOfBirth)
        {
            if (!DateTime.TryParse(dateOfBirth, out DateTime dateTime)) return false;
            return dateTime <= DateTime.Now;
        }
    }
}