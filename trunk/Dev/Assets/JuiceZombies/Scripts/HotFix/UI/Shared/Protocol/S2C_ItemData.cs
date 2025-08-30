using MessagePack;


namespace HotFix_UI
{
   
    [MessagePackObject]
    [Union(0, typeof(S2C_BagItemData))]
    [Union(1, typeof(S2C_HeroItemData))]
    public abstract class S2C_ItemData
    {
        [Key(0)] public int ConfigId { get; set; }
        [Key(1)] public long Count { get; set; }
    }
}