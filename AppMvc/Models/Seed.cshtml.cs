using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace AppMvc.Models
{
    public class SeedViewModel : PageModel
    {
        //Just like for WebApi
        readonly IAdminService _admin_service;
        readonly ILogger<SeedViewModel> _logger;

        public int NrOfGroups => nrOfFriends().Result;
        private async Task<int> nrOfFriends()
        {
            var info = await _admin_service.GuestInfoAsync();
            return info.Item.Db.NrSeededFriends + info.Item.Db.NrUnseededFriends;
        }

        [BindProperty]
        [Required (ErrorMessage = "You must enter nr of items to seed")]
        public int NrOfItemsToSeed { get; set; } = 100;

        [BindProperty]
        public bool RemoveSeeds { get; set; } = true;

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (RemoveSeeds)
                {
                    await _admin_service.RemoveSeedAsync(true);
                    await _admin_service.RemoveSeedAsync(false);
                }
                await _admin_service.SeedAsync(NrOfItemsToSeed);

                return RedirectToPage("/Friends/FriendsList");
            }
            return Page();
        }

        //Inject services just like in WebApi
        public SeedViewModel(IAdminService admin_service, ILogger<SeedViewModel> logger)
        {
            _admin_service = admin_service;
            _logger = logger;
        }
    }
}
