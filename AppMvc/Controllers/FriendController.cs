using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppMvc.Models;
using Services;
using Services.Interfaces;
using Models.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.DTO;
using AppMvc.Pages;
using Models.Common;
using System.Linq;

namespace AppMvc.Controllers;

public class FriendController : Controller
{
    private readonly ILogger<FriendController> _logger;

    readonly IQuotesService _quotesService;
    readonly IPetsService _petsService;
    readonly IAdminService _adminService;
    readonly IAddressesService _addressesService;
    readonly IFriendsService _friendsService;

    public string? ErrorMessage { get; set; }
    public string PageHeader { get; set; }
    public string ViewType { get; set; } = "pets"; 

    public int NrOfPages { get; set; }
    public int PageSize { get; } = 5;

    public int ThisPageNr { get; set; } = 0;
    public int PrevPageNr { get; set; } = 0;
    public int NextPageNr { get; set; } = 0;
    public int PresentPages { get; set; } = 0;

    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;


    public FriendController(ILogger<FriendController> logger, IAddressesService addressesService, IAdminService adminService, IFriendsService friendsService, IPetsService petsService, IQuotesService quotesService)
    {
        _addressesService = addressesService;
        _logger = logger;
        _adminService = adminService;
        _friendsService = friendsService;
        _petsService = petsService;
        _quotesService = quotesService;
    }

