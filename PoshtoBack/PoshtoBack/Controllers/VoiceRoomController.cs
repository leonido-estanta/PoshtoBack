using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data;
using PoshtoBack.Hubs;
using PoshtoBack.Services;

namespace PoshtoBack.Controllers;

[ApiController]
[Route("[controller]")]
public class VoiceRoomController(
    PoshtoDbContext context,
    IHubContext<VoiceHub> voiceHubContext,
    VoiceRoomService voiceRoomService) : Controller
{
    private readonly IUnitOfWork _unitOfWork = new UnitOfWork(context);

    [HttpGet]
    [Route("List")]
    [Authorize]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = _unitOfWork.VoiceRooms.GetAll().ToList();
        return Ok(rooms);
    }
}