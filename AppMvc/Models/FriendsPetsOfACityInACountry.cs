namespace AppMvc.Models
{
    public class CityOverviewModel
    {
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int NrFriends { get; set; }
        public int NrPets { get; set; }
        public List<CityOverviewModel> CityInfoList { get; set; } = new List<CityOverviewModel>();
    }
}
