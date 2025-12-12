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


namespace AppRazor.Pages
{
    //Demonstrate how to read Query parameters
    public class EditAddressModel(
        IFriendsService service,
        IAddressesService addressesService,
        ILogger<EditAddressModel> logger) : PageModel
    {
        readonly IFriendsService _service = service;
        readonly IAddressesService _addressesService = addressesService;
        readonly ILogger<EditAddressModel> _logger = logger;

        [BindProperty]
        public FineAddressIM AddressIM { get; set; }

        public string PageHeader { get; set; }

        //public member becomes part of the Model in the Razor page
        public string ErrorMessage { get; set; } = null;

        //Will execute on a Get request
        public async Task<IActionResult> OnGet()
        {
            try
            {
                Guid.TryParse(Request.Query["id"], out Guid _id);

                var response = await _service.ReadFriendAsync(_id, false);

                var fineAddress = await _addressesService.ReadAddressAsync(response.Item.Address.AddressId, true);

                AddressIM = new FineAddressIM(fineAddress.Item);

                PageHeader = "Edit details of a address";
                
                
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
            var response = await _addressesService.ReadAddressAsync(AddressIM.AddressId, false);
            AddressIM = new FineAddressIM(response.Item);
            PageHeader = "Edit details of a address";
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (AddressIM.StatusIM == StatusIM.Inserted)
            {
                //It is an create
                var dto = AddressIM.ToDto();
                var response = await _addressesService.CreateAddressAsync(dto);

                AddressIM = new FineAddressIM(response.Item);
            }
            else
            {
                //It is an update
                //update the changes and save
                var dto = AddressIM.ToDto();
                var updateResponse = await _addressesService.UpdateAddressAsync(dto);
                
                AddressIM = new FineAddressIM(updateResponse.Item);
            }

            PageHeader = "Edit details of a address";
            return Page();
        }


        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public enum StatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class FineAddressIM
        {
            //Status of InputModel
            public StatusIM StatusIM { get; set; }

            //Properties from Model which is to be edited in the <form>
            public Guid AddressId { get; init; } = Guid.NewGuid();
            public string StreetAddress { get; set; }
            public int ZipCode { get; set; }
            public string City { get; set; }
            public string Country { get; set; }


            #region constructors and model update
            public FineAddressIM() { StatusIM = StatusIM.Unchanged; }

            //Copy constructor
            public FineAddressIM(FineAddressIM original)
            {
                StatusIM = original.StatusIM;
                AddressId = original.AddressId;
                StreetAddress = original.StreetAddress;      
                ZipCode = original.ZipCode;
                City = original.City;       
                Country = original.Country;
            }

            public FineAddressIM(IAddress original)
            {
                StatusIM = StatusIM.Unchanged;
                AddressId = original.AddressId;
                StreetAddress = original.StreetAddress;
                ZipCode = original.ZipCode; 
                City = original.City;      
                Country = original.Country;
            }

            public AddressCuDto ToDto()
            {
                return new AddressCuDto
                {
                    AddressId = AddressId,
                    StreetAddress = StreetAddress,
                    ZipCode = ZipCode,
                    City = City,
                    Country = Country
                };
            }
            #endregion
        }
        #endregion
    }
}