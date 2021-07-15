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
                cfg.CreateMap<Group, GroupDetailsDTO>()
                    .ForMember(dest => dest.Admins, opt => opt.MapFrom(src => src.Admins))
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));
                cfg.CreateMap<GroupDetailsDTO, Group>()
                    .ForMember(dest => dest.Admins, opt => opt.MapFrom(src => src.Admins))
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));
                cfg.CreateMap<GroupUpdateNameDTO, Group>();

                /// Mapping for Message Controller
                cfg.CreateMap<MessageSendDTO, Message>()
                    .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient))
                    .ForMember(dest => dest.Sender, opt => opt.MapFrom(src=> src.Sender));
                cfg.CreateMap<Message, MessageReceiveDTO>()
                    .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender)); //.Id??
                /// Mapping for User Controller
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<User, UserDTO>();
                
                cfg.CreateMap<UserRegisterDTO, User>()
                    .ForMember(dest => dest.PublicKeys, opt => opt.MapFrom(src => src.PublicKeys));
                cfg.CreateMap<User, UserDetailsDTO>()
                    .ForMember(dest => dest.PublicKeys, opt => opt.MapFrom(src => src.PublicKeys));
                cfg.CreateMap<UserLoginDTO, User>();
                cfg.CreateMap<User, TokenDTO>();
                cfg.CreateMap<PublicKey, PublicKeyDTO>();
                cfg.CreateMap<PublicKeyDTO, PublicKey>();
                cfg.CreateMap<EphemeralKey, EphemKeyDTO>()
                    .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.Initiator))
                    .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner));
                cfg.CreateMap<EphemKeyDTO, EphemeralKey>();
                //    .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.InitiatorId))
                //    .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.OwnerId));

            });

            return config.CreateMapper();
        }
    }
}

