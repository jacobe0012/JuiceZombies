using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

[Index(nameof(UserId), IsUnique = true)]
public class GachaData
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    /// <summary>
    /// 池子id,累计未抽到次数
    /// </summary>
    public ConcurrentDictionary<int, int> Pity_IdCounter { get; set; }
}