using Microsoft.AspNetCore.Mvc;
using PoshtoBack.Data;
using PoshtoBack.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(PoshtoDbContext context) : Controller
{
    private readonly SeedService _seedService = new();
    private readonly IUnitOfWork _unitOfWork = new UnitOfWork(context);
    private readonly IJwtService _jwtService = new JwtService();

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(string seedPhrase)
    {
        var encodedSeed = _seedService.EncodeSeed(seedPhrase);
        var user = _unitOfWork.Users.Find(w => w.PasswordHash == encodedSeed).FirstOrDefault();
        if (user == null) return NotFound();
        
        var token = _jwtService.GenerateToken(user);
        return Ok(new { user, token });
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(string seedPhrase)
    {
        var encodedSeed = _seedService.EncodeSeed(seedPhrase);
        _unitOfWork.Users.Add(new User
        {
            PasswordHash = encodedSeed
        });
        
        _unitOfWork.Save();
        
        return Ok();
    }
    
    [HttpGet]
    [Route("Protected")]
    [Authorize]
    public IActionResult Protected()
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return Ok($"Access to protected resource, User ID: {userId}");
    }
}