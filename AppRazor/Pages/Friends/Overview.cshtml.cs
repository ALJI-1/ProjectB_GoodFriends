using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.Interfaces;
using Services;
using Services.Interfaces;

namespace AppRazor.Pages.Friends
{
    public class OverviewModel(IAddressesService addressesService, IAdminService adminService) : PageModel
    {
        private readonly IAddressesService _addresseServices = addressesService;
        private readonly IAdminService _adminService = adminService;

        public IEnumerable<Models.DTO.GstUsrInfoFriendsDto> CountryInfo;
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
        
       
        public async Task <ActionResult>OnGet()
        {
            var addresses = await _addresseServices.ReadAddressesAsync(true, false, "Denmark", 0, 10);
            var info = await _adminService.GuestInfoAsync();

            CountryInfo = info.Item.Friends.Where(i => i.Country == "Denmark");

            return Page();
            
        }
    }
}
