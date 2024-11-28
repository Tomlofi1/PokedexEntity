using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryInterface
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);

        ICollection<Pokemon> GetPokemonsByCategory(int categoryId);

        bool CategoryExists(int id);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool RemoveCategory(Category category);
        bool Save();


    }
}
