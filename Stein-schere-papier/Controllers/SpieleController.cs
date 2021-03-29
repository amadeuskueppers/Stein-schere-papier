using System.Linq;
using DataClasses;
using Microsoft.AspNetCore.Mvc;

namespace Stein_schere_papier.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpieleController : ControllerBase
    {
        private readonly ISpielLogic _spielLogic;

        public SpieleController(ISpielLogic spielLogic)
        {
            _spielLogic = spielLogic;
        }

        [HttpGet]
        public IActionResult GetSpiele([FromQuery] bool nurOffeneSpiele)
        {
            var spiele = _spielLogic.GetAlleSpiele();

            if (nurOffeneSpiele) spiele = spiele.Where(s => s.ZweitenSpieler == null);

            return Ok(spiele);
        }

        [HttpGet("{spielId:int}")]
        public IActionResult GetSpielById(int spielId)
        {
            var spiel = _spielLogic.GetAlleSpiele().First(s => s.Id == spielId);

            return Ok(spiel);
        }

        [HttpPost]
        public IActionResult CreateSpiel()
        {
            var spiel = _spielLogic.NeuesSpielErstellen();

            return CreatedAtAction(nameof(GetSpielById), new {spielId = spiel.Id}, spiel);
        }

        [HttpPost("{spielId:int}/spieler")]
        public IActionResult CreateSpielerFuerSpiel([FromBody] Spieler spieler, int spielId)
        {
            var spiel = _spielLogic.SpielerZuSpielHinzufuegen(spielId, spieler);
            return Ok(spiel);
        }

        [HttpGet("{spielId:int}/gewinner")]
        public IActionResult GetGewinner(int spielId)
        {
            var gewinner = _spielLogic.GewinnerErmitteln(spielId);
            return Ok(gewinner);
        }
    }
}