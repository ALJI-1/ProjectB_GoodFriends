using Models.Interfaces;

namespace AppMvc.Models
{
    public enum Countries {Sweden, Norway, Denmark, Finland}
    public class CityInfo
    {
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int NrFriends { get; set; }
        public int NrPets { get; set; }
    }

    public class FriendsInACountryViewModel()
    {
        public Countries? SelectedCountry { get; set; } = null;
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
        public IEnumerable<CityInfo> CityInfoList { get; set; } = new List<CityInfo>();
        
        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 5;
        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;
        public string Filter { get; set; } = string.Empty;
       
        
    }
}