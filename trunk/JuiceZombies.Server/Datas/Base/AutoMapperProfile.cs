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

        CreateMap<S2C_ItemData, ItemData>();
        CreateMap<ItemData, S2C_ItemData>();

        CreateMap<S2C_HeroItemData, HeroItemData>();
        CreateMap<HeroItemData, S2C_HeroItemData>();

        CreateMap<S2C_BagItemData, BagItemData>();
        CreateMap<BagItemData, S2C_BagItemData>();
    }
}