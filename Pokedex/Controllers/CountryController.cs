using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [ApiController]
    [Route("api[controller]")]
    public class CountryController : Controller
    {

        private readonly ICountryInterface _countryInterface;
        private readonly IMapper _mapper;
        public CountryController(ICountryInterface countryInterface, IMapper mapper)
        {
            _countryInterface = countryInterface;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]

        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDTO>>(_countryInterface.GetCountries());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(countries);

        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]

        public IActionResult GetCountry(int countryId)
        {
            if (!_countryInterface.CountryExists(countryId))
                return NotFound();

            var country = _mapper.Map<CountryDTO>(_countryInterface.GetCountry(countryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]

        public IActionResult GetCountryOfAnOwner(int ownerId)
        {
            var country = _countryInterface.GetCountryByOwner(ownerId); // o erro estava aqui, prestar mais atencao ao chamar as funcoes, eu tinha chamado o ICollection em vez da func OwnerId
            if (country == null)
            {
                return NotFound("Country not found for this owner.");
            }

            var countryDto = _mapper.Map<CountryDTO>(country);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(countryDto);
        }
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]

        public IActionResult CreateCountry([FromBody] CountryDTO countryCreate)
        {
            if (countryCreate == null)
                return BadRequest(ModelState);

            var country = _countryInterface.GetCountries().Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", "Pais ja existe no Banco de dados");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryCreate);

            if (!_countryInterface.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Algo deu errado tentando salvar");
                return StatusCode(500, ModelState);
            }

            return Ok("Pais salvo com Sucesso");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDTO countryUpdated)
        {
            if (countryUpdated == null)
                return BadRequest(ModelState);
            if (countryId != countryUpdated.Id)
                return BadRequest(ModelState);

            if (!_countryInterface.CountryExists(countryId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(countryUpdated);

            if (!_countryInterface.UpdateCountry(countryMap))
            {
                ModelState.AddModelError("", "Algo deu errado tentando atualizar o pais");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public IActionResult RemoveCountry(int countryId)
        {
            if (!_countryInterface.CountryExists(countryId))
                return NotFound();

            var countryDelete = _countryInterface.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_countryInterface.RemoveCountry(countryDelete))
            {
                ModelState.AddModelError("", "Algo deu erradoa o tentar remover o pais");
            }
            return NoContent();
        }
    }
}
