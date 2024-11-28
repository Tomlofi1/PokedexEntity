using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [ApiController]
    [Route("api/[controller]reviewer")]
    public class ReviewerController : Controller
    {
        private readonly IReviewerInterface _reviewerInterface;
        private readonly IMapper _mapper; 
        
        public ReviewerController(IReviewerInterface reviewerInterface, IMapper mapper)
        {
            _reviewerInterface = reviewerInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]

        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDTO>>(_reviewerInterface.GetReviewers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewers);

        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]

        public IActionResult GetPokemon(int reviewerId)
        {
            if (!_reviewerInterface.ReviewerExists(reviewerId))
                return NotFound();

            var reviewer = _mapper.Map<ReviewerDTO>(_reviewerInterface.GetReviewer(reviewerId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewer);

        }
        [HttpGet("{reviewerId}/reviews")]
        public IActionResult GetReviewsByAReviewer(int reviewerId)
        {
            if(!_reviewerInterface.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewerInterface.GetReviewsByReviwer(reviewerId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDTO reviewerCreate)
        {
            if (reviewerCreate == null)
                return BadRequest(ModelState);

            var reviewer = _reviewerInterface.GetReviewers().Where(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper()).FirstOrDefault();

            if (reviewer != null)
            {
                ModelState.AddModelError("", "erro ao criar um novo reviewer");
                return StatusCode(422, ModelState);
            }
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

            if (!_reviewerInterface.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Erro ao salvar um novo reviewer");
                return StatusCode(500, ModelState);

            }
            return Ok("Reviewer criado com sucesso");
        }
        
        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDTO reviewerUpdate)
        {
            if (reviewerUpdate == null)
                return BadRequest(ModelState);
            if (reviewerId != reviewerUpdate.Id)
                return BadRequest(ModelState);
            if (!_reviewerInterface.ReviewerExists(reviewerId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerMap = _mapper.Map<Reviewer>(reviewerUpdate);

            if (!_reviewerInterface.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Algo deu errado ao tentar atualizar o Reviewer");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerInterface.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerDelete = _reviewerInterface.GetReviewer(reviewerId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerInterface.DeleteReviewer(reviewerDelete))
            {
                ModelState.AddModelError("", "Algo deu errado tentando deletar o reviewer");
            }

            return NoContent();
        }


    }
}
