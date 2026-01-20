using AutoMapper;
using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.DTOs.MembershipDtos;
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
}
