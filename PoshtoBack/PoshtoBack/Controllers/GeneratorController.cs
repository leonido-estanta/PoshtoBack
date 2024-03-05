using Microsoft.AspNetCore.Mvc;
using PoshtoBack.Services;

namespace PoshtoBack.Controllers;

[ApiController]
[Route("[controller]")]
public class GeneratorController : Controller
{
    private readonly SeedService _seedService = new();
    
    [HttpGet]
    [Route("Generate")]
    public async Task<IActionResult> GenerateSeedPhrase()
    {
        var result = _seedService.GenerateSeed();
        
        return Ok(result);
    }
}