using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JuiceZombies.Server.Datas;

[Index(nameof(UserId), IsUnique = true)]
public class GachaData
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    /// <summary>
    /// 池子id,累计未抽到次数
    /// </summary>
    public string Pity_IdCounter { get; set; }
    
    // 这个属性不会被映射，它是你的业务逻辑所用的 ConcurrentDictionary
    [NotMapped]
    public ConcurrentDictionary<int, int> MyPity_IdCounter
    {
        get
        {
            // 反序列化：从数据库中读取 JSON 并转换为 ConcurrentDictionary
            if (string.IsNullOrEmpty(Pity_IdCounter))
            {
                return new ConcurrentDictionary<int, int>();
            }

            // JsonSerializer 默认可以直接反序列化为 ConcurrentDictionary

            return JsonConvert.DeserializeObject<ConcurrentDictionary<int, int>>(Pity_IdCounter);
        }
        set
        {
            // 序列化：将 ConcurrentDictionary 转换为 JSON 字符串以保存到数据库
            Pity_IdCounter = JsonConvert.SerializeObject(value);
        }
    }
}