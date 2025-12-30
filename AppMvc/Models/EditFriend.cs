using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models.Interfaces;
using AppMvc.Pages;


namespace AppMvc.Models
{
    public class EditFriendViewModel
    {
        public BestFriendIM? FriendIM { get; set; }
        public Guid FriendId  { get; set; }

        public string PageHeader { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; } = null;

        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; } = [];
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; } = [];

        public EditFriendViewModel(IFriend friend)
        {
            FriendIM = new BestFriendIM(friend);
        }

        public EditFriendViewModel()
        {
        }
      
    }
}