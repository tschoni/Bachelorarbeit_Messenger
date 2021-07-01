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
    public class GroupController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMServerDbContext _context;
        private readonly IHubContext<MessageHub, INotificationClient> hubContext;
        private readonly MessageHub messageHub;

        public GroupController(IMapper mapper, IMServerDbContext context,
            IHubContext<MessageHub, INotificationClient> hubContext,
            MessageHub messageHub)
        {
            this.mapper = mapper;
            _context = context;
            this.hubContext = hubContext;
            this.messageHub = messageHub;
        }

        // GET: api/Group/GetGroupDetails/5
        [HttpGet("GetGroupDetails/{id}")]
        public async Task<ActionResult<GroupDetailsDTO>> GetGroup(long id)
        {
            await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = id });

            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            return mapper.Map<GroupDetailsDTO>(group);
        }


        // POST: api/Group/CreateGroup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateGroup")]
        public async Task<ActionResult<Group>> PostGroup(GroupDetailsDTO groupDetails)
        {

            // await hubContext.Clients.All.NotifyMessage()


            var @group = mapper.Map<Group>(groupDetails);
            _context.Groups.Add(@group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = @group.Id }, @group);
        }

        [HttpPost(nameof(AddGroupMember) + "/{id}")]
        public async Task<ActionResult<GroupDetailsDTO>> AddGroupMember(long groupId, long userId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            var user = await _context.Users.FindAsync(userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            group.Members.Add(user);
            await _context.SaveChangesAsync();
            return mapper.Map<GroupDetailsDTO>(group);
        }

        [HttpPost(nameof(RemoveGroupMember) + "/{groupId}/{userId}")]
        public async Task<ActionResult<GroupDetailsDTO>> RemoveGroupMember(long groupId, long userId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            var user = group.Members.Find(x => x.Id == userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            group.Members.Remove(user);
            await _context.SaveChangesAsync();
            return mapper.Map<GroupDetailsDTO>(group);
        }

        [HttpPost(nameof(AddGroupAdmin) + "/{groupId}/{userId}")]
        public async Task<ActionResult<GroupDetailsDTO>> AddGroupAdmin(long groupId, long userId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            var user = group.Members.Find(x => x.Id == userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            if (group.Members.Count <= 1)
            {
                return BadRequest("Group must have at least 1 member.");
            }
            if (!group.Admins.Contains(user))
            {
                group.Admins.Add(user);
                await _context.SaveChangesAsync();
            }
            return mapper.Map<GroupDetailsDTO>(group);
        }

        [HttpPost(nameof(RemoveGroupAdmin) + "/{groupId}/{userId}")]
        public async Task<ActionResult<GroupDetailsDTO>> RemoveGroupAdmin(long groupId, long userId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            var user = group.Admins.Find(x => x.Id == userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            if (group.Admins.Count <= 1)
            {
                return BadRequest("Group must have at least 1 admin.");
            }
            group.Admins.Remove(user);
            await _context.SaveChangesAsync();
            return mapper.Map<GroupDetailsDTO>(group);
        }

        /*CreateGroup 1
        AddGroupMember 0
        ChangeGroupName 0
        RemoveGroupMember 0
        GetGroupDetails 1 */

        [HttpDelete(nameof(DeleteGroup) + "/{id}")]
        public async Task<IActionResult> DeleteGroup(long id)
        {
            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(@group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupExists(long id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
