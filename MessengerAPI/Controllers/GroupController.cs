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
        [HttpGet(nameof(GetGroupDetails) + "/{groupId}")]
        public async Task<ActionResult<GroupDetailsDTO>> GetGroupDetails(long groupId, string token)
        {
            //await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = id });

            var group = await _context.Groups.Include(x => x.Admins).Include(x => x.Members).FirstAsync(x => x.Id == groupId);

            if (group == null)
            {
                return NotFound();
            }
            if (!IsMember(group, token).GetAwaiter().GetResult())
            {
                return BadRequest("Not authorized.");
            }
            return mapper.Map<GroupDetailsDTO>(group);
        }

        [HttpGet(nameof(GetGroupListByUser))]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(204)]
        public async Task<ActionResult<List<long>>> GetGroupListByUser(TokenDTO tokenDTO)
        {
            var user = await _context.Users.Include(x => x.Groups).FirstAsync(x => x.Id == tokenDTO.Id);
            if (user.UserToken != tokenDTO.UserToken) { return BadRequest("Not authorized");}
            var groupIds = new List<long>();
            user.Groups.ForEach(x => groupIds.Add(x.Id));
            //if (groupIds.Count < 1)
            //{
            //    return NoContent();
            //}
            return groupIds;//mapper.Map<GroupListDTO>(user.Groups);
        }

        // POST: api/Group/CreateGroup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateGroup")]
        public async Task<ActionResult<long>> PostGroup(GroupDetailsDTO groupDetails, string token)
        {
            var members = new List<User>() { };
            foreach( var member in groupDetails.Members)
            {
                var user = await _context.Users.FindAsync(member.Id);
                members.Add(user);    
            }
            var admins = new List<User>() { };
            foreach (var admin in groupDetails.Admins)
            {
                var user = await _context.Users.FindAsync(admin.Id);
                admins.Add(user);
            }
            var group = new Group() { Members= members, Admins=admins, Name=groupDetails.Name };//mapper.Map<Group>(groupDetails);
            if (!IsAdmin(group, token).GetAwaiter().GetResult())
            {
                return BadRequest("Not authorized.");
            }
            /*            if (group.Members.Count <= 1)
                        {
                            return BadRequest("Group must have at least 1 member.");
                        }
                        if (group.Admins.Count <= 1)
                        {
                            return BadRequest("Group must have at least 1 admin.");
                        }*/
            try
            {
                _context.Groups.Add(group);
            }
            catch (Exception ex ){
                return BadRequest(ex.Message);
            }
            await _context.SaveChangesAsync();

            foreach( var member in group.Members)
            {
                await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = member.Id, GroupId = group.Id, MessageType = MessageType.GroupCreated });
            }
            
            return group.Id;//CreatedAtAction("GetGroup", new { id = @group.Id }, @group)
        }

        [HttpPost(nameof(AddGroupMember) + "/{groupId}/{userId}")]
        public async Task<ActionResult<GroupDetailsDTO>> AddGroupMember(long groupId, long userId, string token)
        {
            var group = await _context.Groups.Include(x => x.Admins).Include(x => x.Members).FirstAsync(x => x.Id == groupId);
            var user = await _context.Users.FindAsync(userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            if (!IsAdmin(group, token).GetAwaiter().GetResult())
            {
                return BadRequest("Not authorized.");
            }
            group.Members.Add(user);
            await _context.SaveChangesAsync();
            foreach (var member in group.Members)
            {
                await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = member.Id, GroupId = group.Id, MessageType = MessageType.GroupMemberAdded});
            }
            return mapper.Map<GroupDetailsDTO>(group);
        }

        [HttpPost(nameof(RemoveGroupMember) + "/{groupId}/{userId}")]
        public async Task<ActionResult<GroupDetailsDTO>> RemoveGroupMember(long groupId, long userId, string token)
        {
            var group = await _context.Groups.Include(x => x.Admins).Include(x => x.Members).FirstAsync(x => x.Id == groupId);
            var user = group.Members.Find(x => x.Id == userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            if (!IsAdmin(group, token).GetAwaiter().GetResult())
            {
                return BadRequest("Not authorized.");
            }
            group.Members.Remove(user);
            await _context.SaveChangesAsync();
            foreach (var member in group.Members)
            {
                await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = member.Id, GroupId = group.Id, MessageType = MessageType.GroupMemberRemoved });
            }
            return mapper.Map<GroupDetailsDTO>(group);
        }

        [HttpPost(nameof(AddGroupAdmin) + "/{groupId}/{userId}")]
        public async Task<ActionResult<GroupDetailsDTO>> AddGroupAdmin(long groupId, long userId, string token)
        {
            var group = await _context.Groups.Include(x => x.Admins).Include(x => x.Members).FirstAsync(x => x.Id == groupId);
            var user = group.Members.Find(x => x.Id == userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            if (!IsAdmin(group, token).GetAwaiter().GetResult())
            {
                return BadRequest("Not authorized.");
            }
            //if (group.Members.Count <= 1)
            //{
            //    return BadRequest("Group must have at least 1 member.");
            //}
            if (!group.Admins.Contains(user))
            {
                group.Admins.Add(user);
                await _context.SaveChangesAsync();
                foreach (var member in group.Members)
                {
                    await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = member.Id, GroupId = group.Id, MessageType = MessageType.GroupAdminAdded });
                }
            }
            return mapper.Map<GroupDetailsDTO>(group);
        }

        [HttpPost(nameof(RemoveGroupAdmin) + "/{groupId}/{userId}")]
        public async Task<ActionResult<GroupDetailsDTO>> RemoveGroupAdmin(long groupId, long userId, string token)
        {
            var group = await _context.Groups.Include(x => x.Admins).Include(x => x.Members).FirstAsync(x => x.Id == groupId);
            var user = group.Admins.Find(x => x.Id == userId);
            if (group == null || user == null)
            {
                return NotFound();
            }
            if (!IsAdmin(group, token).GetAwaiter().GetResult()) 
            {
                return BadRequest("Not authorized.");
            }
            if (group.Admins.Count <= 1)
            {
                return BadRequest("Group must have at least 1 admin.");
            }
            group.Admins.Remove(user);
            await _context.SaveChangesAsync();
            foreach (var member in group.Members)
            {
                await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = member.Id, GroupId = group.Id, MessageType = MessageType.GroupAdminRemoved});
            }
            return mapper.Map<GroupDetailsDTO>(group);
        }

        /*CreateGroup 1
        AddGroupMember 0
        ChangeGroupName 0
        RemoveGroupMember 0
        GetGroupDetails 1 */

        [HttpDelete(nameof(DeleteGroup) + "/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteGroup(long id, string token)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            if (!IsAdmin(group, token).GetAwaiter().GetResult())
            {
                return BadRequest("Not authorized.");
            }
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            foreach (var member in group.Members)
            {
                await hubContext.Clients.All.NotifyMessage(new NotifyMessage { UserId = member.Id, GroupId = group.Id, MessageType = MessageType.GroupDeleted });
            }
            return NoContent();
        }

        private async Task<bool> IsAdmin(Group group, string token)
        {
            var user = await _context.Users.FirstAsync(x => x.UserToken == token);
            if (group.Admins.Exists(x => x.Id == user.Id))
            {
                return true;
            }
            else { return false;  }
        }

        private async Task<bool> IsMember(Group group, string token)
        {
            var user = await _context.Users.FirstAsync(x => x.UserToken == token);
            if (group.Members.Exists(x => x.Id == user.Id))
            {
                return true;
            }
            else { return false; }
        }

        private bool GroupExists(long id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
