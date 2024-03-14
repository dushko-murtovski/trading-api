using System;
using System.Globalization;
using AutoMapper;
using StockwolfTrading.ViewModel.Auth;
using StockWolfTrading.Core.Models;

namespace StockwolfTrading.ViewModel
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AuthenticateRequest, User>();
            CreateMap<User, AuthenticateResponse>().ReverseMap();
            //CreateMap<RegisterRequest, Users>()
            //    .BeforeMap((s, d) => d.IsExpire = false)
            //    .BeforeMap((s, d) => d.IsFirstLogin = true)
            //    .BeforeMap((s, d) => d.Expire = DateTime.Now.AddDays(1))
            //    .BeforeMap((s, d) => d.VerificationCode = Guid.NewGuid().ToString());

            //CreateMap<UpdateRequest, Users>()
            //    .ForAllMembers(x => x.Condition(
            //        (src, dest, prop) =>
            //        {
            //            // ignore null & empty string properties
            //            if (prop == null) return false;
            //            return prop.GetType() != typeof(string) || !string.IsNullOrEmpty((string)prop);
            //        }
            //    ));
        }
    }
}
