using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SuperHeroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private static List<SuperHero> heroes = new List<SuperHero>
            {
                new SuperHero{
                    Id = 1,
                    Nome = "Thor",
                    PrimeiroNome = "Chris",
                    UltimoNome="Hemsworth",
                    lugar = "Austrália"
                },
                new SuperHero{
                    Id = 2,
                    Nome = "Aquaman",
                    PrimeiroNome = "Jason",
                    UltimoNome="Momoa",
                    lugar = "EUA"
                },
                new SuperHero{
                    Id = 3,
                    Nome = "Capitão América",
                    PrimeiroNome = "Sandy",
                    UltimoNome="Becker",
                    lugar = "EUA"
                },
                new SuperHero{
                    Id = 4,
                    Nome = "Viúva Negra",
                    PrimeiroNome = "Natasha",
                    UltimoNome="Romanoff",
                    lugar = "Rússia"
                },
                new SuperHero{
                    Id = 5,
                    Nome = "Mulher Maravilha",
                    PrimeiroNome = "Gal",
                    UltimoNome="Gaddot",
                    lugar = "Israel"
                }
             };

        [HttpGet]
        public ActionResult<List<SuperHero>> Get()
        {
          return Ok(heroes);
        }
    
        //[HttpGet]
      //  public async Task<ActionResult<SuperHero>> Overview(int id)
       // {
        //    var hero = heroes.Find(h => h.Id == id);
        //    if (hero == null)
           //     return BadRequest("Heroe não encontrado!!!");
           // return Ok(heroes);
       // }

        [HttpPost]
        public ActionResult<List<SuperHero>> AddHero(SuperHero hero)
        {
            heroes.Add(hero);
            return Ok(heroes);
        }

        [HttpPut]
        public ActionResult<List<SuperHero>> UpdateHero(SuperHero request)
        {
            var hero = heroes.Find(h => h.Id == request.Id);
            if (hero == null)
                return BadRequest("Heroe não encontrado!!!");
            hero.Nome = request.Nome;
            hero.PrimeiroNome = request.PrimeiroNome;
            hero.UltimoNome = request.UltimoNome;
            hero.lugar = request.lugar; 

            return Ok(heroes);
        }

        [HttpDelete("{id}")]
        public ActionResult<List<SuperHero>> Delete(int id)
        {

            var hero = heroes.Find(h => h.Id == id);
            if (hero == null)
                return BadRequest("Heroe não encontrado!!!");

            heroes.Remove(hero);
            return Ok(heroes);
        }       
        
    }
}
