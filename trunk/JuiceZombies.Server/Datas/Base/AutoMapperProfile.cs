using AutoMapper;
using HotFix_UI;

namespace JuiceZombies.Server.Datas;

// AutoMapperProfile.cs
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {

        CreateMap<ShopData, S2C_ShopData>();
        CreateMap<S2C_ShopData, ShopData>();
        
        CreateMap<S2C_UserResData, UserResData>();
        CreateMap<UserResData, S2C_UserResData>();
    }
}