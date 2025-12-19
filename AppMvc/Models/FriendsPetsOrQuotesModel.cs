using Models.Interfaces;

namespace AppMvc.Models
{
    public class FriendsPetsOrQuotesViewModel() 
    {
        public IFriend? Friend { get; set; }
        public List<IPet> Pets { get; set; } = [];
        public List<IQuote> Quotes { get; set; } = [];
        public List<FavoritePetIM> PetsIM { get; set; } = [];
        public List<FavoriteQuoteIM> QuotesIM { get; set; } = [];
        public string ViewType { get; set; } = "pets"; 
        public string ErrorMessage { get; set; } = null;  
    }
}
