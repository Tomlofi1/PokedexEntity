namespace PokemonReviewApp.Models
{
    public class Owner
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Gym { get; set; }

        public Country Country { get; set; }//Diferente dos outros, este é muitos para um.

        public ICollection<PokemonOwner> PokemonOwners { get; set; }

    }
}
