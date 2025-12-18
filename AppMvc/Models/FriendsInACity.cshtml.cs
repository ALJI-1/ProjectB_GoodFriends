using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Friends
{
    public class FriendsInACityModel(IAddressesService addressesService) : PageModel
    {
        private readonly IAddressesService _addresseServices = addressesService;
        
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
        public List<IPet> Pets {get; set;} = new List<IPet>();
        
        //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 10;
        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
       
        public async Task <ActionResult> OnGet(string pagenr, string city, string country)
        {
            City = city ?? "";
            Country = country ?? "";
            
            if (int.TryParse(pagenr, out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }

            // Get all addresses (use a large page size to get all)
            var info = await _addresseServices.ReadAddressesAsync(true, false, null, 0, 1000);

            // Filter friends whose address is in the selected city and country
            var allFriends = info.PageItems
                .Where(a => a.City == City && a.Country == Country)
                .SelectMany(a => a.Friends)
                .ToList();

            // Get all pets for those friends
            Pets = allFriends
                .SelectMany(f => f.Pets ?? new List<IPet>())
                .ToList();

            // Calculate pagination
            NrOfPages = (int)Math.Ceiling(allFriends.Count / (double)PageSize);
            ThisPageNr = Math.Min(ThisPageNr, Math.Max(0, NrOfPages - 1));
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(Math.Max(0, NrOfPages - 1), ThisPageNr + 1);
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
