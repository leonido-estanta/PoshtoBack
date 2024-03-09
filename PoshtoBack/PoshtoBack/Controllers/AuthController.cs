using Microsoft.AspNetCore.Mvc;
using PoshtoBack.Data;
using PoshtoBack.Services;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(PoshtoDbContext context) : Controller
{
    private readonly IUnitOfWork _unitOfWork = new UnitOfWork(context);
    private readonly IJwtService _jwtService = new JwtService();

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(string seedPhrase)
    {
        var encodedSeed = SeedService.EncodeSeed(seedPhrase);
        var user = _unitOfWork.Users.Find(w => w.PasswordHash == encodedSeed).FirstOrDefault();
        if (user == null) return NotFound();
        
        var token = _jwtService.GenerateToken(user);
        return Ok(new { user, token });
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(string seedPhrase)
    {
        var encodedSeed = SeedService.EncodeSeed(seedPhrase);

        var newUser = new User
        {
            PasswordHash = encodedSeed,
            AvatarUrl = "https://i.pinimg.com/736x/0d/64/98/0d64989794b1a4c9d89bff571d3d5842.jpg"
        };

        _unitOfWork.Users.Add(newUser);
        _unitOfWork.Save();

        newUser.Name = $"user{newUser.Id}";
        _unitOfWork.Save();

        return Ok();
    }
}