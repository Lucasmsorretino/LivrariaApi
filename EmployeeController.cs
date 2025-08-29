using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SuperHeroApi.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private static List<Employee> dados = new List<Employee>
            {

                new Employee{
                    Id = 2,
                    PrimeiroNome = "Jason",
                    UltimoNome="Momoa",
                    EmailId = "jason_mm@gmail.com"}
             };

    [HttpGet]
    public async Task<ActionResult<List<SuperHero>>> Get()
    {
        return Ok(dados);
    }
}
}
