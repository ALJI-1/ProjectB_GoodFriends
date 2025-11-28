using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppMvc.Models;
using Services.Interfaces;

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

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task <IActionResult> Overview()
    {
        var addresses = await _addressService.ReadAddressesAsync(true, false, "Denmark", 0, 10);
        var info = await _adminService.GuestInfoAsync();

        var model = new OverviewViewModel
        {
            CountryInfo = info.Item.Friends.Where(i => i.Country == "Denmark")
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
