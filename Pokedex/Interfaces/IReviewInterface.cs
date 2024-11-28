using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewInterface
    {
        ICollection<Review> GetReviews();
        Review GetReview(int reviewId);

        ICollection<Review> GetReviewsOfAPokemon(int pokeId);

        bool ReviewExists(int reviewId);
        bool CreateReview(Review review);
        bool RemoveReview(Review review);
        bool RemoveReviews(List<Review> reviews);
        bool UpdateReview(Review review);
        bool Save();

        



    }
}
