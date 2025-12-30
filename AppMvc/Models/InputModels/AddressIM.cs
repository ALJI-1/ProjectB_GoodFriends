using System.ComponentModel.DataAnnotations;
using Models.DTO;
using Models.Interfaces;
using Models.Common;

public class FineAddressIM
{
    public StatusIM StatusIM { get; set; }
    public Guid AddressId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "You must provide a street address")]
    public string StreetAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "You must provide a zip code")]
    public int ZipCode { get; set; }

    [Required(ErrorMessage = "You must provide a city")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "You must provide a country")]
    public string Country { get; set; } = string.Empty;


    #region constructors and model update
    public FineAddressIM() { StatusIM = StatusIM.Unchanged; }

    //Copy constructor
    public FineAddressIM(FineAddressIM original)
    {
        StatusIM = original.StatusIM;
        AddressId = original.AddressId;
        StreetAddress = original.StreetAddress;      
        ZipCode = original.ZipCode;
        City = original.City;       
        Country = original.Country;
    }

    public FineAddressIM(IAddress original)
    {
        StatusIM = StatusIM.Unchanged;
        AddressId = original.AddressId;
        StreetAddress = original.StreetAddress;
        ZipCode = original.ZipCode; 
        City = original.City;      
        Country = original.Country;
    }

    public AddressCuDto ToDto()
    {
        return new AddressCuDto
        {
            // Createaddressasync behöver null värde på id men det sätts nytt guid i FineAddressAsync
            AddressId = StatusIM == StatusIM.Inserted ? null : AddressId,
            StreetAddress = StreetAddress,
            ZipCode = ZipCode,
            City = City,
            Country = Country
        };
    }
    #endregion
}