    //Will execute on a Get request
    [HttpGet]
    public async Task <IActionResult> FriendsList(string pagenr)
    {

        if (int.TryParse(pagenr, out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }
            var model = new FriendsListViewModel();

            var response = await _friendsService.ReadFriendsAsync(true, true, null, ThisPageNr, PageSize);
            model.Friends = response.PageItems;

            model.NrOfPages = response.PageCount;
            model.ThisPageNr = response.PageNr; 
            model.PrevPageNr = Math.Max(0, model.ThisPageNr - 1);
            model.NextPageNr = Math.Min(model.NrOfPages - 1, model.ThisPageNr + 1);
            model.PresentPages = model.NrOfPages;

        return View(model);
    }

    [HttpGet]
    public async Task <IActionResult> FriendDetail(string pagenr)
    {

        if (int.TryParse(pagenr, out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }
            var model = new FriendsListViewModel();

            var response = await _friendsService.ReadFriendsAsync(true, true, null, ThisPageNr, PageSize);
            model.Friends = response.PageItems;

            model.NrOfPages = response.PageCount;
            model.ThisPageNr = response.PageNr; 
            model.PrevPageNr = Math.Max(0, model.ThisPageNr - 1);
            model.NextPageNr = Math.Min(model.NrOfPages - 1, model.ThisPageNr + 1);
            model.PresentPages = model.NrOfPages;

        return View(model);
    }

    [HttpGet]
    public async Task <IActionResult> UpdateLists(string id, string view)
    {
        var vm = new FriendsPetsOrQuotesViewModel();
        try
        {
            if (!Guid.TryParse(id, out Guid friendId))
            {
                ErrorMessage = "Invalid friend ID";
                return View();
            }

            ViewType = view?.ToLower() ?? "pets";
            
            var response = await _friendsService.ReadFriendAsync(friendId, false);
            vm.Friend = response.Item;

            if (vm.Friend == null)
            {
                ErrorMessage = "Friend not found";
                return View();
            }

            if (ViewType == "pets")
            {
                vm.Pets = vm.Friend.Pets.ToList();
            }
            else if (ViewType == "quotes")
            {
                vm.Quotes = vm.Friend.Quotes.ToList();
            }

            return View();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return View();
        }
    }
    public async Task<IActionResult> EditFriend()
    {
        var vm = new EditFriendViewModel();
        try
        {
            if (Guid.TryParse(Request.Query["id"], out Guid _id))
            {
                //Use the Service and populate the InputModel
                var response = await _friendsService.ReadFriendAsync(_id, false);
                vm.FriendIM = new BestFriendIM(response.Item);
                PageHeader = "Edit details of a friend";
            }
            else
            {
                //Create an empty InputModel
                vm.FriendIM = new BestFriendIM();
                vm.FriendIM.StatusIM = StatusIM.Inserted;
                PageHeader = "Create a new friend";
            }
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid addressId, Guid friendId)
    {
        var vm = new EditFriendViewModel();
        var response = await _friendsService.ReadFriendAsync(vm.FriendId, false);
        vm.FriendId = response.Item.FriendId;
        return View(vm);
    }

    [HttpGet]
    public async Task <IActionResult> FriendsInACity(string pagenr, string city, string country)
    {
        var vm = new FriendsInACityViewModel();

        vm.City = city ?? "";
        vm.Country = country ?? "";
        
        if (int.TryParse(pagenr, out int _pagenr))
        {
            ThisPageNr = _pagenr;
        }
        // Get all addresses (use a large page size to get all)
            var info = await _addressesService.ReadAddressesAsync(true, false, null, 0, 1000);

            // Filter friends whose address is in the selected city and country
            var allFriends = info.PageItems
                .Where(a => a.City == vm.City && a.Country == vm.Country)
                .SelectMany(a => a.Friends)
                .ToList();

            // Get all pets for those friends
            vm.Pets = allFriends
                .SelectMany(f => (f.Pets ?? new List<IPet>()))
                .ToList();

            // Calculate pagination
            vm.NrOfPages = (int)Math.Ceiling(allFriends.Count / (double)PageSize);
            vm.ThisPageNr = Math.Min(ThisPageNr, Math.Max(0, vm.NrOfPages - 1));
            vm.PrevPageNr = Math.Max(0, vm.ThisPageNr - 1);
            vm.NextPageNr = Math.Min(Math.Max(0, vm.NrOfPages - 1), vm.ThisPageNr + 1);
            vm.PresentPages = vm.NrOfPages;

            // Get friends for current page
            vm.Friends = allFriends
                .Skip(vm.ThisPageNr * PageSize)
                .Take(PageSize)
                .ToList();

        return View(vm);
    }


    [HttpGet]
    public async Task <IActionResult> ModelView(string id)
    {
        var vm = new ModelViewModel();
        try
        {
            Guid _id = Guid.Parse(id);
            var response = await _friendsService.ReadFriendAsync(_id, false);
            vm.Friend = response.Item; 
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
        return View(vm);
    }

    // Får pets/quotes via asp-route-view i ModelView
    // Lägger skapar inputmodeller av databasmodellerna
    [HttpGet]
    public async Task <IActionResult> FriendsPetsOrQuotesModel(string id, string view)
    {
        var vm = new FriendsPetsOrQuotesViewModel();
        try
        {
            if (!Guid.TryParse(id, out Guid friendId))
            {
                ErrorMessage = "Invalid friend ID";
                return View();
            }

            ViewType = view?.ToLower() ?? "pets";
            
            var response = await _friendsService.ReadFriendAsync(friendId, false);
            vm.Friend = response.Item;

            if (vm.Friend == null)
            {
                ErrorMessage = "Friend not found";
                return View();
            }

            if (ViewType == "pets")
            {
                vm.Pets = vm.Friend.Pets.ToList();
            }
            else if (ViewType == "quotes")
            {
                vm.Quotes = vm.Friend.Quotes.ToList();
            }

            return View();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeletePet(Guid id, Guid friendId)
    {
        var vm = new FriendsPetsOrQuotesViewModel();
        var response = await _friendsService.ReadFriendAsync(friendId, false);
        vm.Friend = response.Item;
        vm.Pets = vm.Friend.Pets.ToList();
        vm.PetsIM = vm.Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
        var petToDelete = vm.PetsIM.FirstOrDefault(q => q.PetId == id);
        if (petToDelete != null)
        {
            petToDelete.StatusIM = StatusIM.Deleted;
        }
        // Om någon pet i listorna har tagits bort så kör den delete på databasobjektet via service 
            var _petsDeletes = vm.PetsIM.FindAll(q => (q.StatusIM == StatusIM.Deleted));
            foreach (var item in _petsDeletes)
            {
                await _petsService.DeletePetAsync(item.PetId);
            }
            vm.PetsIM = vm.Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
        return RedirectToAction(nameof(EditFriend), new { id = friendId, view = "pets" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuote(Guid id, Guid friendId)
    {
        var response = await _friendsService.ReadFriendAsync(friendId, false);
        var vm = new FriendsPetsOrQuotesViewModel();
        vm.Friend = response.Item;
        vm.QuotesIM = vm.Friend.Quotes.Select(q => new FavoriteQuoteIM(q)).ToList();
        var quoteToDelete = vm.QuotesIM.FirstOrDefault(q => q.QuoteId == id);
        if (quoteToDelete != null)
        {
            quoteToDelete.StatusIM = StatusIM.Deleted;
        }

        // Om någon quote i listorna har tagits bort så kör den delete på databasobjektet via service 
        var _quotesDeletes = vm.QuotesIM.FindAll(q => (q.StatusIM == StatusIM.Deleted));
        foreach (var item in _quotesDeletes)
        {
            await _quotesService.DeleteQuoteAsync(item.QuoteId);
        }
        vm.QuotesIM = vm.Friend.Quotes.Select(q => new FavoriteQuoteIM(q)).ToList();

        return RedirectToAction(nameof(EditFriend), new { id = friendId, view = "quotes" });
    }

    public async Task<IActionResult> Save(EditFriendViewModel vm)
    {
        vm.PageHeader = (vm.FriendIM.StatusIM == StatusIM.Inserted) ?
            "Create a new friend" : "Edit details of a friend";

        if (!ModelState.IsValid)
        {
            return View("Edit", vm);
        }
        if (vm.FriendIM.StatusIM == StatusIM.Inserted)
        {
            var dto = vm.FriendIM.ToDto();
            var response = await _friendsService.CreateFriendAsync(dto);
            vm.FriendIM = new BestFriendIM(response.Item);
        }
        else
        {
            var dto = vm.FriendIM.ToDto();
            var updateResponse = await _friendsService.UpdateFriendAsync(dto);
            
            vm.FriendIM = new BestFriendIM(updateResponse.Item);
        
        }

        vm.PageHeader= "Edit details of a friend";
        return RedirectToAction("FriendsList");
    }


    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

