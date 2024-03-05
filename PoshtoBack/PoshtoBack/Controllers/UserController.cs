using Mapster;
using Microsoft.AspNetCore.Mvc;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(PoshtoDbContext context) : Controller
{
    private readonly IUnitOfWork _unitOfWork = new UnitOfWork(context);

    [HttpGet]
    [Route("List")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _unitOfWork.Users.GetAll().ToList().Adapt<List<UserDto>>();

        foreach (var user in users)
        {
            user.Name = !string.IsNullOrEmpty(user.Name) ? user.Name : string.Empty;
        }

        return Ok(users);
    }
}