using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppMvc.Models;
using Services;
using Services.Interfaces;
using Models.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AppRazor.Pages.Friends;
using Models.DTO;
using AppMvc.Pages;
using Models.Common;

namespace AppMvc.Controllers;

public class ModelController : Controller
{
    private readonly ILogger<ModelController> _logger;

    readonly IQuotesService _quotesService;
    readonly IPetsService _petsService;
    readonly IAdminService _adminService;
    readonly IAddressesService _addressesService;
    readonly IFriendsService _friendsService;
    public List<IFriend> Friends {get; set;} = new List<IFriend>();
    public List<FavoritePetIM> PetsIM { get; set; } = new List<FavoritePetIM>();
    public List<FavoriteQuoteIM> QuotesIM { get; set; } = new List<FavoriteQuoteIM>();
    public IFriend? Friend { get; set; }
    public string? ErrorMessage { get; set; }
    public string PageHeader { get; set; }
    public BestFriendIM FriendIM { get; set; }
    public string ViewType { get; set; } = "pets"; 

    public int NrOfPages { get; set; }
    public int PageSize { get; } = 5;

    public int ThisPageNr { get; set; } = 0;
    public int PrevPageNr { get; set; } = 0;
    public int NextPageNr { get; set; } = 0;
    public int PresentPages { get; set; } = 0;


    public ModelController(ILogger<ModelController> logger, IAddressesService addressesService, IAdminService adminService, IFriendsService friendsService, IPetsService petsService, IQuotesService quotesService)
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
            var model = new FriendsListModel();

            var response = await _friendsService.ReadFriendsAsync(true, true, null, ThisPageNr, PageSize);
            model.Friends = response.PageItems;

            NrOfPages = response.PageCount;
            ThisPageNr = response.PageNr; 
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            PresentPages = NrOfPages;

        return View(model);
    }

    [HttpGet]
    public async Task <IActionResult> FriendDetail(string pagenr)
    {

        if (int.TryParse(pagenr, out int _pagenr))
            {
                ThisPageNr = _pagenr;
            }
            var model = new FriendsListModel();

            var response = await _friendsService.ReadFriendsAsync(true, true, null, ThisPageNr, PageSize);
            model.Friends = response.PageItems;

            NrOfPages = response.PageCount;
            ThisPageNr = response.PageNr; 
            PrevPageNr = Math.Max(0, ThisPageNr - 1);
            NextPageNr = Math.Min(NrOfPages - 1, ThisPageNr + 1);
            PresentPages = NrOfPages;

        return View(model);
    }

    [HttpGet]
    public async Task <IActionResult> UpdateLists(string id, string view)
    {
    try
        {
            if (!Guid.TryParse(id, out Guid friendId))
            {
                ErrorMessage = "Invalid friend ID";
                return View();
            }

            ViewType = view?.ToLower() ?? "pets";
            
            var response = await _friendsService.ReadFriendAsync(friendId, false);
            Friend = response.Item;

            if (Friend == null)
            {
                ErrorMessage = "Friend not found";
                return View();
            }

            if (ViewType == "pets")
            {
                PetsIM = Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
            }
            else if (ViewType == "quotes")
            {
                QuotesIM = Friend.Quotes.Select(p => new FavoriteQuoteIM(p)).ToList();
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
        try
        {
            if (Guid.TryParse(Request.Query["id"], out Guid _id))
            {
                //Use the Service and populate the InputModel
                var response = await _friendsService.ReadFriendAsync(_id, false);
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
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid addressId, Guid friendId)
    {
        var response = await _friendsService.ReadFriendAsync(FriendIM.FriendId, false);
        var vm = new EditFriendViewModel(response.Item) { FriendId = friendId };
        return View(vm);
    }


    [HttpGet]
    public async Task <IActionResult> ModelView(string id)
    {
        try
        {
            Guid _id = Guid.Parse(id);
            var response = await _friendsService.ReadFriendAsync(_id, false);
            Friend = response.Item; 
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
        return View(Friend);
    }

    // Får pets/quotes via asp-route-view i ModelView
    // Lägger skapar inputmodeller av databasmodellerna
    [HttpGet]
    public async Task <IActionResult> ReadPetsQuotes(string id, string view)
    {
        try
            {
                if (!Guid.TryParse(id, out Guid friendId))
                {
                    ErrorMessage = "Invalid friend ID";
                    return View();
                }

                ViewType = view?.ToLower() ?? "pets";
                
                var response = await _friendsService.ReadFriendAsync(friendId, false);
                Friend = response.Item;

                if (Friend == null)
                {
                    ErrorMessage = "Friend not found";
                    return View();
                }

                if (ViewType == "pets")
                {
                    PetsIM = Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
                }
                else if (ViewType == "quotes")
                {
                    QuotesIM = Friend.Quotes.Select(p => new FavoriteQuoteIM(p)).ToList();
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
        var response = await _friendsService.ReadFriendAsync(friendId, false);
        Friend = response.Item;
        PetsIM = Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
        var petToDelete = PetsIM.FirstOrDefault(q => q.PetId == id);
        if (petToDelete != null)
        {
            petToDelete.StatusIM = StatusIM.Deleted;
        }
        // Om någon pet i listorna har tagits bort så kör den delete på databasobjektet via service 
            var _petsDeletes = PetsIM.FindAll(q => (q.StatusIM == StatusIM.Deleted));
            foreach (var item in _petsDeletes)
            {
                await _petsService.DeletePetAsync(item.PetId);
            }
            PetsIM = Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
        return RedirectToAction(nameof(EditFriend), new { id = friendId, view = "pets" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuote(Guid id, Guid friendId)
    {
        var response = await _friendsService.ReadFriendAsync(friendId, false);
        QuotesIM = Friend.Quotes.Select(q => new FavoriteQuoteIM(q)).ToList();
        var quoteToDelete = QuotesIM.FirstOrDefault(q => q.QuoteId == id);
        if (quoteToDelete != null)
        {
            quoteToDelete.StatusIM = StatusIM.Deleted;
        }

        // Om någon quote i listorna har tagits bort så kör den delete på databasobjektet via service 
        var _quotesDeletes = QuotesIM.FindAll(q => (q.StatusIM == StatusIM.Deleted));
        foreach (var item in _quotesDeletes)
        {
            await _quotesService.DeleteQuoteAsync(item.QuoteId);
        }
        QuotesIM = Friend.Quotes.Select(q => new FavoriteQuoteIM(q)).ToList();

        return RedirectToAction(nameof(EditFriend), new { id = friendId, view = "quotes" });
    }

    public async Task<IActionResult> Save(EditFriendViewModel vm)
    {
        vm.PageHeader = (FriendIM.StatusIM == StatusIM.Inserted) ?
            "Create a new friend" : "Edit details of a friend";

        if (!ModelState.IsValid)
        {
            return View("Edit", vm);
        }
        if (vm.FriendIM.StatusIM == StatusIM.Inserted)
        {
            var dto = FriendIM.ToDto();
            var response = await _friendsService.CreateFriendAsync(dto);
            FriendIM = new BestFriendIM(response.Item); 
        }
        else
        {
            var dto = FriendIM.ToDto();
            var updateResponse = await _friendsService.UpdateFriendAsync(dto);
            
            FriendIM = new BestFriendIM(updateResponse.Item);
        
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

