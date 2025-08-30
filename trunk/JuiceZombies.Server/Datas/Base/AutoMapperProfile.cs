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

        
        
        
        // 1. 基类映射：告诉 AutoMapper 如何处理继承关系
        CreateMap<ItemData, S2C_ItemData>()
            .IncludeAllDerived();
        CreateMap<S2C_ItemData, ItemData>()
            .IncludeAllDerived();

        // 2. 子类映射：为每个具体的子类配置双向映射
        CreateMap<HeroItemData, S2C_HeroItemData>();
        CreateMap<S2C_HeroItemData, HeroItemData>();

        CreateMap<BagItemData, S2C_BagItemData>();
        CreateMap<S2C_BagItemData, BagItemData>();
    }
}