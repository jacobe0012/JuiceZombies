using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

[Index(nameof(UserId), IsUnique = true)]
public class HeroData
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    public int ConfigId { get; set; }

    public int Level { get; set; }

    public int Quality { get; set; }
}