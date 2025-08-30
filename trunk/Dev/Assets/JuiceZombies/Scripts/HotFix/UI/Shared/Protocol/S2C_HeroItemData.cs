using MessagePack;


namespace HotFix_UI
{
    [MessagePackObject]
    public class S2C_HeroItemData : S2C_ItemData
    {
        [Key(2)] public int Exp { get; set; }
        [Key(3)] public int Level { get; set; }
        [Key(4)] public int Quality { get; set; }
    }
}