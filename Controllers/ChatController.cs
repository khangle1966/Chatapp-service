using System.Threading.Tasks;
using dotnet_chat.Dtos;
using dotnet_chat.Services;
using Microsoft.AspNetCore.Mvc;
using PusherServer;
using System.Collections.Generic;

namespace dotnet_chat.Controllers
{
    [Route("api")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly JsonFileService _jsonFileService;

        public ChatController(JsonFileService jsonFileService)
        {
            _jsonFileService = jsonFileService;
        }

        [HttpPost("messages")]
        public async Task<ActionResult> Message(MessageDTO dto)
        {
          
            var options = new PusherOptions
            {
                Cluster = "ap1",
                Encrypted = true
            };

            var pusher = new Pusher(
                "1817134",
                "8ec6339cb89b7edf0ee4",
                "11fc88541e3b118c7da8",
                options);

            await pusher.TriggerAsync(
                "chat",
                "message",
                new
                {
                    username = dto.Username,
                    message = dto.Message
                });

          
            _jsonFileService.AddMessage(dto);

           
            return Ok(new { message = "Message sent successfully" });
        }
        [HttpGet("getmessages")]
        public ActionResult<IEnumerable<MessageDTO>> GetMessages()
        {
            var messages = _jsonFileService.GetMessages();
            return Ok(messages);
        }

    }
}
