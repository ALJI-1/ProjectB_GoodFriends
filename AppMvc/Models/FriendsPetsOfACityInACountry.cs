using System.IO.Compression;
using DbRepos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Models.Interfaces;
using Services;
using Services.Interfaces;

namespace AppMvc.Models
{
    public class FriendsPetsOfACityInACountryModel
    {
         public string Country { get; set; }
        public string City { get; set; }
        public int NrFriends { get; set; }
        public int NrPets { get; set; }
        public List<FriendsPetsOfACityInACountryModel> CityInfoList { get; set; }
    }
}
