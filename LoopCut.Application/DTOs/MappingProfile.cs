using AutoMapper;
using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.DTOs.MembershipDtos;
using LoopCut.Application.DTOs.PaymentDTO;
using LoopCut.Application.DTOs.UserMembershipDtos;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;

namespace LoopCut.Application.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Config Mapper Paging
            CreateMap(typeof(BasePaginatedList<>), typeof(BasePaginatedList<>))
                .ConvertUsing(typeof(BasePaginatedListConverter<,>));
            // Mapping
            CreateMap<Accounts, AccountResponse>();
            CreateMap<Membership, MembershipResponse>();
            CreateMap<UserMembership, UserMembershipResponse>()
                .ForMember(dest => dest.MembershipName, opt => opt.MapFrom(src => src.Membership.Name))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName));
            CreateMap<UserMembership, UserMembershipDetail>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.User.LastUpdatedAt))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role));

            CreateMap<Membership, MembershipDetail>()
                .ForMember(d => d.MembershipId, opt => opt.MapFrom(s => s.Id));
            CreateMap<Payment, PaymentDetailResponse>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderCode))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.MembershipId, opt => opt.MapFrom(src => src.Membership.Id))
                .ForMember(dest => dest.MembershipName, opt => opt.MapFrom(src => src.Membership.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }

    }
        public class BasePaginatedListConverter<TSource, TDestination> : ITypeConverter<BasePaginatedList<TSource>, BasePaginatedList<TDestination>>
        {
            public BasePaginatedList<TDestination> Convert(
                BasePaginatedList<TSource> source,
                BasePaginatedList<TDestination> destination,
                ResolutionContext context)
            {
                var mappedItems = context.Mapper.Map<List<TDestination>>(source.Items);

                return new BasePaginatedList<TDestination>(
                    mappedItems,
                    source.TotalItems,
                    source.PageIndex,
                    source.PageSize
                );
            }
        }
    }

