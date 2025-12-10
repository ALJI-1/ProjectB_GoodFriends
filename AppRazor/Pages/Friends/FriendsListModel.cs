
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Models;
using Services;

using Services.Interfaces;
using Models.Interfaces;

namespace AppRazor.Pages.Friends
{
    public class FriendsListModel : PageModel
    {
        readonly IFriendsService _service = null;
        readonly IAdminService _adminService = null;
        readonly ILogger<FriendsListModel> _logger = null;

        public List<IFriend> Friends { get; set; } = new List<IFriend>();

         //Pagination
        public int NrOfPages { get; set; }
        public int PageSize { get; } = 5;

        public int ThisPageNr { get; set; } = 0;
        public int PrevPageNr { get; set; } = 0;
        public int NextPageNr { get; set; } = 0;
        public int PresentPages { get; set; } = 0;


        public async Task<IActionResult> OnGetAsync(string pagenr)
        {
            if (int.TryParse(pagenr, out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }

            var response = await _service.ReadFriendsAsync(true, true, null, ThisPageNr, PageSize);
            Friends = response.PageItems;
            
            NrOfPages = response.PageCount;
            ThisPageNr = response.PageNr; 
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            PresentPages = NrOfPages;
            
            return Page();
        }

        public FriendsListModel(IFriendsService service, IAdminService adminService, ILogger<FriendsListModel> logger)
        {
            _logger = logger;
            _service = service;
            _adminService = adminService;
        }
    }
}
