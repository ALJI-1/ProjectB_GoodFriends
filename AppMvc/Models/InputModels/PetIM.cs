using Models.Interfaces;
using Models.Common;

namespace AppMvc.Models
{
    public class FavoritePetIM
        {
            public StatusIM StatusIM { get; set; }
            public Guid PetId { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = string.Empty;
            public AnimalMood Mood { get; set; }

            public FavoritePetIM() { StatusIM = StatusIM.Unchanged; }

            public FavoritePetIM(FavoritePetIM original)
            {
                StatusIM = original.StatusIM;

                PetId = original.PetId;
                Name = original.Name;
                Mood = original.Mood;
            }
            public FavoritePetIM(IPet pet)
            {
                StatusIM = StatusIM.Unchanged;
                PetId = pet.PetId;
                Name = pet.Name;
                Mood = pet.Mood;
            }
        }
}