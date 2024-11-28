using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewerInterface
    {
        ICollection<Reviewer> GetReviewers();

        Reviewer GetReviewer(int reviwerId);

        ICollection<Review> GetReviewsByReviwer(int reviwerId);

        bool ReviewerExists(int reviwerId);
        bool CreateReviewer(Reviewer reviewer);
        bool DeleteReviewer(Reviewer reviewer);
        bool UpdateReviewer(Reviewer reviewer);
        bool Save();

    }
}
