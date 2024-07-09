using Microsoft.AspNetCore.Mvc;
using PoshtoBack.Containers;
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
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = _unitOfWork.Users.Find(w => w.Email == model.Email).FirstOrDefault();
        if (user == null) return NotFound();
        
        var encodedPassword = SeedService.EncodeString(model.Password);
        if (user.PasswordHash != encodedPassword) return NotFound(); 
        
        var token = _jwtService.GenerateToken(user);
        return Ok(new { user, token });
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register(LoginModel model)
    {
        var encodedPassword = SeedService.EncodeString(model.Password);

        var user = new User
        {
            Email = model.Email,
            PasswordHash = encodedPassword,
            AvatarUrl = "https://i.pinimg.com/736x/0d/64/98/0d64989794b1a4c9d89bff571d3d5842.jpg"
        };

        _unitOfWork.Users.Add(user);
        _unitOfWork.Save();

        user.Name = $"user{user.Id}";
        _unitOfWork.Save();
        
        GlobalContainer.DbUsers.Add(user);

        var token = _jwtService.GenerateToken(user);
        return Ok(new { user, token });
    }
}