using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoshtoBack.Data;
using PoshtoBack.Data.Models;

namespace PoshtoBack.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController(PoshtoDbContext context) : Controller
{
    private readonly IUnitOfWork _unitOfWork = new UnitOfWork(context);
    
    [HttpGet]
    [Route("Get")]
    [Authorize]
    public async Task<IActionResult> GetMessages(int skip, int take)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var messages = _unitOfWork.Messages
            .GetAll()
            .OrderBy(o => o.Timestamp)
            .Skip(skip).Take(take)
            .ToList()
            .Adapt<List<MessageDto>>();
        
        foreach (var messageDto in messages)
        {
            messageDto.IsFromMe = messageDto.SenderId.ToString() == userId;
        }
        
        return Ok(messages);
    }

    [HttpPost]
    [Route("Add")]
    [Authorize]
    public async Task<IActionResult> AddMessage(AddMessageDto model)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var message = new Message()
        {
            SenderId = Convert.ToInt32(userId),
            Timestamp = DateTime.Now,
            Text = model.Text
        };
        
        _unitOfWork.Messages.Add(message);
        _unitOfWork.Save();

        var result = message.Adapt<MessageDto>();
        result.IsFromMe = true;
        
        return Ok(result);
    }
}