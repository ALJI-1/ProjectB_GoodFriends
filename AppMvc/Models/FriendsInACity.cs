using Models.Interfaces;

namespace AppMvc.Models
{
    public class FriendsInACityViewModel()
    {
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
        public List<IPet> Pets {get; set;} = new List<IPet>();

        public int NrOfPages { get; set; }
        public int PageSize { get; } = 10;
        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
