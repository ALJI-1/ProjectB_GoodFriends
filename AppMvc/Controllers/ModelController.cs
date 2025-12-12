using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppMvc.Models;
using Services;
using Services.Interfaces;
using Models.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppStudiesMVC.Controllers;

public class ModelController : Controller
{
    private readonly ILogger<ModelController> _logger;
    readonly IAdminService _adminService;
    readonly IAddressesService _addressesService;
    readonly IFriendsService _friendService;
    public List<IFriend> Friends {get; set;} = new List<IFriend>();
    public IFriend? Friend { get; set; }
    public string? ErrorMessage { get; set; }


    public ModelController(ILogger<ModelController> logger, IAddressesService addressesService, IAdminService adminService, IFriendsService friendsService)
    {
        _addressesService = addressesService;
        _logger = logger;
        _adminService = adminService;
        _friendService = friendsService;
    }

    //Will execute on a Get request
    [HttpGet]
    public async Task <IActionResult> Overview()
    {
        var addresses = await _addressesService.ReadAddressesAsync(true, false, "Denmark", 0, 10);
        var info = await _adminService.GuestInfoAsync();

        var model = new OverviewModel
        {
            CountryInfo = info.Item.Friends.Where(i => i.Country == "Denmark")
        };

        return View(model);
    }

    [HttpGet]
    public async Task <IActionResult> ModelView(string id)
    {
        try
        {
            Guid _id = Guid.Parse(id);
            var response = await _friendService.ReadFriendAsync(_id, false);
            Friend = response.Item; 
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
        return View(Friend);
    }

    [HttpGet]
    public async Task <IActionResult> CityOverview()
    {
        var fm = new CityOverviewModel();

        var info = await _adminService.GuestInfoAsync();

        var friends = info.Item.Friends.Where(i => i.Country == "Denmark");
        var pets = info.Item.Pets.Where(i => i.Country == "Denmark");
        
        fm.CityInfoList = friends.Select(f => new CityOverviewModel
            {
                Country = f.Country,
                City = f.City,
                NrFriends = f.NrFriends,
                NrPets = pets.FirstOrDefault(p => p.City == f.City && p.Country == f.Country)?.NrPets ?? 0
            }).ToList();

        return View(fm);
    }

    [HttpGet]
    public async Task <IActionResult> AllFriendsInACountry()
    {
        var info = await _addressesService.ReadAddressesAsync(true, false, null, 0, 10);

        var friends = info.PageItems.SelectMany(a => a.Friends).Where(c => c.Address.Country == "Sweden").ToList();

        var model = new AllFriendsInACountryModel
        {
            Friends = friends
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

