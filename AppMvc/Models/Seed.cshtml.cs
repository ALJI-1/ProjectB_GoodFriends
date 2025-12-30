using System.ComponentModel.DataAnnotations;

namespace AppMvc.Models
{
    public class SeedViewModel
    {
        public int NrOfGroups { get; set; }

        [Required (ErrorMessage = "You must enter nr of items to seed")]
        public int NrOfItemsToSeed { get; set; } = 100;

        public bool RemoveSeeds { get; set; } = true;
    }
}
