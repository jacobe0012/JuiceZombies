using System.Diagnostics;

namespace JuiceZombies.Server.Log;

//using System.Diagnostics;

public class MyLogger
{
    // 用于打印日志信息
    public static void Log(string cmd, string userId, string input, string output, Stopwatch stopwatch)
    {
        // 执行用户的动作方法
        //methodToExecute.Invoke();
        //Console.SetBufferSize(1000, Console.BufferHeight);
        // 获取当前时间
        //string timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            // 获取北京时区（中国标准时间）
        TimeZoneInfo beijingZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

        // 将 UTC 时间转换成北京时间
        DateTime beijingTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, beijingZone);

        // 格式化为字符串
        string timeStamp = beijingTime.ToString("yyyy-MM-dd HH:mm:ss");
        // 格式化输出日志
        string outputStr = $@"
╔════════════════════════════════════════════════════════════════════════════════════╗
║-CMD:      {cmd}
║-Time:     {timeStamp}
║-UserId:   {userId}
║-Input:    {input}
║-Output:   {output}
║-Duration: {stopwatch.ElapsedMilliseconds} ms
╚════════════════════════════════════════════════════════════════════════════════════╝
";

        // 打印日志
        Console.WriteLine(outputStr);
    }
}