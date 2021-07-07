using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MessengerAPI.Business;
using MessengerAPI.Models.DbModels;
using MessengerAPI.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using MessengerCommon;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMServerDbContext _context;
        private readonly IHubContext<MessageHub, INotificationClient> hubContext;
        private readonly MessageHub messageHub;

        public MessageController(IMapper mapper, IMServerDbContext context,
            IHubContext<MessageHub, INotificationClient> hubContext,
            MessageHub messageHub)
        {
            this.mapper = mapper;
            _context = context;
            this.hubContext = hubContext;
            this.messageHub = messageHub;
        }

        /*
        // GET: api/Message/5
        [HttpGet("{nameof(GetMessage)}/{id}")]
        public async Task<ActionResult<MessageReceiveDTO>> GetMessage(long messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);

            if (message == null)
            {
                return NotFound();
            }

            return mapper.Map<MessageReceiveDTO>(message);
        }

        [HttpGet(nameof(GetListofMessagesByUserId))]
        public async Task<ActionResult<MessageReceiveListDTO>> GetListofMessagesByIds(List<long> ids)
        {
            List<Message> messages = new List<Message>();
            foreach (var id in ids)
            {
                var message = await _context.Messages.FindAsync(id);
                messages.Add(message);
            }            

            if (messages == null)
            {
                return NotFound();
            }

            return mapper.Map<MessageReceiveListDTO>(messages);+ "/{userId}"
        }*/

        [HttpGet(nameof(GetUsersReceivedMessages))]
        public async Task<ActionResult<MessageReceiveListDTO>> GetUsersReceivedMessages(TokenDTO tokenDTO)
        {

            User user = await _context.Users.FindAsync(tokenDTO.UserID);
            if (user.UserToken != tokenDTO.UserToken)
            {
                return BadRequest("Not Authorized");
            }
            List<Message> messages = user.ReceivedMessages;

            if (messages == null)
            {
                return NotFound();
            }
            // TODO delete Messages after
            return mapper.Map<MessageReceiveListDTO>(messages);
        }

        // POST: api/Message
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(nameof(PostMessage))]
        public async Task<ActionResult<DateTime>> PostMessage(MessageSendDTO messageDTO, string token)
        {

            var message = mapper.Map<Message>(messageDTO);
            if (message.Sender.UserToken != token)
            {
                return BadRequest("Not Authorized");
            }
            message.TimeStamp = DateTime.UtcNow;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = message.Recipient.Id, MessageType = MessageType.MessagePosted });
            return message.TimeStamp;//CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        [HttpPost(nameof(PostMultipleMessages))]
        public async Task<ActionResult> PostMultipleMessages(MessageSendListDTO messageListDTO, string token)
        {
            var recipients = new List<User>();
            foreach(var messageDTO in messageListDTO.MessageList)
            {
                var message = mapper.Map<Message>(messageDTO);
                if (message.Sender.UserToken != token)
                {
                    return BadRequest("Not Authorized");
                }
                message.TimeStamp = DateTime.UtcNow;
                _context.Messages.Add(message);
                recipients.Add(message.Recipient);
            }
            await _context.SaveChangesAsync();
            foreach(var recipient in recipients)
            {
                await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = recipient.Id, MessageType = MessageType.MessagePosted });
            }
            return NoContent();//CreatedAtAction("GetMessage", new { id = message.Id }, message);<List<long>>
        }
        // DELETE: api/Message/5
        [HttpDelete(nameof(DeleteMessage)+"/{id}")]
        public async Task<IActionResult> DeleteMessage(long id)
        {
            
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(long id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
