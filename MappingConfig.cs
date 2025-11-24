using AutoMapper;
using MinimalAPI.Dto;
using MinimalAPI.Models;
using System.Reflection;

namespace MinimalAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CouponDto, Coupon>()
                            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Coupon, CouponDtoWithoutId>().ReverseMap();
        }
    }
}
