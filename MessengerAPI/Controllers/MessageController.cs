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

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMServerDbContext _context;

        public MessageController(IMapper mapper, IMServerDbContext context)
        {
            this.mapper = mapper;
            _context = context;
        }


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

            return mapper.Map<MessageReceiveListDTO>(messages);
        }

        [HttpGet(nameof(GetListofMessagesByUserId)+ "/{userId}")]
        public async Task<ActionResult<MessageReceiveListDTO>> GetListofMessagesByUserId(long userId)
        {

            User user = await _context.Users.FindAsync(userId);
            List<Message> messages = user.ReceivedMessages;

            if (messages == null)
            {
                return NotFound();
            }

            return mapper.Map<MessageReceiveListDTO>(messages);
        }

        // POST: api/Message
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(nameof(PostMessage))]
        public async Task<ActionResult<long>> PostMessage(MessageSendDTO messageDTO)
        {
            var message = mapper.Map<Message>(messageDTO);
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return message.Id;//CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        [HttpPost(nameof(PostMultipleMessages))]
        public async Task<ActionResult> PostMultipleMessages(MessageSendListDTO messageListDTO)
        {
            foreach(var messageDTO in messageListDTO.MessageList)
            {
                var message = mapper.Map<Message>(messageDTO);
                _context.Messages.Add(message);
            }
            await _context.SaveChangesAsync();

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
