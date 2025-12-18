using Models.Interfaces;

namespace AppMvc.Models
{
    public class FriendsListModel
    {
        public List<IFriend> Friends { get; set; } = new List<IFriend>();

         //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 5;

        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;
    }
}
