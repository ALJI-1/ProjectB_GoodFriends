using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Models;
using Services;
using System.Security.Cryptography;
using Services.Interfaces;
using Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Models.Common;


namespace AppRazor.Pages
{
    //Demonstrate how to read Query parameters
    public class EditFriendModel : PageModel
    {
        //Just like for WebApi
        readonly IFriendsService _service = null;
        readonly ILogger<EditFriendModel> _logger = null;

        [BindProperty]
        public BestFriendIM FriendIM { get; set; }

        public string PageHeader { get; set; }

        //public member becomes part of the Model in the Razor page
        public string ErrorMessage { get; set; } = null;

        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }


        //Will execute on a Get request
        public async Task<IActionResult> OnGet()
        {
            try
            {
                if (Guid.TryParse(Request.Query["id"], out Guid _id))
                {
                    //Use the Service and populate the InputModel
                    var response = await _service.ReadFriendAsync(_id, false);
                    FriendIM = new BestFriendIM(response.Item);
                    PageHeader = "Edit details of a friend";
                }
                else
                {
                    //Create an empty InputModel
                    FriendIM = new BestFriendIM();
                    FriendIM.StatusIM = StatusIM.Inserted;
                    PageHeader = "Create a new friend";
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUndo()
        {
            //Use the Service and populate the InputModel
            var response = await _service.ReadFriendAsync(FriendIM.FriendId, false);
            FriendIM = new BestFriendIM(response.Item);
            PageHeader = "Edit details of a friend";
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            //PageHeader is stored in TempData which has to be set after a Post
            PageHeader = (FriendIM.StatusIM == StatusIM.Inserted) ?
                "Create a new friend" : "Edit details of a friend";

            if (!IsValid())
            {
                //The page is not valid
                return Page();
            }
            if (FriendIM.StatusIM == StatusIM.Inserted)
            {
                //It is an create
                var dto = FriendIM.ToDto();
                var response = await _service.CreateFriendAsync(dto);

                FriendIM = new BestFriendIM(response.Item);
            }
            else
            {
                //It is an update
                //update the changes and save
                var dto = FriendIM.ToDto();
                var updateResponse = await _service.UpdateFriendAsync(dto);
                
                FriendIM = new BestFriendIM(updateResponse.Item);
            }

            PageHeader = "Edit details of a friend";
            return Redirect("FriendsList");
        }


        //Inject services just like in WebApi
        public EditFriendModel(IFriendsService service, ILogger<EditFriendModel> logger)
        {
            _logger = logger;
            _service = service;
        }

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation

        public class BestFriendIM
        {
            //Status of InputModel
            public StatusIM StatusIM { get; set; }

            //Properties from Model which is to be edited in the <form>
            public Guid FriendId { get; init; } = Guid.NewGuid();
            
            [Required(ErrorMessage = "You must provide a first name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "You must provide a last name")]
            public string LastName { get; set; }
            
            [Required(ErrorMessage = "You must provide an email")]
            public string Email { get; set; }


            #region constructors and model update
            public BestFriendIM() { StatusIM = StatusIM.Unchanged; }

            //Copy constructor
            public BestFriendIM(BestFriendIM original)
            {
                StatusIM = original.StatusIM;
                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;       
                Email = original.Email;
            }

            public BestFriendIM(IFriend original)
            {
                StatusIM = StatusIM.Unchanged;
                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;       
                Email = original.Email;
            }

            public FriendCuDto ToDto()
            {
                return new FriendCuDto
                {
                    FriendId = FriendId,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email
                };
            }
            #endregion
        }
        #endregion
        private bool IsValid(string[] validateOnlyKeys = null)
        {
            InvalidKeys = ModelState
               .Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

            if (validateOnlyKeys != null)
            {
                InvalidKeys = InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
            }

            ValidationErrorMsgs = InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            HasValidationErrors = InvalidKeys.Any();

            return !HasValidationErrors;
        }
    }
}