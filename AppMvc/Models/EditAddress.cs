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
using DbModels;


namespace AppMvc.Models
{
    //Demonstrate how to read Query parameters
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