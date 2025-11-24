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
                .ForAllMembers(opts =>
                {
                    opts.Condition((src, dest, srcMember) =>
                    {
                        if (srcMember == null) return false;
                        var memberType = srcMember.GetType();
                        if (memberType == typeof(int) && (int)srcMember == default(int))
                            return false;
                        if (memberType == typeof(bool) && (bool)srcMember == default(bool))
                            return false;
                        // int? ve bool? nullable olduğundan default kontrolü gerekmez
                        return true;
                    });
                });
            CreateMap<Coupon, CouponDtoWithoutId>().ReverseMap();
        }
    }
}
