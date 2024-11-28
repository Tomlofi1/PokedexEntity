using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;


namespace PokemonReviewApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : Controller
    {
        private readonly IPokemonInterface _pokemonInterface;
        private readonly IMapper _mapper;
        private readonly IReviewerInterface _reviewerInterface;
        private readonly IReviewInterface _reviewInterface;

        public PokemonController(IPokemonInterface pokemonInterface, IMapper mapper, IReviewerInterface reviewerInterface, IReviewInterface reviewInterface )
        {
            _pokemonInterface = pokemonInterface;
            _mapper = mapper;
            _reviewerInterface = reviewerInterface;
            _reviewInterface = reviewInterface;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]

        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDTO>>(_pokemonInterface.GetPokemons());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }
        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonInterface.PokemonExist(pokeId))
            {
                return NotFound();
            }

            var pokemon = _mapper.Map<PokemonDTO>(_pokemonInterface.GetPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }
        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]

        public IActionResult GetPokemonRating(int pokeId)
        {

            if (!_pokemonInterface.PokemonExist(pokeId))
            {
                return NotFound();
            }

            var rating = _pokemonInterface.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDTO pokemonCreate)
        {
            if (pokemonCreate == null)
                return BadRequest(ModelState);

            var pokemons = _pokemonInterface.GetPokemonTrimToUpper(pokemonCreate);
            
            if(pokemons != null)
            {
                ModelState.AddModelError("", "Dono ja existente");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if (!_pokemonInterface.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Algo deu errado ao salvar");
                return StatusCode(500, ModelState);
            }
            return Ok("Pokemon criado e salvo com sucesso");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult UpdatePokemon(int pokeId, [FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDTO pokemonUpdate)
        {
            if (pokemonUpdate == null)
                return BadRequest(ModelState);

            if (pokeId != pokemonUpdate.Id)
                return BadRequest(ModelState);
            if (!_pokemonInterface.PokemonExist(pokeId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokeMap = _mapper.Map<Pokemon>(pokemonUpdate);

            if (!_pokemonInterface.UpdatePokemon(ownerId,catId,pokeMap))
            {
                ModelState.AddModelError("", "Algo deu errado ao tentar salvar");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        
        public IActionResult RemovePokemon(int pokeId)
        {
            if (!_pokemonInterface.PokemonExist(pokeId))
                return NotFound();

            var reviewsDelete = _reviewInterface.GetReviewsOfAPokemon(pokeId);
            var pokemonDelete = _pokemonInterface.GetPokemon(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewInterface.RemoveReviews(reviewsDelete.ToList())) 
            {
                ModelState.AddModelError("", "Algo deu errado ao deletar as reviews");
            }
            if (!_pokemonInterface.RemovePokemon(pokemonDelete))
            {
                ModelState.AddModelError("", "Algo deu errado tentando deletar");
            }

            return NoContent();
        }

    }
}
