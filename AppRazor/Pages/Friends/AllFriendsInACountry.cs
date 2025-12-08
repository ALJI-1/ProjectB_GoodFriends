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
    public class AllFriendsInACountryModel(IAddressesService addressesService) : PageModel
    {
        private readonly IAddressesService _addresseServices = addressesService;
        
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
        
       
        public async Task <ActionResult> OnGet()
        {
            var info = await _addresseServices.ReadAddressesAsync(true, false, null, 0, 10);

            Friends = info.PageItems.SelectMany(a => a.Friends).Where(c => c.Address.Country == "Sweden").ToList();

            return Page(); 
        }
    }
}