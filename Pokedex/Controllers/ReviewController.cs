using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;


namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewInterface _reviewinterface;
        private readonly IMapper _mapper;
        private readonly IReviewerInterface _reviewerinterface;
        private readonly IPokemonInterface _pokemoninterface;

        public ReviewController(IReviewInterface reviewinterface, IMapper mapper, IReviewerInterface reviewerinterface, IPokemonInterface pokemoninterface)
        {
            _reviewinterface = reviewinterface;
            _mapper = mapper;
            _reviewerinterface = reviewerinterface;
            _pokemoninterface = pokemoninterface;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))] 
        [ProducesResponseType(400)]

        public IActionResult GetReviews()
        {
            var review = _mapper.Map<List<ReviewDTO>>(_reviewinterface.GetReviews());

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]

        public IActionResult GetPokemon(int reviewId)
        {
            if (!_reviewinterface.ReviewExists(reviewId))
                return NotFound();
            var review = _mapper.Map<ReviewDTO>(_reviewinterface.GetReview(reviewId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(review);
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]

        public IActionResult GetReviewOfAPokemon(int pokeId)
        {
            var review = _mapper.Map<List<ReviewDTO>>(_reviewinterface.GetReviewsOfAPokemon(pokeId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(review);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId, [FromBody] ReviewDTO reviewCreate)
        {
            if (reviewCreate == null)
                return BadRequest(ModelState);

            var reviews = _reviewinterface.GetReviews().Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper()).FirstOrDefault();

            if (reviews != null)
            {
                ModelState.AddModelError("", "Review ja existente");
                return StatusCode(422, ModelState);
            }

            var reviewMap = _mapper.Map<Review>(reviewCreate);

            reviewMap.Pokemon = _pokemoninterface.GetPokemon(pokeId);
            reviewMap.Reviewer = _reviewerinterface.GetReviewer(reviewerId);

            if (!_reviewinterface.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Algo deu errado tentando salvar o review");
                return StatusCode(500, ModelState);
            }

            return Ok("Review salvo com sucesso!");

        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDTO reviewUpdate)
        {
            if (reviewUpdate == null)
                return BadRequest(ModelState);
            if (reviewId != reviewUpdate.Id)
                return BadRequest(ModelState);
            if (!_reviewinterface.ReviewExists(reviewId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(reviewUpdate);
            
            if (!_reviewinterface.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Algo deu errado tentando atualizar o Review");
                return StatusCode(500, ModelState);
            }
            
            return NoContent();

        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewinterface.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var reviewDelete = _reviewinterface.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_reviewinterface.RemoveReview(reviewDelete))
            {
                ModelState.AddModelError("", "Algo deu errado tentando deletar o review");
            }
            return NoContent();
        }

        [HttpDelete("/DeleteReviewsByReviewer/{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult RemoveReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerinterface.ReviewerExists(reviewerId))
                return NotFound();

            var reviewsDelete = _reviewerinterface.GetReviewsByReviwer(reviewerId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewinterface.RemoveReviews(reviewsDelete))
            {
                ModelState.AddModelError("", "erro ao deletar reviews");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

    }
}
