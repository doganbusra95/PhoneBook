namespace PhoneBookApi.Models
{
    public class LocationStat
    {
        public string Location { get; set; }
        public int PersonCount { get; set; }
        public int PhoneNumberCount { get; set; }

        public LocationStat()
        {
            Location= string.Empty;
            PersonCount= 0;
            PhoneNumberCount= 0;
        }
    }
}
