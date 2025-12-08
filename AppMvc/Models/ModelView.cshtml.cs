
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Models;
using Models.Interfaces;
using Services;
using Services.Interfaces;

namespace AppMvc.Models
{
    public class ModelViewModel(IFriendsService friendsService) : PageModel
    {
        readonly IFriendsService _friendService = friendsService;
        
        public IFriend? Friend { get; set; }
        public string? ErrorMessage { get; set; }

        //Will execute on a Get request
        public async Task <ActionResult> OnGet(string id)
        {
            try
            {
                Guid _id = Guid.Parse(id);
                //Read a QueryParameter
                //Guid _id = Guid.Parse(Request.Query["id"]);

                //Use the Service
                var response = await _friendService.ReadFriendAsync(_id, false);
                Friend = response.Item;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
        }
        
    }
}
