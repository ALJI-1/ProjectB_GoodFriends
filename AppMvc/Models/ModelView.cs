using Models.Interfaces;

namespace AppMvc.Models
{
    public class ModelViewModel()
    {
        public IFriend? Friend { get; set; }
        public string? ErrorMessage { get; set; }     
    }
}
