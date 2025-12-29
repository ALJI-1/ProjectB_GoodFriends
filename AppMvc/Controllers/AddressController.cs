using Microsoft.AspNetCore.Mvc;
using AppMvc.Models;
using Services.Interfaces;
using Models.DTO;
using Models.Common;

namespace AppMvc.Controllers;

public class AddressController : Controller
{
    readonly IAdminService _adminService;
    readonly IAddressesService _addressesService;
    readonly IFriendsService _friendService;

    public int PageSize { get; } = 5;

    public AddressController(IAddressesService addressesService, IAdminService adminService, IFriendsService friendsService)
    {
        _addressesService = addressesService;
        _adminService = adminService;
        _friendService = friendsService;
    }

    public async Task<IActionResult> EditAddress(Guid id)
    {
        var vm = new EditAddressViewModel()
            {
                FriendId = id
            };
        try
        {
            

            var friendResponse = await _friendService.ReadFriendAsync(id, false);

            if (friendResponse.Item.Address != null)
            {
                var fineAddress = await _addressesService.ReadAddressAsync(friendResponse.Item.Address.AddressId, true);
                vm.AddressIM = new FineAddressIM(fineAddress.Item);
                vm.PageHeader = "Edit details of a address";
                
            }
            else
            {
                vm.AddressIM = new FineAddressIM()
                {
                    StatusIM = StatusIM.Inserted
                };
                vm.PageHeader = "Create a new address";
            }
        }
        catch (Exception e)
        {
            vm.ErrorMessage = e.Message;
            vm.AddressIM = new FineAddressIM();
            vm.PageHeader = "Edit details of a address";
        }
        return View(vm);
    }

    [HttpGet]
    public async Task <IActionResult> CountryOverview()
    {
        var co = new CountryOverviewModel();
         var info = await _adminService.GuestInfoAsync();

            co.CountryInfo = info.Item.Friends
                .Where(i => !string.IsNullOrEmpty(i.Country))
                .GroupBy(i => i.Country)
                .Select(g => new GstUsrInfoFriendsDto 
                { 
                    Country = g.Key, 
                    NrFriends = g.Sum(x => x.NrFriends),
                    City = null
                })
                .OrderBy(i => i.Country)
                .ToList();

        
        return View(co);
    }

    [HttpGet]
    public async Task <IActionResult> FriendsInACountry(string pagenr, string filter)
    {
        var vm = new FriendsInACountryViewModel();
        int thisPageNr = 0;
        if (!string.IsNullOrEmpty(filter) && Enum.TryParse<Countries>(filter, true, out var country))
        {
            vm.SelectedCountry = country;
            vm.Filter = country.ToString();
        }
        else
        {
            vm.Filter = filter ?? "";
        }
        
        if (int.TryParse(pagenr, out int _pagenr))
        {
            thisPageNr = _pagenr;
        }
        var info = await _adminService.GuestInfoAsync();

            var friends = info.Item.Friends.Where(i => i.Country == vm.Filter && !string.IsNullOrEmpty(i.City));
            var pets = info.Item.Pets.Where(i => i.Country == vm.Filter && !string.IsNullOrEmpty(i.City));

            vm.CityInfoList = friends.Select(f => new CityInfo
            {
                Country = f.Country,
                City = f.City,
                NrFriends = f.NrFriends,
                NrPets = pets.FirstOrDefault(p => p.City == f.City && p.Country == f.Country)?.NrPets ?? 0
            });


            // Get all addresses (use a large page size to get all)
            var addressinfo = await _addressesService.ReadAddressesAsync(true, false, null, 0, 1000);

            // Filter friends whose address is in the selected country
            var allFriends = addressinfo.PageItems
                .SelectMany(a => a.Friends)
                .Where(f => f.Address?.Country == vm.Filter)
                .ToList();

            // Calculate pagination
            vm.NrOfPages = (int)Math.Ceiling(allFriends.Count / (double)PageSize);
            vm.ThisPageNr = Math.Min(thisPageNr, vm.NrOfPages - 1);
            vm.PrevPageNr = Math.Max(0, vm.ThisPageNr - 1);
            vm.NextPageNr = Math.Min(vm.NrOfPages - 1, vm.ThisPageNr + 1);
            vm.PresentPages = vm.NrOfPages;

            // Get friends for current page
            vm.Friends = allFriends
                .Skip(vm.ThisPageNr * PageSize)
                .Take(PageSize)
                .ToList();

            return View(vm); 
    }

    public async Task<IActionResult> Save(EditAddressViewModel vm)
    {
        vm.PageHeader = (vm.AddressIM.StatusIM == StatusIM.Inserted) ?
            "Create a new address" : "Edit details of a address";

        if (!ModelState.IsValid)
        {
            // Populate validation error messages for server-side validation display
            vm.HasValidationErrors = true;
            vm.ValidationErrorMsgs = ModelState
                .Where(s => s.Value!.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                .SelectMany(e => e.Value!.Errors)
                .Select(e => e.ErrorMessage);
            return View("EditAddress", vm);
        }
        if (vm.AddressIM.StatusIM == StatusIM.Inserted)
        {
            try {

                var dto = vm.AddressIM.ToDto();
                var response = await _addressesService.CreateAddressAsync(dto);
                vm.AddressIM = new FineAddressIM(response.Item);
            }
            catch (ArgumentException ex){
                var existingId = Guid.Parse(ex.Message.Split("id ")[1]);

                var existingAddress = await _addressesService.ReadAddressAsync(existingId, true);
                    vm.AddressIM = new FineAddressIM(existingAddress.Item);
            }

            // Länkar nya addressen till friend
            var friendResponse = await _friendService.ReadFriendAsync(vm.FriendId, false);
            var friendDto = new FriendCuDto(friendResponse.Item)
            {
                AddressId = vm.AddressIM.AddressId
            };
            await _friendService.UpdateFriendAsync(friendDto);
        }
        else
        {
            //Uppdaterar addressen
            var dto = vm.AddressIM.ToDto();
            var updateResponse = await _addressesService.UpdateAddressAsync(dto);
            
            vm.AddressIM = new FineAddressIM(updateResponse.Item);
            
            // Ensure friend still links to the address after update
            var friendResponse = await _friendService.ReadFriendAsync(vm.FriendId, false);
            if (friendResponse.Item.Address?.AddressId != vm.AddressIM.AddressId)
            {
                var friendDto = new FriendCuDto(friendResponse.Item)
                {
                    AddressId = vm.AddressIM.AddressId
                };
                await _friendService.UpdateFriendAsync(friendDto);
            }
        }

        return RedirectToAction("ModelView", "Friend", new { id = vm.FriendId });
    }
}