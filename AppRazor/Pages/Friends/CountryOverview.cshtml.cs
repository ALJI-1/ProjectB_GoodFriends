using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.Interfaces;
using Services;
using Services.Interfaces;
using Models.DTO;

namespace AppRazor.Pages.Friends
{
    public class CountryOverviewModel(IAdminService adminService) : PageModel
    {
        private readonly IAdminService _adminService = adminService;
        public IEnumerable<Models.DTO.GstUsrInfoFriendsDto> CountryInfo = [];
       
        public async Task <ActionResult>OnGet()
        {
            var info = await _adminService.GuestInfoAsync();

            CountryInfo = info.Item.Friends
                .Where(i => !string.IsNullOrEmpty(i.Country))
                .GroupBy(i => i.Country)
                .Select(g => new GstUsrInfoFriendsDto 
                { 
                    Country = g.Key, 
                    NrFriends = g.Sum(x => x.NrFriends),
                    City = null
                })
                .OrderBy(i => i.Country)
                .ToList();

            return Page();
            
        }
    }
}
