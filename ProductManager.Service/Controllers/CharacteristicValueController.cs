using Microsoft.AspNetCore.Mvc;
using ProductManager.Glue.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductManager.Service.Controllers
{
    /// <summary>
    /// Represents a controller for managing characteristic values.
    /// </summary>
    [Route("api/Characteristic/{id:guid}/value")]
    [ApiController]
    public class CharacteristicValueController : ControllerBase
    {
        private readonly ILogger<CharacteristicValueController> _logger;
        private readonly ICharacteristicService _characteristicService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacteristicValueController"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="characteristicService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CharacteristicValueController(ILogger<CharacteristicValueController> logger,
            ICharacteristicService characteristicService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _characteristicService =
                characteristicService ?? throw new ArgumentNullException(nameof(characteristicService));
        }


        /// <summary>
        /// Adds a value to a characteristic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(Guid id, [FromBody] string value)
        {   
            await _characteristicService.AddValueToCharacteristicAsync(id, value);
            return Ok();
        }

    }
}
