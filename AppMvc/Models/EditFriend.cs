using Services.Interfaces;
using Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AppMvc.Pages;


namespace AppMvc.Models
{
    //Demonstrate how to read Query parameters
    public class EditFriendViewModel
    {
        //Just like for WebApi
        readonly IFriendsService? _service = null;
        readonly ILogger<EditFriendViewModel>? _logger = null;

        public BestFriendIM? FriendIM { get; set; }
        public Guid FriendId  { get; set; }

        public string PageHeader { get; set; } = string.Empty;

        //public member becomes part of the Model in the Razor page
        public string? ErrorMessage { get; set; } = null;

        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }

        public EditFriendViewModel(IFriend friend)
        {
            FriendIM = new BestFriendIM(friend);
            ValidationErrorMsgs = new List<string>();
            InvalidKeys = new List<KeyValuePair<string, ModelStateEntry>>();
        }

        public EditFriendViewModel()
        {
            ValidationErrorMsgs = new List<string>();
            InvalidKeys = new List<KeyValuePair<string, ModelStateEntry>>();
        }
      
    }
}