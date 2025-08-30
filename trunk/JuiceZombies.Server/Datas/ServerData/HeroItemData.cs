using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

public class HeroItemData : ItemData
{
    public int Exp { get; set; }

    public int Level { get; set; }

    public int Quality { get; set; }
}