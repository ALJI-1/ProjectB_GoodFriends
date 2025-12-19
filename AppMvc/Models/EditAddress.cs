using Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AppMvc.Models
{
    public class EditAddressViewModel
    {
        public FineAddressIM AddressIM { get; set; } = new();
        public Guid AddressId { get; set; }
        public Guid FriendId  { get; set; }

        public string PageHeader { get; set; } = "";
        public string? ErrorMessage { get; set; } 

        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }


        public EditAddressViewModel(IAddress address)
        {
            AddressIM = new FineAddressIM(address);
            ValidationErrorMsgs = new List<string>();
            InvalidKeys = new List<KeyValuePair<string, ModelStateEntry>>();
        }

        public EditAddressViewModel()
        {
            ValidationErrorMsgs = new List<string>();
            InvalidKeys = new List<KeyValuePair<string, ModelStateEntry>>();
        }

    
    }
}