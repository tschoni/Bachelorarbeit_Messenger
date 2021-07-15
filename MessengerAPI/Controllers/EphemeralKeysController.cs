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
using MessengerCommon;
using Microsoft.AspNetCore.SignalR;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EphemeralKeysController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMServerDbContext _context;
        private readonly IHubContext<MessageHub, INotificationClient> hubContext;
        private readonly MessageHub messageHub;

        public EphemeralKeysController(IMapper mapper, IMServerDbContext context, 
            IHubContext<MessageHub, INotificationClient> hubContext,
            MessageHub messageHub)
        {
            this.mapper = mapper;
            _context = context;
            this.hubContext = hubContext;
            this.messageHub = messageHub;
        }


        // GET: api/EphemeralKeys/5
        [HttpGet(nameof(GetEphemeralKeys))]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(200)]
        public async Task<ActionResult<EphemKeyListDTO>> GetEphemeralKeys(long userId, string token)
        {
            var user = await _context.Users.FindAsync(userId);//.Include(x => x.EphemeralKeys).ThenInclude(x => x.Owner).FirstAsync(x => x.Id == userId);
            if (user.UserToken != token)
            {
                return BadRequest("Not authorized.");
            }
            var ephemeralKeys = _context.EphemeralKeys.Include(x => x.Initiator).Where(x => x.Owner.Id == user.Id);//user.EphemeralKeys;

            if (ephemeralKeys == null)
            {
                return NotFound();
            }
            var keyList = new List<EphemKeyDTO>();
            foreach (var ephemeralKey in ephemeralKeys)
            {
                var ephemKey = mapper.Map<EphemKeyDTO>(ephemeralKey);
                keyList.Add(ephemKey);
            }
            //if (keyList.Count <= 1)
            //{
            //    return NoContent();
            //}
            return new EphemKeyListDTO() { KeyDTOs = keyList };
        }


        // POST: api/EphemeralKeys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(nameof(PostEphemeralKey))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PostEphemeralKey(EphemKeyDTO ephemeralKey, string token)
        {
            
            var ephemKey = mapper.Map<EphemeralKey>(ephemeralKey);
            ephemKey.Initiator = await _context.Users.FindAsync(ephemeralKey.Initiator.Id);
            ephemKey.Owner = await _context.Users.FindAsync(ephemeralKey.Owner.Id);
            if (ephemKey.Initiator.UserToken != token)
            {
                return BadRequest("Not authorized.");
            }
            _context.EphemeralKeys.Add(ephemKey);
            await _context.SaveChangesAsync();
            await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = ephemeralKey.Owner.Id, MessageType = MessageType.EphemKeyPosted });
            return NoContent();
        }

        // DELETE: api/EphemeralKeys/5
        [HttpDelete(nameof(DeleteEphemeralKey))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEphemeralKey(long id, string token)
        {
            var ephemeralKey = await _context.EphemeralKeys.Include(x => x.Owner).FirstAsync(x => x.Id == id);
            if (ephemeralKey == null)
            {
                return NotFound();
            }
            if (ephemeralKey.Owner.UserToken != token)
            {
                return BadRequest("Not authorized.");
            }
            _context.EphemeralKeys.Remove(ephemeralKey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EphemeralKeyExists(long id)
        {
            return _context.EphemeralKeys.Any(e => e.Id == id);
        }
    }
}
