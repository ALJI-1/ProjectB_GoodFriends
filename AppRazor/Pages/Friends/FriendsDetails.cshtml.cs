using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Interfaces;
using Services.Interfaces;

namespace AppRazor.Pages.Friends
{
    public class FriendsDetailsModel(IFriendsService friendsService) : PageModel
    {
        private readonly IFriendsService _friendsService = friendsService;
        
        public IFriend Friend { get; set; }
        public List<IPet> Pets { get; set; } = new List<IPet>();
        public List<IQuote> Quotes { get; set; } = new List<IQuote>();
        public string ViewType { get; set; } = "pets"; 
        public string ErrorMessage { get; set; } = null;
        
        public async Task<IActionResult> OnGet(string id, string view)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid friendId))
                {
                    ErrorMessage = "Invalid friend ID";
                    return Page();
                }

                ViewType = view?.ToLower() ?? "pets";
                
                var response = await _friendsService.ReadFriendAsync(friendId, false);
                Friend = response.Item;

                if (Friend == null)
                {
                    ErrorMessage = "Friend not found";
                    return Page();
                }

                if (ViewType == "pets")
                {
                    Pets = Friend.Pets?.ToList() ?? new List<IPet>();
                }
                else if (ViewType == "quotes")
                {
                    Quotes = Friend.Quotes?.ToList() ?? new List<IQuote>();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
