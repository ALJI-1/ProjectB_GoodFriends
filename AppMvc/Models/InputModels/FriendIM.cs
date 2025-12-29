using System.ComponentModel.DataAnnotations;
using Models.DTO;
using Models.Interfaces;
using Models.Common;


namespace AppMvc.Pages
{
    public class BestFriendIM
    {
        //Status of InputModel
        public StatusIM StatusIM { get; set; }

        //Properties from Model which is to be edited in the <form>
        public Guid FriendId { get; set; } = Guid.NewGuid();
        
        [Required(ErrorMessage = "You must provide a first name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must provide a last name")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "You must provide an email")]
        public string Email { get; set; } = string.Empty;


        public BestFriendIM() { StatusIM = StatusIM.Unchanged; }

        //Copy constructor
        public BestFriendIM(BestFriendIM original)
        {
            StatusIM = original.StatusIM;
            FriendId = original.FriendId;
            FirstName = original.FirstName;
            LastName = original.LastName;       
            Email = original.Email;
        }

        public BestFriendIM(IFriend original)
        {
            StatusIM = StatusIM.Unchanged;
            FriendId = original.FriendId;
            FirstName = original.FirstName;
            LastName = original.LastName;       
            Email = original.Email;
        }

        public FriendCuDto ToDto()
        {
            return new FriendCuDto
            {
                FriendId = FriendId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email
            };
        }
    }

}

    

