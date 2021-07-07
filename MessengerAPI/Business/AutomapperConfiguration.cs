using AutoMapper;
using MessengerAPI.Models.DbModels;
using MessengerAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Business
{
    public class AutomapperConfiguration
    {
        public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                /// Mapping for Group Controller
                cfg.CreateMap<Group, GroupDetailsDTO>();
                cfg.CreateMap<GroupDetailsDTO, Group>();
                cfg.CreateMap<GroupUpdateNameDTO, Group>();

                /// Mapping for Message Controller
                cfg.CreateMap<MessageSendDTO, Message>();
                cfg.CreateMap<Message, MessageReceiveDTO>();
                /// Mapping for User Controller
                cfg.CreateMap<User, UserRegisterDTO>();
                cfg.CreateMap<UserDetailsDTO, User>();
                cfg.CreateMap<UserLoginDTO, User>();
                cfg.CreateMap<User, TokenDTO>();
                cfg.CreateMap<PublicKey, PublicKeyDTO>();
                cfg.CreateMap<PublicKeyDTO, PublicKey>();
            });

            return config.CreateMapper();
        }
    }
}

