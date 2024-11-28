using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [ApiController]
    [Route("api[controller]")]
    public class OwnerController : Controller
    {
        private readonly IOwnerInterface _ownerInterface;
        private readonly IMapper _mapper;
        private readonly ICountryInterface _countryInterface;
        public OwnerController(IOwnerInterface ownerInterface, IMapper mapper, ICountryInterface countryInterface)
        {
            _ownerInterface = ownerInterface;
            _mapper = mapper;
            _countryInterface = countryInterface;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]

        public IActionResult GetOwners() 
        {
            var owners = _mapper.Map<List<OwnerDTO>>(_ownerInterface.GetOwners());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type =typeof(Owner))]
        [ProducesResponseType(400)]

        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerInterface.OwnerExists(ownerId))
                return NotFound();
            var owner = _mapper.Map<OwnerDTO>(_ownerInterface.GetOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owner);
        }
        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]

        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerInterface.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<List<PokemonDTO>>(_ownerInterface.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        
        public IActionResult CreateOwner([FromQuery]int countryId, [FromBody] OwnerDTO createOwner)
        {
            if (createOwner == null)
            {
                return BadRequest(ModelState);
            }

            var owners = _ownerInterface.GetOwners()
                .Where(c => c.LastName.Trim().ToUpper() == createOwner.LastName.TrimEnd().ToUpper()).FirstOrDefault();
            
            if (owners != null)
            {
                ModelState.AddModelError("", "Dono ja existente");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownersMap = _mapper.Map<Owner>(createOwner);

            ownersMap.Country = _countryInterface.GetCountry(countryId);

            if (!_ownerInterface.CreateOwner(ownersMap))
            {
                ModelState.AddModelError("", "Algo deu errado ao salvar");
                return StatusCode(500, ModelState);
            }

            return Ok("Sucesso ao criar");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        
        public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDTO ownerUpdate)
        {
            if (ownerUpdate == null)
                return BadRequest(ModelState);
            if (ownerId != ownerUpdate.Id)
                return BadRequest(ModelState);
            if (!_ownerInterface.OwnerExists(ownerId))
                return NotFound();

            var ownerMap = _mapper.Map<Owner>(ownerUpdate);

            if (!_ownerInterface.UpdateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Erro ao tentar atualizar o dono");
                return StatusCode(500, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerInterface.OwnerExists(ownerId))
            {
                return NotFound();
            }
            var ownerDelete = _ownerInterface.GetOwner(ownerId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_ownerInterface.RemoveOwner(ownerDelete))
            {
                ModelState.AddModelError("", "Algo deu errado tentando apagar o owner");
            }
            return NoContent();
        }

    }
}
