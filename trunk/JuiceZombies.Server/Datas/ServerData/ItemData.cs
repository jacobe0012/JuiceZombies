using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

[Index(nameof(UserId))]
public abstract class ItemData
{
    public uint Id { get; set; }
    public uint UserId { get; set; }

    public int ConfigId { get; set; }
    
    public long Count { get; set; }
}