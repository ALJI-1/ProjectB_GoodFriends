using System.IO.Compression;
using DbRepos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Models.Interfaces;
using Services;
using Services.Interfaces;

namespace AppRazor.Pages.Friends
{
    public class CityInfo
    {
        public string Country { get; set; }
        public string City { get; set; }
        public int NrFriends { get; set; }
        public int NrPets { get; set; }
    }

    public class FriendsPetsOfACityInACountryModel(IAdminService adminService, IAddressesService addressesService) : PageModel
    {
        private readonly IAdminService _adminService = adminService;
        public IEnumerable<CityInfo> CityInfoList { get; set; }


        public async Task <ActionResult>OnGet()
        {
            var info = await _adminService.GuestInfoAsync();

            var friends = info.Item.Friends.Where(i => i.Country == "Denmark");
            var pets = info.Item.Pets.Where(i => i.Country == "Denmark");

            CityInfoList = friends.Select(f => new CityInfo
            {
                Country = f.Country,
                City = f.City,
                NrFriends = f.NrFriends,
                NrPets = pets.FirstOrDefault(p => p.City == f.City && p.Country == f.Country)?.NrPets ?? 0
            });

            return Page();
            
        }
    }
}
