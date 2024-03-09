using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PoshtoBack.Data;
using PoshtoBack.Hubs;
using PoshtoBack.Services;

namespace PoshtoBack.Controllers;

[ApiController]
[Route("[controller]")]
public class VoiceChannelController(
    PoshtoDbContext context,
    IHubContext<VoiceHub> voiceHubContext,
    VoiceChannelService voiceChannelService) : Controller
{
    private readonly IUnitOfWork _unitOfWork = new UnitOfWork(context);

    [HttpGet]
    [Route("List")]
    [Authorize]
    public async Task<IActionResult> GetChannels()
    {
        var channels = _unitOfWork.VoiceChannels.GetAll().ToList();
        return Ok(channels);
    }

    [HttpPost]
    [Route("Connect/{channelId:int}")]
    [Authorize]
    public async Task<IActionResult> ConnectUserToChannel(int channelId)
    {
        var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        await voiceChannelService.ConnectUserToChannelAsync(voiceHubContext, channelId, userId);
        
        return Ok();
    }

    [HttpPost]
    [Route("Disconnect/{channelId:int}")]
    [Authorize]
    public async Task<IActionResult> DisconnectUserFromChannel(int channelId)
    {
        var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        await voiceChannelService.DisconnectUserFromChannelAsync(voiceHubContext, channelId, userId);
        
        return Ok();
    }

    [HttpGet]
    [Route("ConnectedUsers/{channelId:int}")]
    [Authorize]
    public IActionResult GetConnectedUsers(int channelId)
    {
        var connectedUsers = voiceChannelService.GetConnectedUsers(channelId, _unitOfWork.VoiceChannels.GetAll().ToList());
        return Ok(connectedUsers);
    }
}