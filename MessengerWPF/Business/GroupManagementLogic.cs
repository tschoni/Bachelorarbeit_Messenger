using MessengerApiClient;
using MessengerWPF.Models.DbModels;
using Microsoft.EntityFrameworkCore;
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



        public async Task AddGroupAsync( string name, List<User> members)
        {
            var memberDTOs = new List<UserDTO>();
            var me = dbContext.Users.Find(tokenAndId.Id);
            memberDTOs.Add(new UserDTO() { Id = me.Id, Name= me.Name });
            foreach(var member in members)
            {
                memberDTOs.Add(new UserDTO() { Id = member.Id, Name = member.Name });
            }
            
            var groupDTO = new GroupDetailsDTO() { Name = name, Admins = new List<UserDTO>() { new UserDTO() { Id = me.Id, Name = me.Name } }, Members = memberDTOs };
            var groupId = await apiClient.CreateGroupAsync(tokenAndId.Token, groupDTO);
            //var group = new Group() { Name = name , Id = groupId, Members=members, Admins =new List<User>(), Messages = new List<GroupMessage>()};FirstAsync
            var group = new Group() { Name = name, Id = groupId, Members = new List<User>(), Admins = new List<User>(), Messages = new List<GroupMessage>() };
            foreach(var member in members)
            {
                group.Members.Add(await dbContext.Users.FindAsync(member.Id));
            }
            group.Admins.Add(me);
            group.Members.Add(me);
            dbContext.Groups.Add(group);
            await dbContext.SaveChangesAsync();

        }

        public async Task UpdateGroupByIdAsync(long id)
        {
            var groupDTO = await apiClient.GetGroupDetailsAsync(id, tokenAndId.Token);
            var group = await dbContext.Groups.Include(x => x.Members).Include(x => x.Admins).FirstOrDefaultAsync(x => x.Id == id);
            if (group == null)
            {
                group = new Group() { Id = id, Name = groupDTO.Name, Members = new List<User>(), Admins = new List<User>(), Messages = new List<GroupMessage>() };
            }
            group = await UpdateGroupAsync(group, groupDTO);
            await dbContext.SaveChangesAsync();
        }

        private async Task<Group> UpdateGroupAsync( Group group, GroupDetailsDTO groupDTO)
        {
            var members = new List<User>();
            foreach (var memberDTO in groupDTO.Members)
            {
                var contact = await dbContext.Users.FindAsync(memberDTO.Id);
                if (contact == null)
                {
                    await contactInitiation.InitiateKeyExchangeByIdAsync(memberDTO.Id);
                }
                members.Add(contact);
            }
            var admins = new List<User>();
            foreach (var adminDTO in groupDTO.Admins)
            {
                admins.Add(await dbContext.Users.FindAsync(adminDTO.Id));
            }
            group.Name = groupDTO.Name;
            group.Members = members;
            group.Admins = admins;
            return group;

        }

        public async Task UpdateAllGroupAsync()
        {
            var groupIds = await apiClient.GetGroupListByUserAsync(new TokenDTO() {Id = tokenAndId.Id, UserToken = tokenAndId.Token });
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
            await dbContext.SaveChangesAsync();
            //TODO austreten aus Gruppe + nur löschen wenn admin oder ausgetreten
        }
    }
}
