using GameBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly GameManager _gameManager;

        public GameController(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        [HttpGet("state")]
        public IActionResult GetGameState()
        {
            return Ok(_gameManager.GetGameState());
        }
    }
}
