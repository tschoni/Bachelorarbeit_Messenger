using MessengerApiClient;
using MessengerWPF.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    public class GroupManagementLogic : BusinessLogicBase
    {
        private readonly ContactInitiationLogic contactInitiation;

        public GroupManagementLogic(IMApiClient apiClient, IMClientDbContext dbContext, TokenAndIdProvider tokenAndId, ContactInitiationLogic contactInitiation) : base(apiClient, dbContext, tokenAndId)
        {
            this.contactInitiation = contactInitiation;
        }



        public async Task AddGroupAsync( string name, List<long> memberIds)
        {
            memberIds.Add(tokenAndId.Id);
            var groupDTO = new GroupDetailsDTO() { Name = name, AdminIds = new List<long>() { tokenAndId.Id }, MemberIds = memberIds };
            var groupId = await apiClient.CreateGroupAsync(tokenAndId.Token, groupDTO);
            var group = new Group() { Name = name , Id = groupId};
            var me = await dbContext.Users.FindAsync(tokenAndId.Id);
            group.Admins.Add(me);
            foreach(var memberId in memberIds)
            {
                var member = await dbContext.Users.FindAsync(memberId);
                group.Members.Add(member);
            }
            dbContext.Add(group);
            await dbContext.SaveChangesAsync();

        }

        public async Task UpdateGroupByIdAsync(long id)
        {
            var groupDTO = await apiClient.GetGroupDetailsAsync(id, tokenAndId.Token);
            var group = await  dbContext.Groups.FindAsync(id);
            if (group == null)
            {
                group = new Group() { Id = id, Name = groupDTO.Name };
            }
            group = await UpdateGroupAsync(group, groupDTO);
            await dbContext.SaveChangesAsync();
        }

        private async Task<Group> UpdateGroupAsync( Group group, GroupDetailsDTO groupDTO)
        {
            var members = new List<User>();
            foreach (var memberId in groupDTO.MemberIds)
            {
                var contact = await dbContext.Users.FindAsync(memberId);
                if (contact == null)
                {
                    await contactInitiation.InitiateKeyExchangeByIdAsync(memberId);
                }
                members.Add(contact);
            }
            var admins = new List<User>();
            foreach (var adminId in groupDTO.AdminIds)
            {
                admins.Add(await dbContext.Users.FindAsync(adminId));
            }
            group.Name = groupDTO.Name;
            group.Members = members;
            group.Admins = admins;
            return group;

        }

        public async Task UpdateAllGroupAsync()
        {
            var groupIds = await apiClient.GetGroupListByUserAsync(new TokenDTO() {UserID = tokenAndId.Id, UserToken = tokenAndId.Token });
            if (groupIds == null)
            {
                return;
            }
            foreach (var groupId in groupIds)
            {
                await UpdateGroupByIdAsync(groupId);
            }
            
        }

        public async Task AddGroupMemberAsync(long id)
        {
            await apiClient.AddGroupMemberAsync(id, tokenAndId.Id, tokenAndId.Token);
            
        }

        public async Task RemoveGroupMemberAsync(long id)
        {
            await apiClient.RemoveGroupMemberAsync(id, tokenAndId.Id, tokenAndId.Token);
        }
        public async Task AddGroupAdminAsync(long id)
        {
            await apiClient.AddGroupAdminAsync(id, tokenAndId.Id, tokenAndId.Token);
        }
        public async Task RemoveGroupAdminAsync(long id)
        {
            await apiClient.RemoveGroupAdminAsync(id, tokenAndId.Id, tokenAndId.Token);
        }

        //TODO delete Group when other admin has deleted it
        public async Task DeleteGroupAsync(long id)
        {
            var group = await dbContext.Groups.FindAsync(id);
            dbContext.Groups.Remove(group);
            await apiClient.DeleteGroupAsync(id, tokenAndId.Token);

        }
    }
}
