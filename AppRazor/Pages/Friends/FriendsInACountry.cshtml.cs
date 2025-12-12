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
    public enum Countries {Sweden, Norway, Denmark, Finland}
    public class CityInfo
    {
        public string Country { get; set; }
        public string City { get; set; }
        public int NrFriends { get; set; }
        public int NrPets { get; set; }
    }

    public class FriendsInACountryModel(IAdminService adminService, IAddressesService addressesService) : PageModel
    {
        private readonly IAddressesService _addresseServices = addressesService;
        private readonly IAdminService _adminService = adminService;

        [BindProperty]
        public Countries? SelectedCountry { get; set; } = null;
        
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
        public IEnumerable<CityInfo> CityInfoList { get; set; }
        
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
            if (!string.IsNullOrEmpty(filter) && Enum.TryParse<Countries>(filter, true, out var country))
            {
                SelectedCountry = country;
                Filter = country.ToString();
            }
            else
            {
                Filter = filter ?? "";
            }
            
            if (int.TryParse(pagenr, out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }
            var info = await _adminService.GuestInfoAsync();

            var friends = info.Item.Friends.Where(i => i.Country == Filter && !string.IsNullOrEmpty(i.City));
            var pets = info.Item.Pets.Where(i => i.Country == Filter && !string.IsNullOrEmpty(i.City));

            CityInfoList = friends.Select(f => new CityInfo
            {
                Country = f.Country,
                City = f.City,
                NrFriends = f.NrFriends,
                NrPets = pets.FirstOrDefault(p => p.City == f.City && p.Country == f.Country)?.NrPets ?? 0
            });


            // Get all addresses (use a large page size to get all)
            var addressinfo = await _addresseServices.ReadAddressesAsync(true, false, null, 0, 1000);

            // Filter friends whose address is in the selected country
            var allFriends = addressinfo.PageItems
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