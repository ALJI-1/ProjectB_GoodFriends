using DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Models.Interfaces;
using Services;
using Services.Interfaces;

namespace AppMvc.Models
{
    public class AllFriendsInACountryModel
    {
        public List<IFriend> Friends {get; set;} = new List<IFriend>();
    }
}