using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Models.Common;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Friends
{
    public class FriendsDetailsModel(IFriendsService friendsService, IPetsService petsService, IQuotesService quotesService) : PageModel
    {
        private readonly IFriendsService _friendsService = friendsService;
        private readonly IPetsService _petService = petsService;
        private readonly IQuotesService _quoteService = quotesService;
        
        public IFriend Friend { get; set; }
        public List<FavoritePetIM> PetsIM { get; set; } = new List<FavoritePetIM>();
        public List<FavoriteQuoteIM> QuotesIM { get; set; } = new List<FavoriteQuoteIM>();
        public string ViewType { get; set; } = "pets"; 
        public string ErrorMessage { get; set; } = null;
        
        // Får pets/quotes via asp-route-view i ModelView
        // Lägger skapar inputmodeller av databasmodellerna
        public async Task<IActionResult> OnGet(string id, string view)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid friendId))
                {
                    ErrorMessage = "Invalid friend ID";
                    return Page();
                }

                ViewType = view?.ToLower() ?? "pets";
                
                var response = await _friendsService.ReadFriendAsync(friendId, false);
                Friend = response.Item;

                if (Friend == null)
                {
                    ErrorMessage = "Friend not found";
                    return Page();
                }

                if (ViewType == "pets")
                {
                    PetsIM = Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
                }
                else if (ViewType == "quotes")
                {
                    QuotesIM = Friend.Quotes.Select(p => new FavoriteQuoteIM(p)).ToList();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        // Metod som hämtar rätt objekt från databasen som ska tas bort. Sätter dess status till deleted
        public async Task<IActionResult> OnPostDelete(Guid id, string viewType, Guid friendId)
        {
            ViewType = viewType;
            
            var response = await _friendsService.ReadFriendAsync(friendId, false);
            Friend = response.Item;
            
            if (ViewType == "pets")
            {
                PetsIM = Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
                var petToDelete = PetsIM.FirstOrDefault(q => q.PetId == id);
                if (petToDelete != null)
                {
                    petToDelete.StatusIM = StatusIM.Deleted;
                }
            }
            else
            {
                QuotesIM = Friend.Quotes.Select(q => new FavoriteQuoteIM(q)).ToList();
                var quoteToDelete = QuotesIM.FirstOrDefault(q => q.QuoteId == id);
                if (quoteToDelete != null)
                {
                    quoteToDelete.StatusIM = StatusIM.Deleted;
                }
            }

            // Om någon pet eller quote i listorna har tagits bort så kör den delete på databasobjektet via service 
            var _petsDeletes = PetsIM.FindAll(q => (q.StatusIM == StatusIM.Deleted));
            foreach (var item in _petsDeletes)
            {
                await _petService.DeletePetAsync(item.PetId);
            }

            var _quotesDeletes = QuotesIM.FindAll(q => (q.StatusIM == StatusIM.Deleted));
            foreach (var item in _quotesDeletes)
            {
                await _quoteService.DeleteQuoteAsync(item.QuoteId);
            }

            
            // Poppulerar listerna igen utifrån uppdaterad databas
            if (ViewType == "pets")
            {
                PetsIM = Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
            }
            else
            {
                QuotesIM = Friend.Quotes.Select(q => new FavoriteQuoteIM(q)).ToList();
            }
            
            // Går tillbaka till listan och som också uppdateras
            return RedirectToPage("/Friends/FriendsDetails", new { id = friendId, view = viewType });
        }

       
       // Input Models för quotes och pets 
        public class FavoritePetIM
        {
            public StatusIM StatusIM { get; set; }
            public Guid PetId { get; set; } = Guid.NewGuid();
            public string Name { get; set; }
            public AnimalMood Mood { get; set; }

            public FavoritePetIM() { StatusIM = StatusIM.Unchanged; }

            public FavoritePetIM(FavoritePetIM original)
            {
                StatusIM = original.StatusIM;

                PetId = original.PetId;
                Name = original.Name;
                Mood = original.Mood;
            }
            public FavoritePetIM(IPet pet)
            {
                StatusIM = StatusIM.Unchanged;
                PetId = pet.PetId;
                Name = pet.Name;
                Mood = pet.Mood;
            }
        }

        public class FavoriteQuoteIM
        {
            public StatusIM StatusIM { get; set; }
            public Guid QuoteId { get; set; } = Guid.NewGuid();
            public string Quote { get; set; }
            public string Author { get; set; }


            public FavoriteQuoteIM() { StatusIM = StatusIM.Unchanged; }

            public FavoriteQuoteIM(FavoriteQuoteIM original)
            {
                StatusIM = original.StatusIM;

                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;
            }
            public FavoriteQuoteIM(IQuote quote)
            {
                StatusIM = StatusIM.Unchanged;
                QuoteId = quote.QuoteId;
                Quote = quote.QuoteText;
                Author = quote.Author;
            }
        }
    }
}
