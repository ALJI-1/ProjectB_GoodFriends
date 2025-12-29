using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppMvc.Models;
using AppMvc.Pages;
using Services.Interfaces;
using Models.Interfaces;
using Models.DTO;
using Models.Common;

namespace AppMvc.Controllers;

public class FriendController : Controller
{
    readonly IQuotesService _quotesService;
    readonly IPetsService _petsService;
    readonly IAddressesService _addressesService;
    readonly IFriendsService _friendsService;

    public int PageSize { get; } = 5;

    public FriendController(IAddressesService addressesService, IFriendsService friendsService, IPetsService petsService, IQuotesService quotesService)
    {
        _addressesService = addressesService;
        _friendsService = friendsService;
        _petsService = petsService;
        _quotesService = quotesService;
    }

    //Will execute on a Get request
    [HttpGet]
    public async Task <IActionResult> FriendsList(string pagenr)
    {
        int thisPageNr = 0;
        if (int.TryParse(pagenr, out int _pagenr))
        {
            thisPageNr = _pagenr;
        }
        var model = new FriendsListViewModel();

        var response = await _friendsService.ReadFriendsAsync(true, true, null, thisPageNr, PageSize);
        model.Friends = response.PageItems;

        model.NrOfPages = response.PageCount;
        model.ThisPageNr = response.PageNr; 
        model.PrevPageNr = Math.Max(0, model.ThisPageNr - 1);
        model.NextPageNr = Math.Min(model.NrOfPages - 1, model.ThisPageNr + 1);
        model.PresentPages = model.NrOfPages;

        return View(model);
    }
    public async Task<IActionResult> EditFriend(string id)
    {
        var vm = new EditFriendViewModel();
        try
        {
            if (Guid.TryParse(id, out Guid _id))
            {
                //Use the Service and populate the InputModel
                var response = await _friendsService.ReadFriendAsync(_id, false);
                vm.FriendIM = new BestFriendIM(response.Item);
                vm.PageHeader = "Edit details of a friend";
            }
            else
            {
                //Create an empty InputModel
                vm.FriendIM = new BestFriendIM();
                vm.FriendIM.StatusIM = StatusIM.Inserted;
                vm.PageHeader = "Create a new friend";
            }
        }
        catch (Exception e)
        {
            vm.ErrorMessage = e.Message;
        }
        return View(vm);
    }

    [HttpGet]
    public async Task <IActionResult> FriendsInACity(string pagenr, string city, string country)
    {
        var vm = new FriendsInACityViewModel();

        vm.City = city ?? "";
        vm.Country = country ?? "";
        
        int thisPageNr = 0;
        if (int.TryParse(pagenr, out int _pagenr))
        {
            thisPageNr = _pagenr;
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
        vm.ThisPageNr = Math.Min(thisPageNr, Math.Max(0, vm.NrOfPages - 1));
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
            vm.ErrorMessage = e.Message;
        }
        return View(vm);
    }

    [HttpGet]
    public async Task <IActionResult> FriendsDetails(string id, string view)
    {
        var vm = new FriendsPetsOrQuotesViewModel();
        try
        {
            if (!Guid.TryParse(id, out Guid friendId))
            {
                vm.ErrorMessage = "Invalid friend ID";
                return View(vm);
            }

            vm.ViewType = view?.ToLower() ?? "pets";
            
            var response = await _friendsService.ReadFriendAsync(friendId, false);
            vm.Friend = response.Item;

            if (vm.Friend == null)
            {
                vm.ErrorMessage = "Friend not found";
                return View(vm);
            }

            if (vm.ViewType == "pets")
            {
                vm.Pets = vm.Friend.Pets.ToList();
                vm.PetsIM = vm.Friend.Pets.Select(p => new FavoritePetIM(p)).ToList();
            }
            else if (vm.ViewType == "quotes")
            {
                vm.Quotes = vm.Friend.Quotes.ToList();
                vm.QuotesIM = vm.Friend.Quotes.Select(q => new FavoriteQuoteIM(q)).ToList();
            }

            return View(vm);
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = ex.Message;
            return View(vm);
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
        return RedirectToAction(nameof(ModelView), new { id = friendId });
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

        return RedirectToAction(nameof(ModelView), new { id = friendId });
    }

    [HttpPost]
    public async Task<IActionResult> Undo(EditFriendViewModel vm)
    {
        // Clear ModelState so the reloaded values from database are displayed
        ModelState.Clear();
        
        // Reload the friend from database, discarding any unsaved changes
        var response = await _friendsService.ReadFriendAsync(vm.FriendIM!.FriendId, false);
        vm.FriendIM = new BestFriendIM(response.Item!);
        vm.PageHeader = "Edit details of a friend";
        return View("EditFriend", vm);
    }

    public async Task<IActionResult> Save(EditFriendViewModel vm)
    {
        vm.PageHeader = (vm.FriendIM!.StatusIM == StatusIM.Inserted) ?
            "Create a new friend" : "Edit details of a friend";

        if (!ModelState.IsValid)
        {
            // Populate validation error messages for server-side validation display
            vm.HasValidationErrors = true;
            vm.ValidationErrorMsgs = ModelState
                .Where(s => s.Value!.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                .SelectMany(e => e.Value!.Errors)
                .Select(e => e.ErrorMessage);
            return View("EditFriend", vm);
        }
        if (vm.FriendIM.StatusIM == StatusIM.Inserted)
        {
            var dto = vm.FriendIM.ToDto();
            var response = await _friendsService.CreateFriendAsync(dto);
            vm.FriendIM = new BestFriendIM(response.Item!);
        }
        else
        {
            // Fetch existing friend to preserve relationships (Address, Pets, Quotes)
            var existingFriend = await _friendsService.ReadFriendAsync(vm.FriendIM.FriendId, false);
            
            // Create DTO from existing friend to preserve all relationships
            var dto = new FriendCuDto(existingFriend.Item!)
            {
                // Update only the editable fields
                FirstName = vm.FriendIM.FirstName,
                LastName = vm.FriendIM.LastName,
                Email = vm.FriendIM.Email
            };
            
            var updateResponse = await _friendsService.UpdateFriendAsync(dto);
            vm.FriendIM = new BestFriendIM(updateResponse.Item!);
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

