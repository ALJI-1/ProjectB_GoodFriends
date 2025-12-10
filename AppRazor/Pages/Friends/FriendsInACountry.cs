using DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Models.Interfaces;
using Services;
using Services.Interfaces;

namespace AppRazor.Pages.Friends
{
    public class FriendsInACountryModel(IAddressesService addressesService) : PageModel
    {
        private readonly IAddressesService _addresseServices = addressesService;
        
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
        
        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 5;
        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;
        public string Filter { get; set; } = string.Empty;
       
        public async Task <ActionResult> OnGet(string pagenr, string filter)
        {

            Filter = filter ?? "Denmark";
            
            if (int.TryParse(pagenr, out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }

            // Get all addresses (use a large page size to get all)
            var info = await _addresseServices.ReadAddressesAsync(true, false, null, 0, 1000);

            // Filter friends whose address is in the selected country
            var allFriends = info.PageItems
                .SelectMany(a => a.Friends)
                .Where(f => f.Address?.Country == Filter)
                .ToList();

            // Calculate pagination
            NrOfPages = (int)Math.Ceiling(allFriends.Count / (double)PageSize);
            ThisPageNr = Math.Min(ThisPageNr, NrOfPages - 1);
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            PresentPages = NrOfPages;

            // Get friends for current page
            Friends = allFriends
                .Skip(ThisPageNr * PageSize)
                .Take(PageSize)
                .ToList();

            return Page(); 
        }
    }
}