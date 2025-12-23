using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppMvc.Models;
using Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace AppMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAddressesService _addressService;
    private readonly IAdminService _adminService;

    public HomeController(ILogger<HomeController> logger, IAddressesService addressesService, IAdminService adminService)
    {
        _logger = logger;
        _addressService = addressesService;
        _adminService = adminService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Seed()
    {
        var info = await _adminService.GuestInfoAsync();
        var vm = new SeedViewModel
        {
            NrOfGroups = info.Item.Db.NrSeededFriends + info.Item.Db.NrUnseededFriends
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Seed(SeedViewModel vm)
    {
        if (ModelState.IsValid)
        {
            if (vm.RemoveSeeds)
            {
                await _adminService.RemoveSeedAsync(true);
                await _adminService.RemoveSeedAsync(false);
            }
            await _adminService.SeedAsync(vm.NrOfItemsToSeed);

            return RedirectToAction("FriendsList", "Friend");
        }
        
        var info = await _adminService.GuestInfoAsync();
        vm.NrOfGroups = info.Item.Db.NrSeededFriends + info.Item.Db.NrUnseededFriends;
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
