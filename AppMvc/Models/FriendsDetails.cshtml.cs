using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Models.Interfaces;
using Services.Interfaces;
using Models.Common;
using AppMvc.Models;

namespace AppRazor.Pages.Friends
{
    public class FriendsDetailsModel(IFriendsService friendsService, IPetsService petsService, IQuotesService quotesService) : PageModel
    {
        private readonly IFriendsService _friendsService = friendsService;
        private readonly IPetsService _petService = petsService;
        private readonly IQuotesService _quoteService = quotesService;
        
        public IFriend? Friend { get; set; }
        public List<FavoritePetIM> PetsIM { get; set; } = [];
        public List<FavoriteQuoteIM> QuotesIM { get; set; } = [];
        public string ViewType { get; set; } = "pets"; 
        public string ErrorMessage { get; set; } = null;  
    }
}
